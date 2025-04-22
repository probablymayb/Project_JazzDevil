using System.Collections;
using UnityEngine;

abstract public class Monster : MonoBehaviour
{

    public MonsterSO monsterData;   // ��ũ���ͺ� ������Ʈ ����
    private Transform player;       // �÷��̾�
    private float currentHealth;    // ���� ü��
    private int windupTimer = 0;    // 준비 동작으로 부터 얼만큼 흘렀는지를 나타내는 타이머
    private float speed;            //instance monster speed*****

    protected IMonsterPattern AttackPattern = null;

    [SerializeField] private int attackDamage = 1;
    [SerializeField] private int goldReward = 10; // ??몬스?��? 처치?�면 주는 골드
    [HideInInspector] public bool isClone = false; // 복제??몬스???��?
    public Vector3 fixedPosition = new Vector3(0f, 0f, 0f); // ?�본 몬스???�치 고정

    private Animator animator;

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();

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
            speed = monsterData.speed;
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
        // TODO : ��Ʈ�� ���� �̵��ϵ��� ���� �ʿ�.
        // �÷��̾ ���� �̵�
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // ���Ͱ� �÷��̾ �ٶ󺸰� ȸ��
        transform.LookAt(player);
    }

    // 공격 로직 판단 + 수행 함수
    private void Attack()
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
                AttackPattern?.AttackPattern(player, animator, monsterData);
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

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ���� ���� (������Ʈ Ǯ�� ��ȯ)
    private void Die()
    {
        Debug.Log("Monster is Dead!");
        // TODO : ������Ʈ Ǯ�� �߰� �� �Ʒ��� ���� Ǯ�� ��ȯ�ϴ� �ڵ� �ۼ� �ؾ� ��.
        // EnemyPool.Instance.ReturnToPool(this);


        // ?�레?�어?�게 골드 지�?********
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerController pc = playerObj.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.AddGold(goldReward);
            }
        }

        Destroy(gameObject); // ??��???�론�?가??(?�본?� ?�직이지 ?�아??공격받�? ?�음)
    }

    //clone 몬스터 스텟 초기화*****
    public void Initialize(int maxHp, int atk, float spd)
    {
        currentHealth = maxHp;
        attackDamage = atk;
        speed = spd;
    }

    // �̵� �ӵ��� factor�� ���ؼ� �����ϴ� �޼���
    public void AdjustSpeed(float factor)
    {
        speed *= factor;
    }
    
    // �̵� �ӵ� ���� ����
    public void ResetSpeed()
    {
        speed = monsterData.speed;
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
        float startSpeed = 2f;
        float timer = 0.1f;
        float duration = 60f / RhythmManager.Instance.CurrentBpm;

        if (animator == null) yield break;

        animator.speed = startSpeed;

        while (timer < duration)
        {
            if (this == null || animator == null) yield break;

            timer += Time.deltaTime;
            animator.speed = Mathf.Lerp(startSpeed, 0.1f, timer / duration);
            yield return null;
        }

        if (animator != null)
            animator.speed = 0.1f;
    }

}
