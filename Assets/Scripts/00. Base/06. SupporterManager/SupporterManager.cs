using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 동료를 호출하는 등의 연산을 위한 enum 리스트
public enum ESupporters { Trumpet, Piano, Saxophone, KontraBass, Guitar }

public class SupporterManager : Singleton<SupporterManager>
{
    //Supporter 프리팹들 참조
    [field: SerializeField] public GameObject[] SupporterPrefs { get; private set; }
    //Supporter 스크립터블 오브젝트
    [field: SerializeField] public SupporterSO[] SupporterSos { get; private set; }

    [Header("회전 설정")]
    [SerializeField] private float orbitRadius = 1f;    // 회전 반경
    [SerializeField] private float maxRotSpeed = 100f;   // 회전 최대 속도
    private float rotationSpeed; // 회전 속도

    [Header("리듬 판정 설정")]
    [SerializeField] private float hitZoneAngle = 0f;       // 판정 구역 각도 (예: 플레이어 앞쪽 0도)
    [SerializeField] private float perfectAngleRange = 15f; // Perfect 판정 각도 범위 (±15도)
    [SerializeField] private float goodAngleRange = 30f;    // Good 판정 각도 범위 (±30도)
    [SerializeField] private float badAngleRange = 45f;     // Bad 판정 각도 범위 (±45도)

    [Header("비주얼 피드백")]
    [SerializeField] private LineRenderer hitZoneIndicator; // 판정 구역 표시용
    [SerializeField] private Color perfectColor = Color.green;
    [SerializeField] private Color goodColor = Color.yellow;
    [SerializeField] private Color badColor = Color.red;

    private Transform playerTransform;
    private List<GameObject> orbitalSup = new List<GameObject>(); // 회전 동료 목록 (HEAD 버전 유지)
    public HashSet<ESupporters> OwnedSupporters = new HashSet<ESupporters>();   // 보유 동료 리스트 (main에서 추가)
    private float currentDeg = 0f; // 현재 회전 각

    // 리듬 판정 관련
    public event System.Action<JudgementResult, Supporter> OnRhythmJudged;

    protected override void Awake()
    {
        base.Awake();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        RhythmManager.beatUpdated += OnBeat;
        rotationSpeed = maxRotSpeed;

        SetupHitZoneVisual();
    }

    private void OnDestroy()
    {
        RhythmManager.beatUpdated -= OnBeat;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != EGameState.Playing) return;

        if (orbitalSup.Count == 0) return;

        // 회전 각 업뎃
        currentDeg += rotationSpeed * Time.deltaTime;

        if (currentDeg > 360f)
        {
            currentDeg -= 360f;
        }

        // 동료 위치 업뎃
        UpdateSupPos();

        // 판정 구역 비주얼 업데이트
        UpdateHitZoneVisual();

