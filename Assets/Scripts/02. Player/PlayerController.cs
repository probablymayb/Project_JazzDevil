using UnityEngine;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;


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

    //골드 관련 변수
    [Header("Inventory")]
    [SerializeField] private int gold = 0;

    //Animator
    private Rigidbody rb;
    private Animator upperBodyAnimator;  // 상체 애니메이터
    private Animator lowerBodyAnimator;  // 하체 애니메이터
    private SphereCollider detectionCollider;
    private int attackCounter = 0;

    // 입력 변수
    private float horizontalInput;
    private float verticalInput;

    // 공격 관련 변수
    private float nextAttackTime;
    private List<Monster> monstersInRange = new List<Monster>();

    // 외부(UI)에서 접근 가능한 변수*****
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int Gold => gold;
    public int killCount = 0;


    [Header("Note Timing Judge")]

    //Note Timing 판단
    [SerializeField] private NoteJudge noteJudge;


    //Ride One sHot Audio
    [SerializeField]
    private EventReference rideOneShotSound;

    private void Awake()
    {
        // 트리거 콜라이더 추가 (몬스터 감지용)
        detectionCollider = gameObject.AddComponent<SphereCollider>();
        detectionCollider.radius = attackRange;
        detectionCollider.isTrigger = true;

        // NoteJudge 참조 찾기
        if (noteJudge == null)
            noteJudge = FindFirstObjectByType<NoteJudge>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 상체와 하체 애니메이터 각각 찾기
        Transform upperBodyTransform = transform.Find("UpperBody");
        Transform lowerBodyTransform = transform.Find("LowerBody");

        if (upperBodyTransform != null)
            upperBodyAnimator = upperBodyTransform.GetComponent<Animator>();
        else
            Debug.LogWarning("[PlayerController] UpperBody를 찾을 수 없습니다!");

        if (lowerBodyTransform != null)
            lowerBodyAnimator = lowerBodyTransform.GetComponent<Animator>();
        else
            Debug.LogWarning("[PlayerController] LowerBody를 찾을 수 없습니다!");

        currentHealth = maxHealth;

        // 회전 제한
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        // 게임 상태가 Playing이 아니면 Update 수행하지 않음
        if (GameManager.Instance.CurrentGameState != EGameState.Playing) return;

        ProcessInputs();
        UpdateAnimationState();

        // 공격 입력 처리
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextAttackTime)
        {
            Attack();
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
        // 하체 애니메이션만 이동 상태에 따라 업데이트
        if (lowerBodyAnimator != null)
        {
            bool isMoving = Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f;
            lowerBodyAnimator.SetBool("isMove", isMoving);
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

    //골드 추가 ******
    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"[Player] 골드 획득: +{amount} → 총 골드: {gold}");
    }

    //골드 소모 ****** (상점 기능에 따라 변경 예정)
    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            Debug.Log($"[Player] 골드 사용: -{amount} → 남은 골드: {gold}");
            return true;
        }
        else
        {
            Debug.LogWarning("[Player] 골드 부족! 구매 실패");
            return false;
        }
    }

    private void Attack()
    {
        // 상체 공격 애니메이션 트리거
        if (upperBodyAnimator != null)
        {
            attackCounter++;
            Debug.Log($"🎯 attackCounter % 2 = {attackCounter % 2}");
            Debug.Log($"🎯 SetInteger AttackCounter = {attackCounter % 2}");

            upperBodyAnimator.SetBool("isAttack", true);
            upperBodyAnimator.SetInteger("AttackCounter", attackCounter % 2);

            // 🔍 실제 Animator Parameter 값 확인
            int currentAttackCounter = upperBodyAnimator.GetInteger("AttackCounter");
            Debug.Log($"🎯 [ANIMATOR] 실제 AttackCounter 값 = {currentAttackCounter}");

           
        }

        //Ride One sHot Audio

        if (rideOneShotSound.IsNull)
        {
            Debug.LogWarning("rideOneShotSound 사운드 이벤트를 찾을 수 없음.");
        }
        else
        {
            AudioManager.Instance.PlayOneShot(rideOneShotSound, transform.position);
        }
        

        // 노트 판정 요청
        JudgementResult judgement = JudgementResult.Miss;
        float damageMultiplier = 1.0f;

        if (noteJudge != null)
        {
            judgement = noteJudge.Judge();
            damageMultiplier = noteJudge.GetDamageMultiplier(judgement);
        }

        // 판정에 따른 데미지 계산
        int finalDamage = Mathf.RoundToInt(attackDamage * damageMultiplier);

        // 가장 가까운 몬스터 공격
        AttackNearestMonster(finalDamage);
    }

    private void AttackNearestMonster(int damage)
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
                    shockwaveComponent.Initialize(damage);
                }
                else
                {
                    // Shockwave 컴포넌트가 없는 경우 직접 데미지 처리
                    nearestMonster.TakeDamage(damage);
                }
            }
            else
            {
                Debug.LogWarning("Shockwave 프리팹이 없습니다");
            }
        }
    }
    
    public void OnMonsterKilled() { killCount++; }

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

    // 데미지 업그레이드
    public void UpgradeDamage(int upgradedDamage)
    {
        attackDamage = upgradedDamage;
    }

    //UpperBody Animation
    public void OnAttackStart()
    {
        Debug.Log("[PlayerController] Attack Start Event - 0% 지점");

        // 역방향 완료 시 isAttack을 false로 설정
        if (upperBodyAnimator != null)
        {
            AnimatorStateInfo stateInfo = upperBodyAnimator.GetCurrentAnimatorStateInfo(0);
            // 현재 상태의 속도가 음수면 역방향
            Debug.Log(stateInfo.GetType());
            if (stateInfo.speed < 0 || stateInfo.speedMultiplier < 0)
            {
                upperBodyAnimator.SetBool("isAttack", false);
                Debug.Log("[PlayerController] 역방향 공격 완료!");
            }
            else
            {
                Debug.Log("[PlayerController] 정방향 공격 시작!");

            }
        }
    }
}
