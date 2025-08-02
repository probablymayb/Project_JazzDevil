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

    [Header("Inventory")]
    [SerializeField] private int gold = 0;

    [Header("Rhythm System")]
    [SerializeField] private bool useRhythmSystem = true; // 리듬 시스템 사용 여부
    [SerializeField] private float rhythmDamageMultiplier = 2f; // 리듬 성공 시 데미지 배율

    //Animator
    private Rigidbody rb;
    private Animator upperBodyAnimator;
    private Animator lowerBodyAnimator;
    private SphereCollider detectionCollider;
    private int attackCounter = 0;

    // 입력 변수
    private float horizontalInput;
    private float verticalInput;

    // 공격 관련 변수
    private float nextAttackTime;
    private List<Monster> monstersInRange = new List<Monster>();

    // 외부(UI)에서 접근 가능한 변수
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int Gold => gold;
    public int killCount = 0;

    [Header("Note Timing Judge")]
    [SerializeField] private NoteJudge noteJudge;

    [Header("Audio")]
    [SerializeField] private EventReference rideOneShotSound;

    // 리듬 시스템 관련
    private SupporterManager supporterManager;

    private void Awake()
    {
        // 트리거 콜라이더 추가 (몬스터 감지용)
        detectionCollider = gameObject.AddComponent<SphereCollider>();
        detectionCollider.radius = attackRange;
        detectionCollider.isTrigger = true;

        // NoteJudge 참조 찾기
        if (noteJudge == null)
            noteJudge = FindFirstObjectByType<NoteJudge>();

        // SupporterManager 참조 찾기
        supporterManager = SupporterManager.Instance;
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

        // 리듬 시스템 이벤트 구독
        if (supporterManager != null)
        {
            supporterManager.OnRhythmJudged += OnRhythmJudged;
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (supporterManager != null)
        {
            supporterManager.OnRhythmJudged -= OnRhythmJudged;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != EGameState.Playing) return;

        ProcessInputs();
        UpdateAnimationState();

        // 공격 입력 처리
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextAttackTime)
        {
            if (useRhythmSystem)
            {
                // 리듬 시스템 사용 시 SupporterManager에서 처리
                // (SupporterManager.Update()에서 Space 키 입력을 이미 처리함)
                // 여기서는 쿨타임만 체크
                nextAttackTime = Time.time + attackCooldown;
            }
            else
            {
                // 기존 시스템 사용
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
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
        if (lowerBodyAnimator != null)
        {
            bool isMoving = Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f;
            lowerBodyAnimator.SetBool("isMove", isMoving);
        }
    }

    /// <summary>
    /// 리듬 판정 결과 처리
    /// </summary>
    private void OnRhythmJudged(JudgementResult result, Supporter supporter)
    {
        float damageMultiplier = GetRhythmDamageMultiplier(result);

        // 상체 공격 애니메이션 트리거
        if (upperBodyAnimator != null)
        {
            attackCounter++;
            upperBodyAnimator.SetBool("isAttack", true);
            upperBodyAnimator.SetInteger("AttackCounter", attackCounter % 2);
        }

        // 사운드 재생
        PlayAttackSound();

        // 데미지 계산 및 공격
        int finalDamage = Mathf.RoundToInt(attackDamage * damageMultiplier);
        AttackNearestMonster(finalDamage);

        // 동료 능력 발동 (리듬 판정 성공 시)
        if (supporter != null && result != JudgementResult.Miss)
        {
            float effectiveness = GetEffectivenessFromJudgement(result);
            supporter.ActivateRhythmAbility(effectiveness);
        }

        Debug.Log($"리듬 공격! 판정: {result}, 데미지 배율: {damageMultiplier:F1}x, 최종 데미지: {finalDamage}");
    }

    /// <summary>
    /// 판정에 따른 데미지 배율 반환
    /// </summary>
    private float GetRhythmDamageMultiplier(JudgementResult result)
    {
        switch (result)
        {
            case JudgementResult.Excellent:
                return rhythmDamageMultiplier; // 2.0x
            case JudgementResult.Solid:
                return rhythmDamageMultiplier * 0.8f; // 1.6x
            case JudgementResult.Good:
                return rhythmDamageMultiplier * 0.6f; // 1.2x
            case JudgementResult.Miss:
                return 0.3f; // 0.3x (페널티)
            default:
                return 1.0f;
        }
    }

    /// <summary>
    /// 판정에 따른 동료 능력 효과 반환
    /// </summary>
    private float GetEffectivenessFromJudgement(JudgementResult result)
    {
        switch (result)
        {
            case JudgementResult.Excellent: return 1.0f;
            case JudgementResult.Solid: return 0.8f;
            case JudgementResult.Good: return 0.6f;
            case JudgementResult.Miss: return 0.0f;
            default: return 0.5f;
        }
    }

    /// <summary>
    /// 기존 공격 시스템 (리듬 시스템 미사용 시)
    /// </summary>
    private void Attack()
    {
        if (upperBodyAnimator != null)
        {
            attackCounter++;
            upperBodyAnimator.SetBool("isAttack", true);
            upperBodyAnimator.SetInteger("AttackCounter", attackCounter % 2);
        }

        PlayAttackSound();

        // 노트 판정 (기존 시스템)
        JudgementResult judgement = JudgementResult.Miss;
        float damageMultiplier = 1.0f;

        if (noteJudge != null)
        {
            judgement = noteJudge.Judge();
            damageMultiplier = noteJudge.GetDamageMultiplier(judgement);
        }

        int finalDamage = Mathf.RoundToInt(attackDamage * damageMultiplier);
        AttackNearestMonster(finalDamage);
    }

    /// <summary>
    /// 공격 사운드 재생
    /// </summary>
    private void PlayAttackSound()
    {
        if (rideOneShotSound.IsNull)
        {
            Debug.LogWarning("rideOneShotSound 사운드 이벤트를 찾을 수 없음.");
        }
        else
        {
            AudioManager.Instance.PlayOneShot(rideOneShotSound, transform.position);
        }
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
                Shockwave shockwaveComponent = shockwave.GetComponent<Shockwave>();
                if (shockwaveComponent != null)
                {
                    shockwaveComponent.Initialize(damage);
                }
            }
            else
            {
                Debug.LogWarning("Shockwave 프리팹이 없습니다");
            }
        }
    }

    // 기존 메서드들은 그대로 유지
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

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"[Player] 골드 획득: +{amount} → 총 골드: {gold}");
    }

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

    public void OnMonsterKilled() { killCount++; }

    public void UpgradeDamage(int upgradedDamage)
    {
        attackDamage = upgradedDamage;
    }

    public void OnAttackStart()
    {
        Debug.Log("[PlayerController] Attack Start Event - 0% 지점");

        if (upperBodyAnimator != null)
        {
            AnimatorStateInfo stateInfo = upperBodyAnimator.GetCurrentAnimatorStateInfo(0);
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

    // 트리거 관련 메서드들
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Monster monster = other.GetComponentInParent<Monster>();
            if (monster != null && !monstersInRange.Contains(monster))
            {
                monstersInRange.Add(monster);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Monster monster = other.GetComponentInParent<Monster>();
            if (monster != null)
            {
                monstersInRange.Remove(monster);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