        // 스페이스바 입력 처리
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ProcessRhythmInput();
        }
    }

    // 동료 위치를 업데이트
    private void UpdateSupPos()
    {
        int supCount = orbitalSup.Count;
        float angleStep = 360f / supCount;

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

                // 동료에게 현재 각도 정보 전달
                Supporter supporterComponent = orbitalSup[i].GetComponent<Supporter>();
                if (supporterComponent != null)
                {
                    supporterComponent.currentAngle = angle;
                }
            }
        }
    }

    // 리듬 입력 처리
    private void ProcessRhythmInput()
    {
        if (orbitalSup.Count == 0) return;

        // 판정 구역에 가장 가까운 동료 찾기
        Supporter closestSupporter = GetClosestSupporterToHitZone();

        if (closestSupporter != null)
        {
            float angleDifference = GetAngleDifferenceToHitZone(closestSupporter.currentAngle);
            JudgementResult result = JudgeHitAccuracy(angleDifference);

            // 판정 결과 처리
            HandleRhythmJudgement(result, closestSupporter);
        }
        else
        {
            // 동료가 없으면 Miss
            HandleRhythmJudgement(JudgementResult.Miss, null);
        }
    }

    // 판정 구역에 가장 가까운 동료 찾기
    private Supporter GetClosestSupporterToHitZone()
    {
        Supporter closest = null;
        float minAngleDiff = float.MaxValue;

        foreach (GameObject supObj in orbitalSup)
        {
            if (supObj != null)
            {
                Supporter supporter = supObj.GetComponent<Supporter>();
                if (supporter != null)
                {
                    float angleDiff = GetAngleDifferenceToHitZone(supporter.currentAngle);

                    if (angleDiff < minAngleDiff && angleDiff <= badAngleRange)
                    {
                        minAngleDiff = angleDiff;
                        closest = supporter;
                    }
                }
            }
        }

        return closest;
    }

    // 판정 구역과의 각도 차이 계산
    private float GetAngleDifferenceToHitZone(float supporterAngle)
    {
        float diff = Mathf.Abs(Mathf.DeltaAngle(supporterAngle, hitZoneAngle));
        return diff;
    }

    // 각도 차이에 따른 판정
    private JudgementResult JudgeHitAccuracy(float angleDifference)
    {
        if (angleDifference <= perfectAngleRange)
            return JudgementResult.Excellent;
        else if (angleDifference <= goodAngleRange)
            return JudgementResult.Solid;
        else if (angleDifference <= badAngleRange)
            return JudgementResult.Good;
        else
            return JudgementResult.Miss;
    }

    // 리듬 판정 결과 처리
    private void HandleRhythmJudgement(JudgementResult result, Supporter supporter)
    {
        Debug.Log($"리듬 판정: {result}");

        // 이벤트 발생
        OnRhythmJudged?.Invoke(result, supporter);

        // 판정에 따른 처리
        switch (result)
        {
            case JudgementResult.Excellent:
                if (supporter != null)
                {
                    // 동료 특수 능력 발동 (100% 효과)
                    TriggerSupporterAbility(supporter, 1.0f);
                    PlayHitEffect(supporter.transform.position, perfectColor);
                }
                break;

            case JudgementResult.Solid:
                if (supporter != null)
                {
                    // 동료 특수 능력 발동 (80% 효과)
                    TriggerSupporterAbility(supporter, 0.8f);
                    PlayHitEffect(supporter.transform.position, goodColor);
                }
                break;

            case JudgementResult.Good:
                if (supporter != null)
                {
                    // 동료 특수 능력 발동 (60% 효과)
                    TriggerSupporterAbility(supporter, 0.6f);
                    PlayHitEffect(supporter.transform.position, badColor);
                }
                break;

            case JudgementResult.Miss:
                Debug.Log("Miss! 동료가 판정 구역에 없거나 타이밍이 맞지 않음");
                break;
        }
    }

    // 동료 특수 능력 발동
    private void TriggerSupporterAbility(Supporter supporter, float effectiveness)
    {
        // 기존 ActPattern 실행하되 효과는 effectiveness에 따라 조정
        //if (supporter.ActPattern != null)
        //{
        //    supporter.ActPattern.ActPattern(supporter.transform, playerTransform, supporter.supporterData);
        //}

        // 추가로 effectiveness에 따른 보너스 효과 적용 가능
        // 예: 데미지 배율, 특수 효과 등
    }

    // 히트 이펙트 재생
    private void PlayHitEffect(Vector3 position, Color color)
    {
        // 이펙트 재생 로직
        // 예: 파티클, 사운드 등
    }

    // 판정 구역 비주얼 설정
    private void SetupHitZoneVisual()
    {
        if (hitZoneIndicator == null) return;

        hitZoneIndicator.positionCount = 3;
        hitZoneIndicator.startWidth = 0.1f;
        hitZoneIndicator.endWidth = 0.1f;
        hitZoneIndicator.material.color = Color.white;
    }

    // 판정 구역 비주얼 업데이트
    private void UpdateHitZoneVisual()
    {
        if (hitZoneIndicator == null || playerTransform == null) return;

        Vector3 playerPos = playerTransform.position;
        float radians = hitZoneAngle * Mathf.Deg2Rad;

        Vector3 hitZoneDirection = new Vector3(
            Mathf.Cos(radians),
            0f,
            Mathf.Sin(radians)
        );

        // 판정 구역 라인 그리기
        hitZoneIndicator.SetPosition(0, playerPos);
        hitZoneIndicator.SetPosition(1, playerPos + hitZoneDirection * orbitRadius * 0.8f);
        hitZoneIndicator.SetPosition(2, playerPos + hitZoneDirection * orbitRadius * 1.2f);
    }

    // 프리팹을 받아서 동료를 생성 (풀링 적용)
    public void AddSup(ESupporters enumSup)
    {
        if (!Enum.IsDefined(typeof(ESupporters), enumSup))
        {
            Debug.LogError("[SupporterManager][AddSup] 유효하지 않은 enum 값");
            return;
        }
        GameObject getPref = SupporterPrefs[Convert.ToInt32(enumSup)];
        GameObject sup = PoolManager.Instance.Get(getPref);
        sup.GetComponent<Supporter>().poolPrefabRef = getPref;
        orbitalSup.Add(sup);
        AddOwnedSupporter(enumSup); // main 브랜치에서 추가된 기능
        UpdateSupPos();
    }

    // 해당 프리팹의 동료를 제거
    public void RemoveSup(ESupporters enumSup)
    {
        if (!Enum.IsDefined(typeof(ESupporters), enumSup))
        {
            Debug.LogError("[SupporterManager][RemoveSup] 유효하지 않은 enum 값");
            return;
        }
        GameObject getPref = SupporterPrefs[Convert.ToInt32(enumSup)];
        GameObject sup = orbitalSup.Find(obj => obj.GetComponent<Supporter>().poolPrefabRef == getPref);
        if (sup == null)
        {
            Debug.LogWarning($"{getPref.name}을 SupporterManager에서 찾을 수 없습니다.");
        }
        else
        {
            PoolManager.Instance.Return(getPref, sup);
            orbitalSup.Remove(sup);
            RemoveOwnedSupporter(enumSup); // main 브랜치에서 추가된 기능

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

    // 박자에 맞춰 동료를 움직이는 코루틴
    private IEnumerator PulsateAnimation()
    {
        float timer = 0f;
        float duration = 60f / RhythmManager.Instance.CurrentBpm;

        while (timer < duration)
        {
            if (this == null) yield break;

            timer += Time.deltaTime;
            rotationSpeed = Mathf.Lerp(maxRotSpeed, 0f, timer / duration);
            yield return null;
        }
    }

    // ========== main 브랜치에서 추가된 소유 시스템 관련 메서드들 ==========

    /// <summary>
    /// 동료를 보유하고 있는지 확인
    /// </summary>
    /// <param name="supporterType"></param>
    /// <returns></returns>
    public bool IsSupporterOwned(ESupporters supporterType)
    {
        return OwnedSupporters.Contains(supporterType);
    }

    /// <summary>
    /// 동료 보유 해쉬 집합에 파라미터의 동료 타입을 추가한다.
    /// </summary>
    /// <param name="supporterType"></param>
    public void AddOwnedSupporter(ESupporters supporterType)
    {
        if (OwnedSupporters.Add(supporterType))
        {
            Debug.Log($"Supporter {supporterType} 획득");
        }
        else
        {
            Debug.LogWarning($"Supporter {supporterType}는 이미 소유 중");
        }
    }

    /// <summary>
    /// 동료 보유 해쉬 집합에 파라미터의 동료 타입을 삭제한다.
    /// </summary>
    /// <param name="supporterType"></param>
    public void RemoveOwnedSupporter(ESupporters supporterType)
    {
        if (OwnedSupporters.Remove(supporterType))
        {
            Debug.Log($"Supporter {supporterType} 제거");
        }
        else
        {
            Debug.LogWarning($"Supporter {supporterType}는 소유 중이지 않음");
        }
    }
}