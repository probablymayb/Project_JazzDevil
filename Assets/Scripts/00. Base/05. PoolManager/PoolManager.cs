using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private Dictionary<GameObject, ObjectPool> poolDict = new Dictionary<GameObject, ObjectPool>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 풀 생성
    public void CreatePool(GameObject prefab, int initialSize = 10)
    {
        // 이미 풀 존재 시 탈출
        if (poolDict.ContainsKey(prefab)) return;

        Debug.Log($"[PoolManager] {prefab.name}Pool 생성됨.");

        ObjectPool pool = new ObjectPool(prefab, initialSize);
        poolDict.Add(prefab, pool);
    }

    // 프리팹에 맞는 풀에서 Get 요청
    public GameObject Get(GameObject prefab)
    {
        // 풀 존재하지 않으면 CreatePool
        if (!poolDict.ContainsKey(prefab))
        {
            Debug.LogWarning($"[PoolManager] {prefab.name} 프리팹에 대한 풀이 없습니다. 자동 생성합니다.");
            CreatePool(prefab, 5);
        }

        return poolDict[prefab].GetObject();
    }

    // 프리팹에 맞는 풀에 오브젝트 Return 요청
    public void Return(GameObject prefab, GameObject obj)
    {
        // 풀 존재하지 않으면 경고 로그 출력 후 탈출
        if (!poolDict.ContainsKey(prefab))
        {
            Debug.LogWarning($"[PoolManager] {prefab.name} 프리팹에 대한 풀이 없습니다. 반환할 수 없습니다.");
            return;
        }

        poolDict[prefab].ReturnObject(obj);
    }
}
