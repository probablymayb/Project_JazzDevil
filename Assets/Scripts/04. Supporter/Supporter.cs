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
        // 플레이어 주위를 공전하기
        OrbitPlayer();

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
                timer = 0f; // 타이머 초기화
            }
        }
    }

    // 플레이어 주위 공전
    private void OrbitPlayer()
    {
        transform.position = player.position + offset;
        transform.RotateAround(player.position, Vector3.up, orbitSpeed * Time.deltaTime);
        offset = transform.position - player.position;
    }
}
