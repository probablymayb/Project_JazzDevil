using UnityEngine;

abstract public class Supporter : MonoBehaviour
{
    public SupporterSO supporterData;

    protected ISupporterPattern ActPattern = null;

    private Transform player;
    private float timer = 0f;   // 패턴 타이머

    // 플레이어 주위 공전 관련 변수
    private float orbitSpeed = 20f;   // 공전 속도
    private Vector3 offset;     // 플레이어와의 거리

    [HideInInspector] public GameObject poolPrefabRef; // 풀 반환용 프리팹 참조

    [Header("패턴 이펙트")]
    [SerializeField] private GameObject patternEffect; // 해당하는 패턴에 대한 프리팹

    [Header("리듬 관련")]
    [HideInInspector] public float currentAngle = 0f; // SupporterManager에서 설정하는 현재 각도
    [SerializeField] private GameObject hitIndicator; // 판정 구역에 가까워졌을 때 표시할 인디케이터
    [SerializeField] private float indicatorShowRange = 45f; // 인디케이터를 보여줄 각도 범위

    private bool isInHitZone = false;
    private Renderer supporterRenderer;
    private Color originalColor;

    protected virtual void Start()
    {
        // "Player" 태그가 있는 오브젝트 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // 상시 발동 시 ActPattern 발동
        if (supporterData.attackCooldown == 0f)
        {
            ActPattern?.ActPattern(transform, player, supporterData);
        }

        // 플레이어와의 offset 초기화
        if (player != null)
        {
            offset = transform.position - player.position;
        }

        // 색상 관련 초기화
        supporterRenderer = GetComponentInChildren<Renderer>();
        if (supporterRenderer != null)
        {
            originalColor = supporterRenderer.material.color;
        }

        // 히트 인디케이터 초기화
        if (hitIndicator != null)
        {
            hitIndicator.SetActive(false);
        }
    }

    protected virtual void FixedUpdate()
    {
        // 가장 가까운 적 바라보기
        Transform lookTarget = Finder.NearestObject(transform, "Monster");
        if (lookTarget != null)
        {
            Vector3 direction = lookTarget.position - transform.position;
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        // 상시 발동이 아니면 타이머를 잰다.
        if (supporterData.attackCooldown != 0f)
        {
            timer += Time.deltaTime;
            if (timer > supporterData.attackCooldown)
            {
                // 일반 패턴은 SupporterManager의 리듬 시스템에 의해 제어되므로
                // 여기서는 실행하지 않음
                timer = 0f;
            }
        }

        // 리듬 관련 업데이트
        UpdateRhythmVisual();
    }

    /// <summary>
    /// 리듬 관련 비주얼 업데이트
    /// </summary>
    private void UpdateRhythmVisual()
    {
        // 판정 구역(각도 0도)에 가까워졌는지 체크
        float angleDifferenceToHitZone = Mathf.Abs(Mathf.DeltaAngle(currentAngle, 0f));

        bool shouldShowIndicator = angleDifferenceToHitZone <= indicatorShowRange;

        if (shouldShowIndicator != isInHitZone)
        {
            isInHitZone = shouldShowIndicator;
            OnHitZoneStateChanged(isInHitZone);
        }

        // 판정 구역에 가까워질수록 색상 변화 (선택사항)
        if (supporterRenderer != null && isInHitZone)
        {
            float intensity = 1f - (angleDifferenceToHitZone / indicatorShowRange);
            Color highlightColor = Color.Lerp(originalColor, Color.yellow, intensity * 0.5f);
            supporterRenderer.material.color = highlightColor;
        }
        else if (supporterRenderer != null)
        {
            supporterRenderer.material.color = originalColor;
        }
    }

    /// <summary>
    /// 판정 구역 상태 변화 시 호출
    /// </summary>
    private void OnHitZoneStateChanged(bool inHitZone)
    {
        if (hitIndicator != null)
        {
            hitIndicator.SetActive(inHitZone);
        }

        // 추가 피드백 (예: 사운드, 애니메이션 등)
        if (inHitZone)
        {
            Debug.Log($"{gameObject.name}이 판정 구역에 진입");
            // 여기에 진입 사운드나 애니메이션 추가 가능
        }
    }

    /// <summary>
    /// 리듬 판정 성공 시 능력 발동 (effectiveness: 0.0 ~ 1.0)
    /// </summary>
    public void ActivateRhythmAbility(float effectiveness)
    {
        Debug.Log($"{gameObject.name} 리듬 능력 발동! 효과: {effectiveness * 100}%");

        // 기본 패턴 실행
        ActPattern?.ActPattern(transform, player, supporterData);

        // 이펙트 생성
        if (patternEffect != null)
        {
            GameObject eff = PoolManager.Instance.Get(patternEffect);
            eff.transform.position = transform.position;

            // effectiveness에 따라 이펙트 크기나 지속시간 조정
            eff.transform.localScale = Vector3.one * (0.5f + effectiveness * 0.5f);
        }

        // effectiveness에 따른 추가 보너스 효과
        ApplyEffectivenessBonus(effectiveness);

        // 피드백 애니메이션
        StartCoroutine(PlayActivationFeedback(effectiveness));
    }

    /// <summary>
    /// 판정 정확도에 따른 보너스 효과 적용
    /// </summary>
    private void ApplyEffectivenessBonus(float effectiveness)
    {
        if (effectiveness >= 1.0f) // Perfect 판정
        {
            // 완벽한 판정 시 특별한 보너스
            Debug.Log("Perfect! 특별 보너스 효과 발동");
            // 예: 쿨타임 감소, 추가 데미지 등
        }
        else if (effectiveness >= 0.8f) // Good 판정
        {
            // 좋은 판정 시 일반 보너스
            Debug.Log("Good! 일반 보너스 효과 발동");
        }
        // 기타 effectiveness에 따른 처리...
    }

    /// <summary>
    /// 능력 발동 피드백 애니메이션
    /// </summary>
    private System.Collections.IEnumerator PlayActivationFeedback(float effectiveness)
    {
        // 크기 변화 애니메이션
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * (1f + effectiveness * 0.3f);

        float duration = 0.2f;
        float elapsed = 0f;

        // 확대
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        elapsed = 0f;

        // 축소
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    /// <summary>
    /// 디버그용: 현재 판정 구역까지의 각도 차이 반환
    /// </summary>
    public float GetAngleDifferenceToHitZone()
    {
        return Mathf.Abs(Mathf.DeltaAngle(currentAngle, 0f));
    }

    /// <summary>
    /// 현재 판정 구역에 있는지 여부 반환
    /// </summary>
    public bool IsInHitZone()
    {
        return isInHitZone;
    }
}
