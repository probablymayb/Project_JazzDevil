using UnityEngine;

/// <summary>
/// 전체 게임 시간이 흐름에 따라 LiveShopTrigger를 3회 소환함 (1/4, 2/4, 3/4 시점)
/// </summary>
public class ShopManager : MonoBehaviour
{
    public GameObject fieldShopPrefab;
    public float totalGameDuration = 3f; // 일단 총 3분 게임 - 타이머 수정하면서 수정 예정

    private float elapsedTime = 0f;
    private int spawnedCount = 0;

    void Update()
    {
        elapsedTime += Time.deltaTime;

        float nextSpawnTime = (spawnedCount + 1) * (totalGameDuration / 4f);
        if (spawnedCount < 3 && elapsedTime >= nextSpawnTime)
        {
            SpawnShopTrigger();
            spawnedCount++;
        }
    }

    private void SpawnShopTrigger()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 basePosition = player != null ? player.transform.position : Vector3.zero;

        Vector3 spawnPosition = GetRandomSpawnPosition(basePosition);

        if (fieldShopPrefab == null)
        {
            Debug.LogError("[ShopManager] shopTriggerPrefab이 비어 있음!");
            return;
        }

        GameObject trigger = Instantiate(fieldShopPrefab, spawnPosition, Quaternion.identity);
    }

    private Vector3 GetRandomSpawnPosition(Vector3 basePosition)
    {
        float minDistance = 3f;
        float maxDistance = 8f;

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
