using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WaveMonsterConfiguration
{
    [Header("웨이브 정보")]
    public int waveNumber = 1;
    
    [Header("이 웨이브에서 등장할 몬스터들")]
    public List<GameObject> monsterPrefabs = new List<GameObject>();
    
    [Header("각 몬스터별 등장 확률 (0~100)")]
    [Tooltip("monsterPrefabs와 같은 순서로 확률을 설정하세요. 비워두면 동일한 확률로 설정됩니다.")]
    public List<int> spawnWeights = new List<int>();
}

public class MonsterSpawner : MonoBehaviour
{
    [Header("웨이브별 몬스터 설정")]
    [Tooltip("각 웨이브에서 등장할 몬스터 조합을 설정하세요")]
    public List<WaveMonsterConfiguration> waveConfigurations = new List<WaveMonsterConfiguration>();
    
    [Header("기본 몬스터 설정 (웨이브 설정이 없을 때 사용)")]
    [Tooltip("설정되지 않은 웨이브에서 사용할 기본 몬스터들")]
    public List<GameObject> defaultMonsterPrefabs = new List<GameObject>();

    [Header("스폰 거리 설정")]
    public float minDistance = 3f;   // 플레이어와 최소 거리
    public float maxDistance = 10f;  // 플레이어와 최대 거리

    [Header("스폰 속도 설정")]
    public float baseSpawnInterval = 1f;         // 기본 스폰 간격
    public float spawnIntervalReduction = 0.1f;  // 웨이브당 간격 감소
    public float minSpawnInterval = 0.3f;        // 최소 간격 보정값

    [Header("몬스터 능력치 설정")]
    public int baseHealth = 3;           // 기본 체력
    public int healthPerWave = 0;        // 웨이브당 체력 증가
    public int baseDamage = 1;           // 기본 공격력
    public int damagePerWave = 1;        // 웨이브당 공격력 증가
    public float fixedSpeed = 1.0f;      // 속도는 고정

    private Coroutine spawnCoroutine;    // 현재 웨이브 스폰 상태 저장용
    
    // 웨이브별 설정을 캐시하는 딕셔너리
    private Dictionary<int, WaveMonsterConfiguration> waveConfigCache;

    private void Start()
    {
        // 웨이브 설정 캐시 초기화
        InitializeWaveConfigCache();
        
        // 모든 몬스터 프리팹에 대해 오브젝트 풀 생성
        CreatePoolsForAllMonsters();
    }

    /// <summary>
    /// 웨이브 설정을 딕셔너리로 캐시
    /// </summary>
    private void InitializeWaveConfigCache()
    {
        waveConfigCache = new Dictionary<int, WaveMonsterConfiguration>();
        
        foreach (var config in waveConfigurations)
        {
            if (config.monsterPrefabs != null && config.monsterPrefabs.Count > 0)
            {
                waveConfigCache[config.waveNumber] = config;
            }
        }
        
        Debug.Log($"[MonsterSpawner] {waveConfigCache.Count}개의 웨이브 설정이 로드되었습니다.");
    }

    /// <summary>
    /// 모든 몬스터 프리팹에 대해 오브젝트 풀 생성
    /// </summary>
    private void CreatePoolsForAllMonsters()
    {
        HashSet<GameObject> allPrefabs = new HashSet<GameObject>();
        
        // 웨이브 설정에서 모든 프리팹 수집
        foreach (var config in waveConfigurations)
        {
            if (config.monsterPrefabs != null)
            {
                foreach (var prefab in config.monsterPrefabs)
                {
                    if (prefab != null)
                        allPrefabs.Add(prefab);
                }
            }
        }
        
        // 기본 몬스터 프리팹 추가
        if (defaultMonsterPrefabs != null)
        {
            foreach (var prefab in defaultMonsterPrefabs)
            {
                if (prefab != null)
                    allPrefabs.Add(prefab);
            }
        }
        
        // 오브젝트 풀 생성
        foreach (var prefab in allPrefabs)
        {
            PoolManager.Instance.CreatePool(prefab, 20);
        }
        
        Debug.Log($"[MonsterSpawner] {allPrefabs.Count}개의 몬스터 프리팹에 대한 풀이 생성되었습니다.");
    }

    /// <summary>
    /// WaveManager에서 호출: 일정 시간동안 몬스터를 계속 스폰
    /// </summary>
    public void SpawnWave(int waveIndex, float duration)
    {
        // 이전 웨이브가 진행 중이면 중단
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        // 새 웨이브 시작 (웨이브 인덱스를 1부터 시작하도록 조정)
        int waveNumber = waveIndex + 1;
        Debug.Log($"[MonsterSpawner] 웨이브 {waveNumber} 스폰 시작 (지속시간: {duration}초)");
        
        spawnCoroutine = StartCoroutine(SpawnMonstersOverTime(waveNumber, duration));
    }

