using System.Collections;
using UnityEngine;
using UnityEngine.UI;

abstract public class Monster : MonoBehaviour
{
     [Header("HP Bar")]
    public Image hpBarImage;
    [SerializeField] private Transform hpBarTransform; // HpBar 오브젝트를 드래그

    public MonsterSO monsterData;               // ��ũ���ͺ� ������Ʈ ����
    protected Transform player;                   // �÷��̾�
    private float currentHealth;                // ���� ü��
    protected int windupTimer = 0;                // 준비 동작으로 부터 얼만큼 흘렀는지를 나타내는 타이머
    private float curSpeed;                        //instance monster speed*****
    private float maxSpeed;
    private float damageEffectDuration = 0.5f;  // isDamaged 유지 시간

    protected IMonsterPattern AttackPattern = null;

    [SerializeField] private int attackDamage = 1;
    [SerializeField] private int goldReward = 10; // ??몬스?��? 처치?�면 주는 골드
    [HideInInspector] public bool isClone = false; // 복제??몬스???��?
    public Vector3 fixedPosition = new Vector3(0f, 0f, 0f); // ?�본 몬스???�치 고정

    protected Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;            // 원래의 색상을 저장하는 용도의 변수

    [HideInInspector] public GameObject poolPrefabRef; // 풀 반환용 프리팹 참조
    [Header("피격 이펙트")]
    [SerializeField] private GameObject damageEffect; // 피격 이펙트

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();

        // 원래 색상 저장
        originalColor = spriteRenderer.color;

        // "Player" 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        //복제되지 않는 기본 몬스터는 so기준 초기화
        if (monsterData != null)
        {
            currentHealth = monsterData.maxHealth;
            attackDamage = monsterData.attackDamage;
            maxSpeed = monsterData.speed;
            curSpeed = maxSpeed;
            UpdateHpBar();
        }
        else
        {
            Debug.LogError("EnemySO ������ �Ҵ���� ����.");
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

    // ������
    private void Move()
    {
        if (player == null)
        {
            Debug.LogWarning("플레이어를 찾을 수 없음");
            return;
        }

        // TODO : ��Ʈ�� ���� �̵��ϵ��� ���� �ʿ�.
            // �÷��̾ ���� �̵�
            Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * curSpeed * Time.fixedDeltaTime;

        // ���Ͱ� �÷��̾ �ٶ󺸰� ȸ��
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
                AttackPattern?.AttackPattern(transform, player, animator, monsterData);
                windupTimer = 0; // 공격 후 타이머 초기화
            }
        }
        else
        {
            windupTimer = 0; // 범위 벗어나면 타이머 초기화
            animator.SetBool("isWindup", false);
            animator.SetBool("isAttack", false);
        }
    }

    // ���� ü�� ����
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Monster took {damage} damage, current HP: {currentHealth}");

        UpdateHpBar();      //체력바 수정

        // 피격 이펙트 표시
        GameObject eff = PoolManager.Instance.Get(damageEffect);
        eff.transform.position = transform.position;

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
        if (hpBarImage != null && monsterData != null)
            hpBarImage.fillAmount = currentHealth / (float)monsterData.maxHealth;
    }
    
    void LateUpdate()
    {
        if (hpBarTransform != null)
            hpBarTransform.LookAt(Camera.main.transform);
    }

    // ���� ���� (������Ʈ Ǯ�� ��ȯ)
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

    //clone 몬스터 스텟 초기화*****
    public void Initialize(int maxHp, int atk, float spd)
    {
        currentHealth = maxHp;
        attackDamage = atk;
        maxSpeed = spd;
        curSpeed = maxSpeed;
    }

    // �̵� �ӵ��� factor�� ���ؼ� �����ϴ� �޼���
    public void AdjustSpeed(float factor)
    {
        maxSpeed *= factor;
    }
    
    // �̵� �ӵ� ���� ����
    public void ResetSpeed()
    {
        maxSpeed = monsterData.speed;
    }

    // beatUpdate 이벤트 발생 시 마다 함수 실행
    private void OnBeat()
    {
        if (!isActiveAndEnabled) return;

        Attack();
        StartCoroutine(PulsateAnimation());
    }

    // 비트에 맞춰 애니메이션 재생하는 코루틴
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
