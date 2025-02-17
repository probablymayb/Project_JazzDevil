using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    public float speed = 3.0f; // 몬스터 이동 속도
    private Transform player; // 플레이어의 Transform

    void Start()
    {
        // "Player" 태그가 있는 오브젝트 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player != null)
        {
            // 플레이어를 향해 이동
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // 몬스터가 플레이어를 바라보게 회전
            transform.LookAt(player);
        }
    }
}
