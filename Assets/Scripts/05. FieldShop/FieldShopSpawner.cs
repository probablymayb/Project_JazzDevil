using UnityEngine;

public class FieldShopSpawner : MonoBehaviour
{
    [SerializeField] private GameObject fieldShop;  // 필드 상점 프리팹을 인스펙터에서 연결

    private Vector3 basePosition;       // 기준이 될 위치
    private Vector3 spawnPosition;      // 스폰할 위치
    private float distance;             // 거리
    private float minDistance = 3f;     // 플레이어로 부터 최소 거리
    private float maxDistance = 10f;    // 플레이어로 부터 최대 거리

    private GameObject player;          // basePosition을 플레이어를 기준으로 함

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Spawn()
    {
        GameObject spawned = PoolManager.Instance.Get(fieldShop);
        spawned.SetActive(false); // 위치 조정까지 잠시 비활성화
        basePosition = player != null ? player.transform.position : Vector3.zero;

        // 위치 랜덤 생성 후 적용
        do
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            distance = Random.Range(minDistance, maxDistance);
            spawnPosition = basePosition + new Vector3(randomCircle.x * distance, 0, randomCircle.y * distance);
        } while (Vector3.Distance(spawnPosition, basePosition) < minDistance);
        spawned.transform.position = spawnPosition;
        spawned.SetActive(true); // 조정 완료했으니 활성화
    }
}
