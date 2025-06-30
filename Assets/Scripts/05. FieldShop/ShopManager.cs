using UnityEngine;

/// <summary>
/// wave 종료시 트리거 생성
/// </summary>
public class ShopManager : MonoBehaviour
{
    public GameObject shopTriggerPrefab;
    public GameObject arrowIndicatorPrefab;   // 인디케이터(화살표) 프리팹
    public Canvas hudCanvas;                  // HUD Canvas (또는 RectTransform)

    public void SpawnShopTrigger()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 basePosition = player != null ? player.transform.position : Vector3.zero;
        Vector3 spawnPosition = GetRandomSpawnPosition(basePosition);

        if (shopTriggerPrefab == null)
        {
            Debug.LogError("[ShopManager] shopTriggerPrefab이 비어 있음!");
            return;
        }

        GameObject trigger = Instantiate(shopTriggerPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"[ShopManager] 트리거 생성 위치: {spawnPosition}");

        // --- 인디케이터 생성 및 변수 연결 ---
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

    private Vector3 GetRandomSpawnPosition(Vector3 basePosition)
    {
        float minDistance = 4f;
        float maxDistance = 7f;

        Vector3 spawnPosition;
        float distance;

        int attempt = 0;

        do
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            distance = Random.Range(minDistance, maxDistance);
            spawnPosition = basePosition + new Vector3(randomCircle.x * distance, 0.5f, randomCircle.y * distance);
            attempt++;
        } while (Vector3.Distance(spawnPosition, basePosition) < minDistance && attempt < 10);

        return spawnPosition;
    }
}
