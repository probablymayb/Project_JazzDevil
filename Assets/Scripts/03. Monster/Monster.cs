using UnityEngine;

abstract public class Monster : MonoBehaviour
{
    public MonsterSO monsterData;   // ��ũ���ͺ� ������Ʈ ����
    private Transform player;       // �÷��̾�
    private float currentHealth;    // ���� ü��
    private float attackTimer = 0f;      // ���� Ÿ�̸�
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

    protected virtual void FixedUpdate()
    {
        Move();
        Attack();
    }

    // ������
    private void Move()
    {
        // TODO : ��Ʈ�� ���� �̵��ϵ��� ���� �ʿ�.
        // �÷��̾ ���� �̵�
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * monsterData.speed * Time.deltaTime;

        // ���Ͱ� �÷��̾ �ٶ󺸰� ȸ��
        transform.LookAt(player);
    }

    // ���� ����
    private void Attack()
    {
        // �Ÿ� Ȯ��
        float distance = Vector3.Distance(transform.position, player.position);
        
        // TODO : �� ������ �ƴ϶� ��Ʈ ������ ���� �ʿ�
        // Windup�ʸ��� �÷��̾�� ������
        if (distance <= monsterData.attackRange)
        {
            animator.SetBool("isWindup", true); // 공격 준비 모션

            attackTimer += Time.deltaTime;

            if (attackTimer >= monsterData.attackWindup)
            {
                AttackPattern?.AttackPattern(player, animator, monsterData);
                attackTimer = 0f; // Ÿ�̸� �ʱ�ȭ
            }
        }
        else
        {
            attackTimer = 0f; // �Ÿ��� �־����� Ÿ�̸� �ʱ�ȭ
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

    public void Initialize(int maxHp, int atk, float spd)
    {
        currentHealth = maxHp;
        attackDamage = atk;
        speed = spd;
    }
}
