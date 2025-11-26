using UnityEngine;

abstract public class Supporter : MonoBehaviour
{
    public SupporterSO supporterData;

    protected ISupporterPattern ActPattern = null;

    private Transform player;
    private float timer = 0f;   // 패턴 타이머

    // 플레이어 주위 공전 관련 변수
    private float orbitSpeed = 20f;   // 공전 속도
    private Vector3 offset;     // 플레이어와의 거리

    [HideInInspector] public GameObject poolPrefabRef; // 풀 반환용 프리팹 참조

    [Header("패턴 이펙트")]
    [SerializeField] private GameObject patternEffect; // 해당하는 패턴에 대한 프리팹

    // 런타임 스탯 캐시
    private SupporterManager.RuntimeStats runtimeStats;

    protected virtual void Start()
    {
        // "Player" 태그가 있는 오브젝트 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // 런타임 스탯 가져오기
        runtimeStats = SupporterManager.Instance.GetRuntimeStats(supporterData.supporterType);
        if (runtimeStats == null)
        {
            Debug.LogWarning($"[Supporter] {supporterData.supporterType} 런타임 스탯 없음, SO 원본 값으로 폴백");
            runtimeStats = new SupporterManager.RuntimeStats
            {
                attackCooldown = supporterData.attackCooldown,
                attackDamage = supporterData.attackDamage,
                attackRange = supporterData.attackRange
            };
        }

        // 상시 발동 시 ActPattern 발동
        if (runtimeStats.attackCooldown == 0f)
        {
            ActPattern?.ActPattern(transform, player, runtimeStats);
        }

        // 플레이어와의 offset 초기화
        offset = transform.position - player.position;
    }

    protected virtual void FixedUpdate()
    {
        // 가장 가까운 적 바라보기
        Transform lookTarget = Finder.NearestObject(transform, "Monster");
        if (lookTarget != null)
        {
            Vector3 direction = lookTarget.position - transform.position; // 타겟까지의 방향 벡터
            direction.y = 0f; // pitch 회전 방지
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        // 런타임 스탯 동기화 (업그레이드 반영)
        runtimeStats = SupporterManager.Instance.GetRuntimeStats(supporterData.supporterType);
        if (runtimeStats == null) return;

        // 상시 발동이 아니면 타이머를 잰다.
        if (runtimeStats.attackCooldown != 0f)
        {
            timer += Time.deltaTime;
            // 만약 쿨 타임이 차면
            if (timer > runtimeStats.attackCooldown)
            {
                // 패턴
                ActPattern?.ActPattern(transform, player, runtimeStats);
                if (patternEffect != null)
                {
                    GameObject eff = PoolManager.Instance.Get(patternEffect);
                    eff.transform.position = transform.position;
                }
                timer = 0f; // 타이머 초기화
            }
        }
    }
}
