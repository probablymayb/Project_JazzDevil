using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;
    private int currentHealth;

    private Rigidbody rb;
    private Animator spriteAnimator;
    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        spriteAnimator = transform.Find("Sprite")?.GetComponent<Animator>();

        currentHealth = maxHealth;

        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        ProcessInputs();
        UpdateAnimationState();

        // 스페이스바 입력으로 몬스터 공격
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttackMonsters();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void ProcessInputs()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void Move()
    {
        float currentYVelocity = rb.linearVelocity.y;

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        rb.linearVelocity = new Vector3(
            moveDirection.x * moveSpeed,
            currentYVelocity,
            moveDirection.z * moveSpeed
        );
    }

    private void UpdateAnimationState()
    {
        if (spriteAnimator != null)
        {
            bool isMoving = Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f;
            spriteAnimator.SetBool("isMove", isMoving);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player is Dead!");
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Player Healed: " + currentHealth);
    }

    // 몬스터 공격 로직 (거리 2 이내)
    private void AttackMonsters()
    {
        float attackRange = 2f;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);

        bool hitMonster = false;

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Monster"))
            {
                MonsterAI monster = hitCollider.GetComponent<MonsterAI>();
                if (monster != null)
                {
                    monster.TakeDamage(1); // 몬스터 체력 1 감소
                    hitMonster = true;
                }
            }
        }
    }

    // 공격 범위 표시 (디버깅용)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2f); // 공격 범위 표시
    }
}
