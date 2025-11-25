using System.Collections;
using UnityEngine;
using UnityEngine.UI;

abstract public class Monster : MonoBehaviour
{
    [Header("HP Bar")]
    public Image hpBarImage;
    [SerializeField] private Transform hpBarTransform; // HpBar 오브젝트를 드래그

    public MonsterSO monsterData;
    protected Transform player;
    
    // 인스턴스 스탯 (웨이브에 따라 달라짐)
    private int scaledMaxHealth;
    private int currentHealth;
    private int attackDamage;
    private float maxSpeed;
    private float curSpeed; // 추가
    protected int windupTimer = 0; // 추가
    
    private float damageEffectDuration = 0.5f;

    protected IMonsterPattern AttackPattern = null;

    [SerializeField] private int goldReward = 10;
    [HideInInspector] public bool isClone = false;
    public Vector3 fixedPosition = new Vector3(0f, 0f, 0f);

    protected Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;            // 원래의 색상을 저장하는 용도의 변수

    [HideInInspector] public GameObject poolPrefabRef; // 풀 반환용 프리팹 참조
    [Header("피격 이펙트")]
    [SerializeField] private GameObject damageEffect;

    // 외부에서 공격력 참조용 (추가)
    public int AttackDamage => attackDamage;

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = transform.Find("Sprite")?.GetComponent<SpriteRenderer>();

        //원래 색상 저장
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        // "Player" 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // 클론이 아닌 경우만 기본 값으로 초기화 (프리팹 원본)
        if (!isClone && monsterData != null)
        {
            monsterData.CacheBase(); // 원본 캐시
            
            int hp, atk;
            float spd;
            monsterData.GetStatsForWave(1, out hp, out atk, out spd);
            
            scaledMaxHealth = hp;
            currentHealth = hp;
            attackDamage = atk;
            maxSpeed = spd;
            curSpeed = maxSpeed;
            
            UpdateHpBar();
        }
    }

    protected void Awake()
    {
        RhythmManager.beatUpdated += OnBeat;
    }

    private void OnDestroy()
    {
        RhythmManager.beatUpdated -= OnBeat;
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (player == null)
        {
            Debug.LogWarning("플레이어를 찾을 수 없음");
            return;
        }

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * curSpeed * Time.fixedDeltaTime;
        transform.LookAt(player);
    }

    // 공격 로직 판단 + 수행 함수
    protected virtual void Attack()
    {
        if (!isActiveAndEnabled) return;

        // 플레이어와의 거리
        float distance = Vector3.Distance(transform.position, player.position);
        
        // 공격 범위에 들어오면
        if (distance <= monsterData.attackRange)
        {
            animator.SetBool("isWindup", true); // 공격 준비 모션

            windupTimer++;

            if (windupTimer > monsterData.attackWindup)
            {
                // 패턴에 Monster 인스턴스 전달하도록 수정 필요
                AttackPattern?.AttackPattern(transform, player, animator, monsterData);
                windupTimer = 0;
            }
        }
        else
        {
            windupTimer = 0; // 범위 벗어나면 타이머 초기화
            animator.SetBool("isWindup", false);
            animator.SetBool("isAttack", false);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Monster took {damage} damage, current HP: {currentHealth}");

        UpdateHpBar();      //체력바 수정

        if (damageEffect != null)
        {
            GameObject eff = PoolManager.Instance.Get(damageEffect);
            eff.transform.position = transform.position;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // 피격 데미지
            StartCoroutine(DamageCoroutine());
        }
    }

    private void UpdateHpBar()
    {
        if (hpBarImage != null && scaledMaxHealth > 0)
            hpBarImage.fillAmount = (float)currentHealth / scaledMaxHealth;
    }
    
    void LateUpdate()
    {
        if (hpBarTransform != null)
            hpBarTransform.LookAt(Camera.main.transform);
    }

    private void Die()
    {
        Debug.Log("Monster is Dead!");
        // TODO : 풀링 구현
        // EnemyPool.Instance.ReturnToPool(this);


        // 플레이어 골드 획득********
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerController pc = playerObj.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.AddGold(goldReward);
                pc.OnMonsterKilled();
            }
        }

        // 풀로 반환
        PoolManager.Instance.Return(poolPrefabRef, gameObject);
    }

    /// <summary>
    /// 웨이브 스폰 시 호출: 웨이브별 스탯 설정
    /// </summary>
    public void Initialize(int hp, int atk, float spd)
    {
        isClone = true;
        scaledMaxHealth = hp;
        currentHealth = hp;
        attackDamage = atk;
        maxSpeed = spd;
        curSpeed = maxSpeed;
        
        UpdateHpBar();
        
        Debug.Log($"[Monster.Initialize] HP:{hp}, ATK:{atk}, SPD:{spd}");
    }

    public void AdjustSpeed(float factor)
    {
        maxSpeed *= factor;
    }
    
    public void ResetSpeed()
    {
        // 클론은 Initialize된 maxSpeed 유지
        if (!isClone && monsterData != null)
        {
            int hp, atk;
            float spd;
            monsterData.GetStatsForWave(1, out hp, out atk, out spd);
            maxSpeed = spd;
        }
    }

    private void OnBeat()
    {
        if (!isActiveAndEnabled) return;

        Attack();
        StartCoroutine(PulsateAnimation());
    }

    private IEnumerator PulsateAnimation()
    {
        float startAnimSpeed = 2f;
        float timer = 0f;
        float duration = 60f / RhythmManager.Instance.CurrentBpm;

        if (animator == null) yield break;

        animator.speed = startAnimSpeed;

        while (timer < duration)
        {
            if (this == null || animator == null) yield break;

            timer += Time.deltaTime;
            curSpeed = Mathf.Lerp(maxSpeed, 0f, timer / duration);
            animator.speed = Mathf.Lerp(startAnimSpeed, 0f, timer / duration);
            yield return null;
        }

        if (animator != null)
            animator.speed = 0.1f;
    }

    private IEnumerator DamageCoroutine()
    {
        animator.SetBool("isDamaged", true);
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(damageEffectDuration);
        animator.SetBool("isDamaged", false);
        spriteRenderer.color = originalColor;
    }
}
