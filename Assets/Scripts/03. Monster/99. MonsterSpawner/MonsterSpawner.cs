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
    [Header("매니저 참조")]
    [SerializeField] private WaveManager waveManager;

    [Header("웨이브별 몬스터 설정")]
    public List<WaveMonsterConfiguration> waveConfigurations = new List<WaveMonsterConfiguration>();
    
    [Header("기본 몬스터 설정")]
    public List<GameObject> defaultMonsterPrefabs = new List<GameObject>();

    [Header("스폰 거리 설정")]
    public float minDistance = 3f;
    public float maxDistance = 10f;

    [Header("스폰 속도 설정")]
    public float baseSpawnInterval = 1f;
    public float spawnIntervalReduction = 0.1f;
    public float minSpawnInterval = 0.3f;

    [Header("플레이 영역 설정")]
    [SerializeField] private GameObject[] wallObjects; // ✅ 4개 벽 할당
    [SerializeField] private float wallPadding = 2f;   // 벽으로부터 여유 거리
    [SerializeField] private float spawnHeight = 0.2f;

    // 계산된 플레이 영역
    private Vector2 playAreaMin;
    private Vector2 playAreaMax;
    private bool playAreaCalculated = false;

    private Coroutine spawnCoroutine;
    private Dictionary<int, WaveMonsterConfiguration> waveConfigCache;
    private HashSet<MonsterSO> cachedMonsterSOs = new HashSet<MonsterSO>();
    private HashSet<BulletSO> cachedBulletSOs = new HashSet<BulletSO>();

    private void Start()
    {
        if (waveManager == null)
        {
            waveManager = FindFirstObjectByType<WaveManager>();
            if (waveManager == null)
                Debug.LogError("[MonsterSpawner] WaveManager를 찾을 수 없습니다!");
        }

        CalculatePlayAreaFromWalls(); // ✅ 플레이 영역 계산
        InitializeWaveConfigCache();
        CreatePoolsForAllMonsters();
        CacheAllMonsterSOs();
        CacheAllBulletSOs();
    }

    private void OnDestroy()
    {
        foreach (var so in cachedMonsterSOs)
        {
            so.ResetToBase();
        }
        
        foreach (var bulletSO in cachedBulletSOs)
        {
            bulletSO.ResetToBase();
        }
    }

    /// <summary>
    /// ✅ 벽 오브젝트로부터 플레이 영역 자동 계산
    /// </summary>
    private void CalculatePlayAreaFromWalls()
    {
        if (wallObjects == null || wallObjects.Length == 0)
        {
            Debug.LogWarning("[MonsterSpawner] wallObjects가 할당되지 않음. 기본 영역 사용.");
            playAreaMin = new Vector2(-20f, -20f);
            playAreaMax = new Vector2(20f, 20f);
            playAreaCalculated = true;
            return;
        }

        float minX = float.MaxValue, maxX = float.MinValue;
        float minZ = float.MaxValue, maxZ = float.MinValue;

        foreach (var wall in wallObjects)
        {
            if (wall == null) continue;

            BoxCollider boxCol = wall.GetComponent<BoxCollider>();
            if (boxCol != null)
            {
                Bounds bounds = boxCol.bounds;
                
                minX = Mathf.Min(minX, bounds.min.x);
                maxX = Mathf.Max(maxX, bounds.max.x);
                minZ = Mathf.Min(minZ, bounds.min.z);
                maxZ = Mathf.Max(maxZ, bounds.max.z);
            }
            else
            {
                Renderer renderer = wall.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Bounds bounds = renderer.bounds;
                    
                    minX = Mathf.Min(minX, bounds.min.x);
                    maxX = Mathf.Max(maxX, bounds.max.x);
                    minZ = Mathf.Min(minZ, bounds.min.z);
                    maxZ = Mathf.Max(maxZ, bounds.max.z);
                }
            }
        }

        playAreaMin = new Vector2(minX, minZ);
        playAreaMax = new Vector2(maxX, maxZ);
        playAreaCalculated = true;

        Debug.Log($"[MonsterSpawner] 플레이 영역 계산 완료: Min({playAreaMin.x:F1}, {playAreaMin.y:F1}), Max({playAreaMax.x:F1}, {playAreaMax.y:F1})");
    }

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

    private void CacheAllMonsterSOs()
    {
        cachedMonsterSOs.Clear();
        HashSet<GameObject> allPrefabs = new HashSet<GameObject>();
        
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
        
        if (defaultMonsterPrefabs != null)
        {
            foreach (var prefab in defaultMonsterPrefabs)
            {
                if (prefab != null)
                    allPrefabs.Add(prefab);
            }
        }

        foreach (var prefab in allPrefabs)
        {
            Monster m = prefab.GetComponent<Monster>();
            if (m != null && m.monsterData != null)
            {
                cachedMonsterSOs.Add(m.monsterData);
                m.monsterData.CacheBase();
            }
        }
        
        Debug.Log($"[MonsterSpawner] {cachedMonsterSOs.Count}개 MonsterSO 캐시 완료");
    }

    /// <summary>
    /// 모든 BulletSO 캐시 (Resources 폴더에서 자동 검색)
    /// </summary>
    private void CacheAllBulletSOs()
    {
        cachedBulletSOs.Clear();
        
        // Resources 폴더에서 모든 BulletSO 로드
        BulletSO[] allBulletSOs = Resources.LoadAll<BulletSO>("");
        
        foreach (var bulletSO in allBulletSOs)
        {
            if (bulletSO != null)
            {
                cachedBulletSOs.Add(bulletSO);
                bulletSO.CacheBase();
            }
        }
        
        Debug.Log($"[MonsterSpawner] {cachedBulletSOs.Count}개 BulletSO 캐시 완료");
    }

    /// <summary>
    /// WaveManager에서 호출: 일정 시간동안 몬스터를 계속 스폰
    /// </summary>
    public void SpawnWave(float duration)
    {
        // 이전 웨이브가 진행 중이면 중단
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        if (!playAreaCalculated)
        {
            CalculatePlayAreaFromWalls();
        }

        int waveNumber = waveManager != null ? waveManager.currentWave : 1;
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
            SpawnMonster(waveNumber);
            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }

        Debug.Log($"[MonsterSpawner] 웨이브 {waveNumber} 스폰 완료");
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
            Debug.LogWarning($"[MonsterSpawner] 웨이브 {waveNumber} 스폰할 몬스터 없음");
            return;
        }

        // 플레이어 기준으로 스폰 위치 설정 (없을 경우 맵 중앙)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 basePosition = player != null 
            ? new Vector3(player.transform.position.x, spawnHeight, player.transform.position.z) 
            : Vector3.zero;
        
        // ✅ 플레이 영역 내에서 랜덤 위치 생성
        Vector3 spawnPosition = GetRandomSpawnPositionInBounds(basePosition);

        GameObject newMonster = PoolManager.Instance.Get(prefabToSpawn);
        if (newMonster == null)
        {
            Debug.LogError($"[MonsterSpawner] 풀 반환 실패: {prefabToSpawn.name}");
            return;
        }

        newMonster.transform.position = spawnPosition;
        newMonster.transform.rotation = Quaternion.identity;
        newMonster.layer = LayerMask.NameToLayer("Enemy");

        Monster monsterAI = newMonster.GetComponent<Monster>();
        if (monsterAI != null && monsterAI.monsterData != null)
        {
            // ✅ MonsterSO에서 웨이브별 스탯 가져오기
            int hp, atk;
            float spd;
            monsterAI.monsterData.GetStatsForWave(waveNumber, out hp, out atk, out spd);
            
            monsterAI.Initialize(hp, atk, spd);
            monsterAI.poolPrefabRef = prefabToSpawn;
            
            Debug.Log($"[MonsterSpawner] Wave {waveNumber} {prefabToSpawn.name} at {spawnPosition} | HP:{hp}, ATK:{atk}, SPD:{spd:F2}");
        }
        else
        {
            Debug.LogError($"[MonsterSpawner] Monster 또는 MonsterSO 없음: {newMonster.name}");
        }
    }

    /// <summary>
    /// ✅ 플레이 영역 내에서 랜덤 위치 생성
    /// </summary>
    private Vector3 GetRandomSpawnPositionInBounds(Vector3 basePosition)
    {
        int maxAttempts = 20;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // 랜덤 방향과 거리
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            float distance = Random.Range(minDistance, maxDistance);
            
            Vector3 candidatePosition = basePosition + new Vector3(
                randomCircle.x * distance, 
                0, 
                randomCircle.y * distance
            );

            // ✅ 플레이 영역 내부이고 최소 거리 만족하는지 확인
            if (IsPositionInPlayArea(candidatePosition) && 
                Vector3.Distance(new Vector3(candidatePosition.x, 0, candidatePosition.z), 
                                new Vector3(basePosition.x, 0, basePosition.z)) >= minDistance)
            {
                candidatePosition.y = spawnHeight;
                return candidatePosition;
            }
        }

        // ✅ 실패 시 플레이어 근처 안전 위치 반환
        Vector3 safePosition = ClampToPlayArea(basePosition);
        safePosition.y = spawnHeight;
        
        Debug.LogWarning($"[MonsterSpawner] {maxAttempts}번 시도 후 유효한 위치를 찾지 못함. 안전 위치 사용: {safePosition}");
        return safePosition;
    }

    /// <summary>
    /// ✅ 위치가 플레이 영역 내부인지 확인
    /// </summary>
    private bool IsPositionInPlayArea(Vector3 position)
    {
        float x = position.x;
        float z = position.z;

        return x >= playAreaMin.x + wallPadding && 
               x <= playAreaMax.x - wallPadding &&
               z >= playAreaMin.y + wallPadding && 
               z <= playAreaMax.y - wallPadding;
    }

    /// <summary>
    /// ✅ 위치를 플레이 영역 내부로 제한
    /// </summary>
    private Vector3 ClampToPlayArea(Vector3 position)
    {
        float x = Mathf.Clamp(position.x, playAreaMin.x + wallPadding, playAreaMax.x - wallPadding);
        float z = Mathf.Clamp(position.z, playAreaMin.y + wallPadding, playAreaMax.y - wallPadding);
        
        return new Vector3(x, position.y, z);
    }

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
    /// ✅ 씬 뷰에서 플레이 영역 시각화
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!playAreaCalculated && Application.isPlaying)
            return;

        if (!Application.isPlaying)
        {
            CalculatePlayAreaFromWalls();
        }

        // 전체 플레이 영역 (녹색)
        Gizmos.color = Color.green;
        Vector3 center = new Vector3(
            (playAreaMin.x + playAreaMax.x) / 2f,
            0.1f,
            (playAreaMin.y + playAreaMax.y) / 2f
        );
        Vector3 size = new Vector3(
            playAreaMax.x - playAreaMin.x,
            0.1f,
            playAreaMax.y - playAreaMin.y
        );
        Gizmos.DrawWireCube(center, size);

        // 안전 영역 (패딩 적용, 노란색)
        Gizmos.color = Color.yellow;
        Vector3 safeCenter = new Vector3(
            (playAreaMin.x + wallPadding + playAreaMax.x - wallPadding) / 2f,
            0.1f,
            (playAreaMin.y + wallPadding + playAreaMax.y - wallPadding) / 2f
        );
        Vector3 safeSize = new Vector3(
            playAreaMax.x - playAreaMin.x - wallPadding * 2,
            0.1f,
            playAreaMax.y - playAreaMin.y - wallPadding * 2
        );
        Gizmos.DrawWireCube(safeCenter, safeSize);

        // 스폰 범위 (플레이어 기준, 빨간색)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawWireSphere(player.transform.position, minDistance);
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawWireSphere(player.transform.position, maxDistance);
        }
    }

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