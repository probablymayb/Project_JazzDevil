using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ObjectPool
{
    private GameObject prefab;
    private Queue<GameObject> inactiveObjects;
    private Transform poolParent;

    public ObjectPool(GameObject prefab, int initialSize)
    {
        this.prefab = prefab;
        inactiveObjects = new Queue<GameObject>(initialSize);

        // 풀의 부모 오브젝트 생성
        GameObject poolObj = new GameObject($"{prefab.name}Pool");
        poolParent = poolObj.transform;

        // 초기 오브젝트 생성
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Object.Instantiate(prefab, poolParent);
            obj.SetActive(false);
            inactiveObjects.Enqueue(obj);
        }
    }

    public GameObject GetObject()
    {
        if (inactiveObjects.Count > 0)
        {
            return inactiveObjects.Dequeue();
        }
        else
        {
            // 풀이 비었으면 새로운 오브젝트 생성
            GameObject newObject = Object.Instantiate(prefab, poolParent);
            return newObject;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(poolParent);
        inactiveObjects.Enqueue(obj);
    }
}