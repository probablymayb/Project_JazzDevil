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

    protected virtual void Start()
    {
        // "Player" 태그가 있는 오브젝트 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // 상시 발동 시 ActPattern 발동
        if (supporterData.attackCooldown == 0f)
        {
            ActPattern?.ActPattern(transform, player, supporterData);
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

        // 상시 발동이 아니면 타이머를 잰다.
        if (supporterData.attackCooldown != 0f)
        {
            timer += Time.deltaTime;
            // 만약 쿨 타임이 차면
            if (timer > supporterData.attackCooldown)
            {
                // 패턴
                ActPattern?.ActPattern(transform, player, supporterData);
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
