using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab; // 몬스터 프리팹
    public float minDistance = 3f;   // 몬스터 최소 스폰 거리
    public float maxDistance = 10f;  // 몬스터 최대 스폰 거리

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) // M 키를 누르면 소환
        {
            SpawnMonster();
        }
    }

    void SpawnMonster()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); // 플레이어 찾기

        if (player != null && monsterPrefab != null)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition(player.transform.position);
            Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Player 또는 MonsterPrefab이 설정되지 않았습니다!");
        }
    }

    Vector3 GetRandomSpawnPosition(Vector3 playerPosition)
    {
        Vector3 spawnPosition;
        float distance;

        do
        {
            // 랜덤 방향 설정 (XY 평면이 아닌, XZ 평면에서)
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            distance = Random.Range(minDistance, maxDistance); // 최소~최대 거리 사이에서 랜덤 선택
            spawnPosition = playerPosition + new Vector3(randomCircle.x * distance, 0, randomCircle.y * distance);
        } while (Vector3.Distance(spawnPosition, playerPosition) < minDistance); // 최소 거리보다 가까우면 다시 생성

        return spawnPosition;
    }
}
