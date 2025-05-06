using UnityEngine;
using System.Collections.Generic;
public class ObjectPool
{
    private GameObject prefab;
    private Queue<GameObject> pool;
    private Transform poolParent;

    public ObjectPool(GameObject prefab, int initialSize = 10)
    {
        this.prefab = prefab;
        pool = new Queue<GameObject>(initialSize);

        // 풀의 부모 오브젝트 생성
        GameObject poolObj = new GameObject($"{prefab.name}Pool");
        poolParent = poolObj.transform;

        // 초기 오브젝트 생성
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Object.Instantiate(prefab, poolParent);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetObject()
    {
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : Object.Instantiate(prefab, poolParent);
        obj.transform.SetParent(null);
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(poolParent);
        pool.Enqueue(obj);
    }
}
