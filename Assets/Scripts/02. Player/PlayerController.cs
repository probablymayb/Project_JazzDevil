using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;
    private int currentHealth;

    [Header("Combat Settings")]
    [SerializeField] private GameObject shockwavePrefab = null;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private int attackDamage = 1;

    // 컴포넌트 참조
    private Rigidbody rb;
    private Animator spriteAnimator;
    private SphereCollider detectionCollider;

    // 입력 변수
    private float horizontalInput;
    private float verticalInput;

    // 공격 관련 변수
    private float nextAttackTime;
    private List<Monster> monstersInRange = new List<Monster>();

    private void Awake()
    {
        // 트리거 콜라이더 추가 (몬스터 감지용)
        detectionCollider = gameObject.AddComponent<SphereCollider>();
        detectionCollider.radius = attackRange;
        detectionCollider.isTrigger = true;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        spriteAnimator = transform.Find("Sprite")?.GetComponent<Animator>();
        currentHealth = maxHealth;

        // 회전 제한
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        ProcessInputs();
        UpdateAnimationState();

        // 공격 입력 처리
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextAttackTime)
        {
            AttackNearestMonster();
            nextAttackTime = Time.time + attackCooldown;
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
        // 여기에 사망 처리 추가
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Player Healed: " + currentHealth);
    }

    // 새로운 공격 로직: 가장 가까운 몬스터 공격
    private void AttackNearestMonster()
    {
        // 리스트에서 사라진 몬스터 제거
        for (int i = monstersInRange.Count - 1; i >= 0; i--)
        {
            if (monstersInRange[i] == null)
            {
                monstersInRange.RemoveAt(i);
            }
        }

        if (monstersInRange.Count == 0) return;

        // 가장 가까운 몬스터 찾기
        Monster nearestMonster = null;
        float nearestDistance = float.MaxValue;

        foreach (Monster monster in monstersInRange)
        {
            float distance = Vector3.SqrMagnitude(monster.transform.position - transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestMonster = monster;
            }
        }

        if (nearestMonster != null)
        {
            // 충격파 생성
            if (shockwavePrefab != null)
            {
                GameObject shockwave = Instantiate(shockwavePrefab, nearestMonster.transform.position, Quaternion.identity);

                // Shockwave 컴포넌트가 있다면 초기화
                Shockwave shockwaveComponent = shockwave.GetComponent<Shockwave>();
                if (shockwaveComponent != null)
                {
                    shockwaveComponent.Initialize(attackDamage);
                }
                else
                {
                    // Shockwave 컴포넌트가 없는 경우 직접 데미지 처리
                    nearestMonster.TakeDamage(attackDamage);
                }
            }
            else
            {
                // 충격파 프리팹이 없는 경우 직접 데미지 처리
                nearestMonster.TakeDamage(attackDamage);
            }
        }
    }

    // 트리거 범위에 들어온 몬스터 추가
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Monster monster = other.GetComponent<Monster>();
            if (monster != null && !monstersInRange.Contains(monster))
            {
                monstersInRange.Add(monster);
            }
        }
    }

    // 트리거 범위에서 나간 몬스터 제거
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Monster monster = other.GetComponent<Monster>();
            if (monster != null)
            {
                monstersInRange.Remove(monster);
            }
        }
    }

    // 공격 범위 표시 (디버깅용)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
