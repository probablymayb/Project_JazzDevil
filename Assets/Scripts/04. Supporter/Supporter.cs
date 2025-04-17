using UnityEngine;

abstract public class Supporter : MonoBehaviour
{
    public SupporterSO supporterData;

    protected ISupporterPattern ActPattern = null;

    private Transform player;
    private float followSpeed;  // 플레이어 따르는 속도
    private float timer = 0f;   // 패턴 타이머

    protected virtual void Start()
    {
        // "Player" 태그가 있는 오브젝트 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        followSpeed = 5f;

        // 상시 발동 시 ActPattern 발동
        if (supporterData.attackCooldown == 0f)
        {
            ActPattern?.ActPattern(transform, player, supporterData);
        }
    }

    protected virtual void FixedUpdate()
    {
        // 플레이어 따라다니기
        transform.position = Vector3.Lerp(transform.position, player.position, followSpeed * Time.deltaTime);

        // 가장 가까운 적 바라보기
        Transform lookTarget = Finder.NearestObject(transform, "Monster");
        if (lookTarget != null)
        {
            transform.LookAt(lookTarget);
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
}
