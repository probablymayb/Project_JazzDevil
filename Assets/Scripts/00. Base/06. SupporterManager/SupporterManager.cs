using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 동료를 호출하는 등의 연산을 위한 enum 리스트
public enum ESupporters { Trumpet, Piano, Saxophone, KontraBass, Guitar }

public class SupporterManager : Singleton<SupporterManager>
{
    // 업그레이드 배율 상수
    private const float UPGRADE_MULTIPLIER = 1.5f;

    //Supporter 프리팹들 참조
    [field: SerializeField] public GameObject[] SupporterPrefs { get; private set; }
    //Supporter 스크립터블 오브젝트
    [field: SerializeField] public SupporterSO[] SupporterSos { get; private set; }

    [Header("회전 설정")]
    [SerializeField] private float orbitRadius = 1f;    // 회전 반경
    [SerializeField] private float maxRotSpeed = 100f;   // 회전 최대 속도
    private float rotationSpeed; // 회전 속도

    private Transform playerTransform;

    public List<GameObject> OrbitalSup { get; private set; } = new List<GameObject>(); // 회전 동료 오브젝트 목록
    public HashSet<ESupporters> OwnedSupporters = new HashSet<ESupporters>();   // 보유 동료 리스트
    // 레벨 추적 (최초 획득 시 1, 업그레이드할 때마다 +1)
    public Dictionary<ESupporters, int> SupporterLevels = new Dictionary<ESupporters, int>();
    
    // 런타임 스탯 관리 (SO는 읽기 전용 유지)
    [System.Serializable]
    public class RuntimeStats
    {
        public float attackCooldown;
        public int attackDamage;
        public float attackRange;
    }
    public Dictionary<ESupporters, RuntimeStats> SupporterStats = new Dictionary<ESupporters, RuntimeStats>();
    
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
        // 게임 상태가 Playing이 아니면 Update 수행하지 않음
        if (GameManager.Instance.CurrentGameState != EGameState.Playing) return;

        if (OrbitalSup.Count == 0) return;

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
        int supCount = OrbitalSup.Count;
        float angleStep = 360f / supCount; // 동료 간 각도 간격

        for (int i = 0; i < supCount; i++)
        {
            if (OrbitalSup[i] != null)
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
                OrbitalSup[i].transform.position = playerTransform.position + newPos;
            }
        }
    }

    // 프리팹을 받아서 동료를 생성 (풀링 적용) 또는 이미 존재하면 업그레이드
    public void AddSup(ESupporters enumSup)
    {
        if (!Enum.IsDefined(typeof(ESupporters), enumSup))
        {
            Debug.LogError("[SupporterManager][AddSup] 유효하지 않은 enum 값");
            return;
        }

        // 이미 소유 중이면 업그레이드만 수행
        if (IsSupporterOwned(enumSup))
        {
            UpgradeSupporter(enumSup);
            return;
        }

        GameObject getPref = SupporterPrefs[Convert.ToInt32(enumSup)];
        GameObject sup = PoolManager.Instance.Get(getPref);
        sup.GetComponent<Supporter>().poolPrefabRef = getPref; // 반환용 참조
        OrbitalSup.Add(sup);
        AddOwnedSupporter(enumSup);
        // 최초 레벨 1 설정
        SupporterLevels[enumSup] = 1;
        
        // 런타임 스탯 초기화 (SO 원본값 복사)
        SupporterSO so = SupporterSos[Convert.ToInt32(enumSup)];
        SupporterStats[enumSup] = new RuntimeStats
        {
            attackCooldown = so.attackCooldown,
            attackDamage = so.attackDamage,
            attackRange = so.attackRange
        };
        
        UpdateSupPos();
    }

    // 이미 존재하는 Supporter 능력치 1.5배 강화
    private void UpgradeSupporter(ESupporters enumSup)
    {
        GameObject sup = OrbitalSup.Find(obj => obj.GetComponent<Supporter>().supporterData.supporterType == enumSup);
        if (sup == null)
        {
            Debug.LogWarning($"[SupporterManager][UpgradeSupporter] {enumSup} 인스턴스를 찾을 수 없음");
            return;
        }

        // 런타임 스탯 업그레이드 (SO는 변경하지 않음)
        if (!SupporterStats.ContainsKey(enumSup))
        {
            Debug.LogError($"[SupporterManager][UpgradeSupporter] {enumSup} 런타임 스탯 없음");
            return;
        }

        RuntimeStats stats = SupporterStats[enumSup];
        stats.attackDamage = Mathf.CeilToInt(stats.attackDamage * UPGRADE_MULTIPLIER);
        stats.attackRange *= UPGRADE_MULTIPLIER;
        if (stats.attackCooldown > 0f)
        {
            stats.attackCooldown /= UPGRADE_MULTIPLIER;
        }

        // 레벨 증가 (기존 없으면 1에서 시작 후 2로 증가)
        if (SupporterLevels.ContainsKey(enumSup))
            SupporterLevels[enumSup]++;
        else
            SupporterLevels[enumSup] = 2;

        Debug.Log($"Supporter {enumSup} 업그레이드 적용 => 공격력:{stats.attackDamage}, 범위:{stats.attackRange}, 쿨타임:{stats.attackCooldown}");
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
        GameObject sup = OrbitalSup.Find(obj => obj.GetComponent<Supporter>().poolPrefabRef == getPref);
        if (sup == null)
        {
            Debug.LogWarning($"{getPref.name}을 SupporterManager에서 찾을 수 없습니다.");
        }
        else
        {
            PoolManager.Instance.Return(getPref, sup);
            OrbitalSup.Remove(sup);
            RemoveOwnedSupporter(enumSup);

            // 동료 위치 업뎃
            if (OrbitalSup.Count > 0)
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
            // 레벨 정보도 제거
            if (SupporterLevels.ContainsKey(supporterType))
                SupporterLevels.Remove(supporterType);
            // 런타임 스탯도 제거
            if (SupporterStats.ContainsKey(supporterType))
                SupporterStats.Remove(supporterType);
        }
        else
        {
            Debug.LogWarning($"Supporter {supporterType}는 소유 중이지 않음");
        }
    }

    /// <summary>
    /// Supporter 레벨 반환 (없으면 0)
    /// </summary>
    public int GetSupporterLevel(ESupporters supporterType)
    {
        if (SupporterLevels.TryGetValue(supporterType, out int lvl)) return lvl;
        return 0;
    }

    /// <summary>
    /// 런타임 스탯 조회 (없으면 null)
    /// </summary>
    public RuntimeStats GetRuntimeStats(ESupporters supporterType)
    {
        if (SupporterStats.TryGetValue(supporterType, out RuntimeStats stats)) return stats;
        return null;
    }
}
