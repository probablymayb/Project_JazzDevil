using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab; // 몬스터 프리팹 (원본)
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
        if (monsterPrefab == null)
        {
            Debug.LogWarning("MonsterPrefab이 설정되지 않았습니다. MonsterSpawner에 Prefab을 할당하세요.");
            return;
        }

        // 플레이어 기준으로 스폰 위치 설정 (없을 경우 맵 중앙)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 basePosition = player != null ? player.transform.position : Vector3.zero;

        Vector3 spawnPosition = GetRandomSpawnPosition(basePosition);

        // MonsterPrefab을 기준으로 인스턴스 생성
        GameObject newMonster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);

        // 클론으로 설정
        MonsterAI monsterAI = newMonster.GetComponent<MonsterAI>();
        if (monsterAI != null)
        {
            monsterAI.isClone = true; // 복제된 몬스터로 설정
            monsterAI.speed = 1.0f;   // 복제 몬스터 속도 활성화
        }

    }

    Vector3 GetRandomSpawnPosition(Vector3 basePosition)
    {
        Vector3 spawnPosition;
        float distance;

        do
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            distance = Random.Range(minDistance, maxDistance);
            spawnPosition = basePosition + new Vector3(randomCircle.x * distance, 0, randomCircle.y * distance);
        } while (Vector3.Distance(spawnPosition, basePosition) < minDistance);

        return spawnPosition;
    }
}
