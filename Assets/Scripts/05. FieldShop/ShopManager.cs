using UnityEngine;

/// <summary>
/// wave 종료시 트리거 생성
/// </summary>
public class ShopManager : MonoBehaviour
{
    public GameObject shopTriggerPrefab;
    public GameObject arrowIndicatorPrefab;
    public Canvas hudCanvas;

    [Header("플레이 영역 설정")]
    [SerializeField] private GameObject[] wallObjects; // ✅ 4개 벽 할당
    [SerializeField] private float wallPadding = 2f;   // 벽으로부터 여유 거리
    [SerializeField] private float spawnHeight = 0.5f;

    // 계산된 플레이 영역
    private Vector2 playAreaMin;
    private Vector2 playAreaMax;
    private bool playAreaCalculated = false;

    private void Start()
    {
        CalculatePlayAreaFromWalls();
    }

    /// <summary>
    /// 벽 오브젝트로부터 플레이 영역 자동 계산
    /// </summary>
    private void CalculatePlayAreaFromWalls()
    {
        if (wallObjects == null || wallObjects.Length == 0)
        {
            Debug.LogWarning("[ShopManager] wallObjects가 할당되지 않음. 기본 영역 사용.");
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

            // BoxCollider 또는 Renderer로부터 경계 계산
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

        Debug.Log($"[ShopManager] 플레이 영역 계산 완료: Min({playAreaMin.x:F1}, {playAreaMin.y:F1}), Max({playAreaMax.x:F1}, {playAreaMax.y:F1})");
    }

    public void SpawnShopTrigger()
    {
        if (!playAreaCalculated)
        {
            CalculatePlayAreaFromWalls();
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 basePosition = player != null ? player.transform.position : Vector3.zero;
        Vector3 spawnPosition = GetRandomSpawnPositionInBounds(basePosition);

        if (shopTriggerPrefab == null)
        {
            Debug.LogError("[ShopManager] shopTriggerPrefab이 비어 있음!");
            return;
        }

        GameObject trigger = Instantiate(shopTriggerPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"[ShopManager] 트리거 생성 위치: {spawnPosition}");

        if (arrowIndicatorPrefab != null && hudCanvas != null)
        {
            GameObject indicatorObj = Instantiate(arrowIndicatorPrefab, hudCanvas.transform);

            var indicator = indicatorObj.GetComponent<ShopScreenIndicator>();
            indicator.shopTarget = trigger.transform;
            indicator.cam = Camera.main;
            indicator.canvasRect = hudCanvas.GetComponent<RectTransform>();
            indicator.arrowUI = indicatorObj.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogWarning("[ShopManager] arrowIndicatorPrefab 또는 hudCanvas가 할당되어 있지 않습니다!");
        }
    }

    /// <summary>
    /// 플레이 영역 내에서 랜덤 위치 생성
    /// </summary>
    private Vector3 GetRandomSpawnPositionInBounds(Vector3 basePosition)
    {
        float minDistance = 4f;
        float maxDistance = 7f;
        int maxAttempts = 20;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // 랜덤 방향과 거리
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            float distance = Random.Range(minDistance, maxDistance);
            
            Vector3 candidatePosition = basePosition + new Vector3(
                randomCircle.x * distance, 
                spawnHeight, 
                randomCircle.y * distance
            );

            // ✅ 플레이 영역 내부인지 확인
            if (IsPositionInPlayArea(candidatePosition))
            {
                Debug.Log($"[ShopManager] 유효한 위치 발견 (시도 {attempt + 1}/{maxAttempts}): {candidatePosition}");
                return candidatePosition;
            }
        }

        // ✅ 실패 시 플레이어 근처 안전 위치 반환
        Vector3 safePosition = ClampToPlayArea(basePosition);
        safePosition.y = spawnHeight;
        
        Debug.LogWarning($"[ShopManager] {maxAttempts}번 시도 후 유효한 위치를 찾지 못함. 안전 위치 사용: {safePosition}");
        return safePosition;
    }

    /// <summary>
    /// 위치가 플레이 영역 내부인지 확인
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
    /// 위치를 플레이 영역 내부로 제한
    /// </summary>
    private Vector3 ClampToPlayArea(Vector3 position)
    {
        float x = Mathf.Clamp(position.x, playAreaMin.x + wallPadding, playAreaMax.x - wallPadding);
        float z = Mathf.Clamp(position.z, playAreaMin.y + wallPadding, playAreaMax.y - wallPadding);
        
        return new Vector3(x, position.y, z);
    }

    /// <summary>
    /// 씬 뷰에서 플레이 영역 시각화
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!playAreaCalculated && Application.isPlaying)
            return;

        // 에디터 모드에서도 미리보기
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
    }
}