    /// <summary>
    /// 주어진 시간동안 일정 간격으로 몬스터를 생성
    /// </summary>
    private IEnumerator SpawnMonstersOverTime(int waveNumber, float duration)
    {
        float elapsed = 0f;
        float spawnInterval = Mathf.Max(baseSpawnInterval - ((waveNumber - 1) * spawnIntervalReduction), minSpawnInterval);
        
        Debug.Log($"[MonsterSpawner] 웨이브 {waveNumber} 스폰 간격: {spawnInterval}초");

        while (elapsed < duration)
        {
            // 첫 번째 스폰은 즉시 실행
            if (elapsed == 0f)
            {
                SpawnMonster(waveNumber);
                elapsed += 0.1f; // 작은 값으로 초기화
            }
            else
            {
                SpawnMonster(waveNumber);
            }
            
            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }

        Debug.Log($"[MonsterSpawner] 웨이브 {waveNumber} 스폰 완료 (총 소요시간: {elapsed}초)");
        spawnCoroutine = null;
    }

    /// <summary>
    /// 실제 몬스터를 복제해서 커스터마이징 후 소환
    /// </summary>
    private void SpawnMonster(int waveNumber)
    {
        GameObject prefabToSpawn = SelectMonsterForWave(waveNumber);
        
        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"[MonsterSpawner] 웨이브 {waveNumber}에 스폰할 몬스터가 없습니다!");
            return;
        }

        // 플레이어 기준으로 스폰 위치 설정 (없을 경우 맵 중앙)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 basePosition = player != null ? player.transform.position : Vector3.zero;
        Vector3 spawnPosition = GetRandomSpawnPosition(basePosition);

        // 풀에서 몬스터 가져오기
        GameObject newMonster = PoolManager.Instance.Get(prefabToSpawn);
        if (newMonster == null)
        {
            Debug.LogError($"[MonsterSpawner] 풀에서 {prefabToSpawn.name} 몬스터를 가져올 수 없습니다!");
            return;
        }

        newMonster.transform.position = spawnPosition;
        newMonster.transform.rotation = Quaternion.identity;
        newMonster.layer = LayerMask.NameToLayer("Enemy");

        Debug.Log($"[MonsterSpawner] 웨이브 {waveNumber}에서 {prefabToSpawn.name} 소환 (위치: {spawnPosition})");

        // 클론으로 설정
        Monster monsterAI = newMonster.GetComponent<Monster>();

        if (monsterAI != null)
        {
            monsterAI.isClone = true;
            
            // 웨이브에 따라 능력치 계산
            int maxHp = baseHealth + ((waveNumber - 1) * healthPerWave);
            int atk = baseDamage + ((waveNumber - 1) * damagePerWave);
            float spd = fixedSpeed;

            // 능력치 초기화
            monsterAI.Initialize(maxHp, atk, spd);

            // 반환용 참조 설정
            monsterAI.poolPrefabRef = prefabToSpawn;
            
            Debug.Log($"[MonsterSpawner] 몬스터 능력치 - HP: {maxHp}, ATK: {atk}, SPD: {spd}");
        }
        else
        {
            Debug.LogError($"[MonsterSpawner] {newMonster.name}에서 Monster 컴포넌트를 찾을 수 없습니다!");
        }
    }

    /// <summary>
    /// 웨이브에 따라 몬스터를 선택 (가중치 고려)
    /// </summary>
    private GameObject SelectMonsterForWave(int waveNumber)
    {
        WaveMonsterConfiguration config = GetWaveConfiguration(waveNumber);
        
        if (config == null || config.monsterPrefabs == null || config.monsterPrefabs.Count == 0)
        {
            Debug.LogWarning($"[MonsterSpawner] 웨이브 {waveNumber}에 대한 설정이 없습니다. 기본 몬스터를 사용합니다.");
            return GetDefaultMonster();
        }

        // null 프리팹 필터링
        List<GameObject> validPrefabs = new List<GameObject>();
        List<int> validWeights = new List<int>();
        
        for (int i = 0; i < config.monsterPrefabs.Count; i++)
        {
            if (config.monsterPrefabs[i] != null)
            {
                validPrefabs.Add(config.monsterPrefabs[i]);
                
                // 가중치가 있다면 추가
                if (config.spawnWeights != null && i < config.spawnWeights.Count)
                {
                    validWeights.Add(config.spawnWeights[i]);
                }
            }
        }

        if (validPrefabs.Count == 0)
        {
            Debug.LogWarning($"[MonsterSpawner] 웨이브 {waveNumber}에 유효한 몬스터 프리팹이 없습니다!");
            return GetDefaultMonster();
        }

        // 가중치가 설정되어 있는지 확인
        if (validWeights.Count == validPrefabs.Count)
        {
            return SelectMonsterByWeight(validPrefabs, validWeights);
        }
        else
        {
            // 가중치가 없으면 랜덤 선택
            return validPrefabs[Random.Range(0, validPrefabs.Count)];
        }
    }

    /// <summary>
    /// 가중치에 따라 몬스터 선택
    /// </summary>
    private GameObject SelectMonsterByWeight(List<GameObject> monsters, List<int> weights)
    {
        int totalWeight = 0;
        foreach (int weight in weights)
        {
            totalWeight += weight;
        }

        if (totalWeight <= 0)
        {
            // 모든 가중치가 0이면 랜덤 선택
            return monsters[Random.Range(0, monsters.Count)];
        }

        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        for (int i = 0; i < monsters.Count; i++)
        {
            currentWeight += weights[i];
            if (randomValue < currentWeight)
            {
                return monsters[i];
            }
        }

        // 예외 상황 처리
        return monsters[monsters.Count - 1];
    }

    /// <summary>
    /// 웨이브 설정 가져오기
    /// </summary>
    private WaveMonsterConfiguration GetWaveConfiguration(int waveNumber)
    {
        if (waveConfigCache != null && waveConfigCache.ContainsKey(waveNumber))
        {
            return waveConfigCache[waveNumber];
        }
        return null;
    }

    /// <summary>
    /// 기본 몬스터 가져오기
    /// </summary>
    private GameObject GetDefaultMonster()
    {
        if (defaultMonsterPrefabs != null && defaultMonsterPrefabs.Count > 0)
        {
            // null이 아닌 첫 번째 프리팹 반환
            foreach (var prefab in defaultMonsterPrefabs)
            {
                if (prefab != null)
                    return prefab;
            }
        }
        
        Debug.LogError("[MonsterSpawner] 사용할 수 있는 기본 몬스터가 없습니다!");
        return null;
    }

    /// <summary>
    /// 플레이어 기준 랜덤한 위치 생성
    /// </summary>
    private Vector3 GetRandomSpawnPosition(Vector3 basePosition)
    {
        Vector3 spawnPosition;
        float distance;
        int attempts = 0;
        const int maxAttempts = 10;

        do
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            distance = Random.Range(minDistance, maxDistance);
            spawnPosition = basePosition + new Vector3(randomCircle.x * distance, 0, randomCircle.y * distance);
            attempts++;
            
            if (attempts > maxAttempts)
            {
                Debug.LogWarning("[MonsterSpawner] 적절한 스폰 위치를 찾을 수 없어 강제로 위치를 설정합니다.");
                break;
            }
        } while (Vector3.Distance(spawnPosition, basePosition) < minDistance);

        return spawnPosition;
    }

    /// <summary>
    /// 현재 웨이브에서 사용 가능한 몬스터 종류 확인 (디버그용)
    /// </summary>
    public void ShowCurrentWaveMonsters(int waveNumber)
    {
        WaveMonsterConfiguration config = GetWaveConfiguration(waveNumber);
        
        if (config != null && config.monsterPrefabs != null)
        {
            string monsterInfo = $"웨이브 {waveNumber} 몬스터: ";
            
            for (int i = 0; i < config.monsterPrefabs.Count; i++)
            {
                if (config.monsterPrefabs[i] != null)
                {
                    string weight = "";
                    if (config.spawnWeights != null && i < config.spawnWeights.Count)
                    {
                        weight = $"(가중치: {config.spawnWeights[i]})";
                    }
                    monsterInfo += $"{config.monsterPrefabs[i].name}{weight} ";
                }
            }
            
            Debug.Log(monsterInfo);
        }
        else
        {
            Debug.Log($"웨이브 {waveNumber}는 기본 몬스터를 사용합니다.");
        }
    }

    /// <summary>
    /// 현재 진행 중인 스폰 중단
    /// </summary>
    public void StopCurrentWave()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
            Debug.Log("[MonsterSpawner] 현재 웨이브 스폰이 중단되었습니다.");
        }
    }

    /// <summary>
    /// 인스펙터에서 웨이브 설정 검증
    /// </summary>
    private void OnValidate()
    {
        // 웨이브 번호 중복 체크
        HashSet<int> usedWaveNumbers = new HashSet<int>();
        
        foreach (var config in waveConfigurations)
        {
            if (usedWaveNumbers.Contains(config.waveNumber))
            {
                Debug.LogWarning($"웨이브 {config.waveNumber}이 중복 설정되어 있습니다!");
            }
            else
            {
                usedWaveNumbers.Add(config.waveNumber);
            }
            
            // 가중치와 몬스터 수가 맞는지 체크
            if (config.spawnWeights != null && config.spawnWeights.Count > 0 && 
                config.monsterPrefabs != null && config.spawnWeights.Count != config.monsterPrefabs.Count)
            {
                Debug.LogWarning($"웨이브 {config.waveNumber}: 몬스터 수({config.monsterPrefabs.Count})와 가중치 수({config.spawnWeights.Count})가 일치하지 않습니다!");
            }
        }
    }
}