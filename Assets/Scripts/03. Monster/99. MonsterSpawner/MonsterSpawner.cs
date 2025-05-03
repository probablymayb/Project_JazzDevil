using UnityEngine;
using System.Collections;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab; // 복제할 원본 몬스터 프리팹

    [Header("스폰 거리 설정")]
    public float minDistance = 3f;   // 플레이어와 최소 거리
    public float maxDistance = 10f;  // 플레이어와 최대 거리

    [Header("스폰 속도 설정")]
    public float baseSpawnInterval = 3f;         // 기본 스폰 간격
    public float spawnIntervalReduction = 0.3f;  // 웨이브당 간격 감소
    public float minSpawnInterval = 0.5f;        // 최소 간격 보정값

    [Header("몬스터 능력치 설정")]
    public int baseHealth = 3;           // 기본 체력
    public int healthPerWave = 2;        // 웨이브당 체력 증가
    public int baseDamage = 1;           // 기본 공격력
    public int damagePerWave = 1;        // 웨이브당 공격력 증가
    public float fixedSpeed = 1.0f;      // 속도는 고정

    private Coroutine spawnCoroutine;    // 현재 웨이브 스폰 상태 저장용

    private void Start()
    {
        PoolManager.Instance.CreatePool(monsterPrefab, 20);
    }

    /// WaveManager에서 호출: 일정 시간동안 몬스터를 계속 스폰
    public void SpawnWave(int waveIndex, float duration)
    {
        // 이전 웨이브가 진행 중이면 중단
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        // 새 웨이브 시작
        spawnCoroutine = StartCoroutine(SpawnMonstersOverTime(waveIndex, duration));
    }


    /// 주어진 시간동안 일정 간격으로 몬스터를 생성
    private IEnumerator SpawnMonstersOverTime(int waveIndex, float duration)
    {
        float elapsed = 0f;
        float spawnInterval = Mathf.Max(baseSpawnInterval - (waveIndex * spawnIntervalReduction), minSpawnInterval);

        while (elapsed < duration)
        {
            SpawnMonster(waveIndex);
            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }

        Debug.Log($"[Spawner] Wave {waveIndex + 1} spawning complete.");
    }

    /// 실제 몬스터를 복제해서 커스터마이징 후 소환
    private void SpawnMonster(int waveIndex)
    {
        if (monsterPrefab == null)
        {
            Debug.LogWarning("MonsterPrefab이 설정되지 않았습니다.");
            return;
        }

        // 플레이어 기준으로 스폰 위치 설정 (없을 경우 맵 중앙)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 basePosition = player != null ? player.transform.position : Vector3.zero;
        Vector3 spawnPosition = GetRandomSpawnPosition(basePosition);

        // 풀에서 몬스터 가져오기
        GameObject newMonster = PoolManager.Instance.Get(monsterPrefab);
        newMonster.transform.position = spawnPosition;
        newMonster.transform.rotation = Quaternion.identity;
        newMonster.layer = LayerMask.NameToLayer("Enemy");

        // MonsterPrefab을 기준으로 인스턴스 생성
        //GameObject newMonster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        //newMonster.layer = LayerMask.NameToLayer("Enemy"); // 레이어 명시적 설정
                                                           // 모든 자식 오브젝트에도 레이어 적용 (필요시)
        //foreach (Transform child in newMonster.transform)
        //{
        //    child.gameObject.layer = LayerMask.NameToLayer("Enemy");
        //}

        Debug.Log("몬스터 콜라이더 상태: " +
          "isTrigger=" + newMonster.GetComponentInChildren<Collider>().isTrigger +
          ", isEnabled=" + newMonster.GetComponentInChildren<Collider>().enabled);

        // 클론으로 설정

        Monster monsterAI = newMonster.GetComponent<Monster>();

        if (monsterAI != null)
        {
            monsterAI.isClone = true;
            // 웨이브에 따라 능력치 계산*******
            int maxHp = baseHealth + (waveIndex * healthPerWave);
            int atk = baseDamage + (waveIndex * damagePerWave);
            float spd = fixedSpeed;

            //능력치 초기화(Monseter.cs 내에서 처리)*****
            monsterAI.Initialize(maxHp, atk, spd);

            // 반환용 참조
            monsterAI.poolPrefabRef = monsterPrefab;
        }
    }

    /// 플레이어 기준 랜덤한 위치 생성
    private Vector3 GetRandomSpawnPosition(Vector3 basePosition)
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
