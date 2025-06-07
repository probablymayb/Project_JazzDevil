using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 동료를 호출하는 등의 연산을 위한 enum 리스트
public enum Supporters { Trumpet, Piano, Saxophone }

public class SupporterManager : Singleton<SupporterManager>
{

    //Supporter 프리팹들 참조
    [SerializeField] private GameObject[] supporters;

    [Header("회전 설정")]
    [SerializeField] private float orbitRadius = 1f;    // 회전 반경
    [SerializeField] private float maxRotSpeed = 100f;   // 회전 최대 속도
    private float rotationSpeed; // 회전 속도

    private Transform playerTransform;

    private List<GameObject> orbitalSup = new List<GameObject>(); // 회전 동료 목록
    private float currentDeg = 0f; // 현재 회전 각

    protected override void Awake()
    {
        // Singleton<T>(부모 클래스)의 Awake() 먼저 수행
        base.Awake();

        // "Player" 태그가 있는 오브젝트 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        RhythmManager.beatUpdated += OnBeat;

        rotationSpeed = maxRotSpeed; // 회전 속도 초기화
    }

    private void OnDestroy()
    {
        RhythmManager.beatUpdated -= OnBeat;
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
    public void AddSup(Supporters enumSup)
    {
        if (!Enum.IsDefined(typeof(Supporters), enumSup))
        {
            Debug.LogError("[SupporterManager][AddSup] 유효하지 않은 enum 값");
            return;
        }
        GameObject getPref = supporters[Convert.ToInt32(enumSup)];
        GameObject sup = PoolManager.Instance.Get(getPref);
        sup.GetComponent<Supporter>().poolPrefabRef = getPref; // 반환용 참조
        orbitalSup.Add(sup);
        UpdateSupPos();
    }

    // 해당 프리팹의 동료를 제거
    public void RemoveSup(Supporters enumSup)
    {
        if (!Enum.IsDefined(typeof(Supporters), enumSup))
        {
            Debug.LogError("[SupporterManager][RemoveSup] 유효하지 않은 enum 값");
            return;
        }
        GameObject getPref = supporters[Convert.ToInt32(enumSup)];
        GameObject sup = orbitalSup.Find(obj => obj.GetComponent<Supporter>().poolPrefabRef == getPref);
        if (sup == null)
        {
            Debug.LogWarning($"{getPref.name}을 SupporterManager에서 찾을 수 없습니다.");
        }
        else
        {
            PoolManager.Instance.Return(getPref, sup);
            orbitalSup.Remove(sup);

            // 동료 위치 업뎃
            if (orbitalSup.Count > 0)
            {
                UpdateSupPos();
            }
        }
    }

    // 박자에 맞춰 코루틴을 실행
    private void OnBeat()
    {
        if (!isActiveAndEnabled) return;
        StartCoroutine(PulsateAnimation());
    }

    // 박자에 맞춰 동료를 움직이는 코루틴 (Monster.cs에서 가져옴)
    // 추후 동료 애니메이션 구현 시 주석 처리된 코드 사용할 예정임
    private IEnumerator PulsateAnimation()
    {
        float timer = 0f;
        float duration = 60f / RhythmManager.Instance.CurrentBpm;

        //if (animator == null) yield break;

        //animator.speed = startSpeed;

        while (timer < duration)
        {
            if (this == null/* || animator == null*/) yield break;

            timer += Time.deltaTime;
            //animator.speed = Mathf.Lerp(maxRotSpeed, 0f, timer / duration);
            rotationSpeed = Mathf.Lerp(maxRotSpeed, 0f, timer / duration);
            yield return null;
        }

        //if (animator != null)
        //    animator.speed = 0.1f;
    }

}
