using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class SupporterManager : MonoBehaviour
{
    [Header("회전 설정")]
    [SerializeField] private float orbitRadius = 1f;    // 회전 반경
    [SerializeField] private float rotationSpeed = 30f; // 회전 속도

    private Transform playerTransform;

    private List<GameObject> orbitalSup = new List<GameObject>(); // 회전 동료 목록
    private float currentDeg = 0f; // 현재 회전 각

    private void Awake()
    {
        // "Player" 태그가 있는 오브젝트 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private void Update()
    {
        if (orbitalSup.Count == 0) return;

        // 회전 각 업뎃
        currentDeg += rotationSpeed * Time.deltaTime;

        // 360 초과 시 360을 빼기
        if (currentDeg > 360f)
        {
            currentDeg -= 360f;
        }

        // 동료 위치 업뎃
        UpdateSupPos();
    }

    // 동료 위치를 업데이트
    private void UpdateSupPos()
    {
        int supCount = orbitalSup.Count;
        float angleStep = 360f / supCount; // 동료 간 각도 간격

        for (int i = 0; i < supCount; i++)
        {
            if (orbitalSup[i] != null)
            {
                // 각 동료의 회전 각 계산
                float angle = currentDeg + (i * angleStep);
                float radians = angle * Mathf.Deg2Rad;

                // 새 위치 계산
                Vector3 newPos = new Vector3(
                    Mathf.Cos(radians) * orbitRadius,
                    0f,
                    Mathf.Sin(radians) * orbitRadius
                );

                // 플레이어 위치 기준으로 동료 위치 설정
                orbitalSup[i].transform.position = playerTransform.position + newPos;
            }
        }
    }

    // 프리팹을 받아서 동료를 생성 (풀링 적용)
    public void AddSup(GameObject prefab)
    {
        GameObject sup = PoolManager.Instance.Get(prefab);
        sup.GetComponent<Supporter>().poolPrefabRef = prefab; // 반환용 참조
        orbitalSup.Add(sup);
        UpdateSupPos();
    }

    // 해당 프리팹의 동료를 제거
    public void RemoveSup(GameObject prefab)
    {
        GameObject sup = orbitalSup.Find(obj => obj.GetComponent<Supporter>().poolPrefabRef == prefab);
        if (sup == null)
        {
            Debug.LogWarning($"{prefab.name}을 SupporterManager에서 찾을 수 없습니다.");
        }
        else
        {
            PoolManager.Instance.Return(prefab, sup);
            orbitalSup.Remove(sup);

            // 동료 위치 업뎃
            if (orbitalSup.Count > 0)
            {
                UpdateSupPos();
            }
        }
    }
}
