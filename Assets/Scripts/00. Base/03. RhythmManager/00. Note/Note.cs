using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Note : MonoBehaviour
{
    [Header("이미지 참조")]
    [SerializeField] private Image approachCircle;
    [SerializeField] private Image targetCircle;

    [Header("타이밍 설정 (Initialize로 덮어씌워짐)")]
    [SerializeField] private float shrinkDuration = 1f;
    [SerializeField] private float startScale = 3f;
    [SerializeField] private float targetScale = 1f;
    [SerializeField] private float endScale = 0.2f;

    [Header("알파 설정")]
    [SerializeField, Range(0f, 1f)] private float approachStartAlpha = 1f;
    [SerializeField, Range(0f, 1f)] private float targetStartAlpha = 1f;

    [Header("기본 색상")]
    [SerializeField] private Color approachDefaultColor = Color.white;
    [SerializeField] private Color targetDefaultColor = Color.white;

    [Header("히트 이펙트")]
    [SerializeField] private float hitFadeDuration = 0.15f;
    [SerializeField] private float hitDelay = 0.2f;

    [Header("Miss 허용 시간")]
    [SerializeField] private float missWindow = 0.3f;  // 타겟 지난 후 Miss 판정 시간

    private float currentTime = 0f;
    private float perfectTime;  // 정확한 판정 타이밍
    private RectTransform approachRect;
    private bool isHit = false;
    private float hitFadeTimer = 0f;

    [HideInInspector] public GameObject poolPrefabRef;

    // 판정별 색상
    public static readonly Color ExcellentColor = new Color(1f, 0.89f, 0.45f, 1f);
    public static readonly Color SolidColor = new Color(0.3f, 1f, 0.3f, 1f);
    public static readonly Color GoodColor = new Color(0.2f, 0.2f, 1.0f, 1f);
    public static readonly Color MissColor = new Color(1f, 0.2f, 0.2f, 1f);

    private void Awake()
    {
        if (approachCircle != null)
        {
            approachRect = approachCircle.GetComponent<RectTransform>();
            approachDefaultColor = approachCircle.color;
        }
        if (targetCircle != null)
        {
            targetDefaultColor = targetCircle.color;
        }
    }

    private void OnEnable()
    {
        currentTime = 0f;
        isHit = false;
        hitFadeTimer = 0f;

        if (approachCircle != null)
        {
            Color c = approachDefaultColor;
            c.a = approachStartAlpha;
            approachCircle.color = c;
            approachRect.localScale = Vector3.one * startScale;
        }

        if (targetCircle != null)
        {
            Color c = targetDefaultColor;
            c.a = targetStartAlpha;
            targetCircle.color = c;
            targetCircle.rectTransform.localScale = Vector3.one * targetScale;
        }
    }

    /// <summary>
    /// 정확한 도달 시간으로 초기화
    /// </summary>
    public void Initialize(float approachTime)
    {
        shrinkDuration = approachTime;
        perfectTime = approachTime;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != EGameState.Playing) return;

        // 히트된 상태면 페이드아웃 후 사라짐
        if (isHit)
        {
            hitFadeTimer += Time.deltaTime;
            float fadeProgress = hitFadeTimer / hitFadeDuration;

            if (approachCircle != null)
            {
                Color c = approachCircle.color;
                c.a = Mathf.Lerp(approachStartAlpha, 0f, fadeProgress);
                approachCircle.color = c;
            }
            if (targetCircle != null)
            {
                Color c = targetCircle.color;
                c.a = Mathf.Lerp(targetStartAlpha, 0f, fadeProgress);
                targetCircle.color = c;
            }

            if (fadeProgress >= 1f)
            {
                ReturnToPool();
            }
            return;
        }

        currentTime += Time.deltaTime;

        // 스케일 계산: perfectTime에 targetScale 도달
        float progress = currentTime / perfectTime;
        float currentScale;

        if (progress <= 1f)
        {
            // 아직 타겟에 도달 전
            currentScale = Mathf.Lerp(startScale, targetScale, progress);
        }
        else
        {
            // 타겟 지남 (Miss 영역)
            float overProgress = (currentTime - perfectTime) / missWindow;
            currentScale = Mathf.Lerp(targetScale, endScale, overProgress);

            // 페이드아웃
            float alpha = 1f - overProgress;
            if (approachCircle != null)
            {
                Color c = approachCircle.color;
                c.a = approachStartAlpha * alpha;
                approachCircle.color = c;
            }
            if (targetCircle != null)
            {
                Color c = targetCircle.color;
                c.a = targetStartAlpha * alpha;
                targetCircle.color = c;
            }
        }

        if (approachRect != null)
            approachRect.localScale = Vector3.one * currentScale;

        // 시간 초과 시 자동 사라짐 (Miss)
        if (currentTime >= perfectTime + missWindow)
        {
            ReturnToPool();
        }
    }

    /// <summary>
    /// 현재 판정 타이밍과의 오차 반환 (0 = 완벽)
    /// </summary>
    public float GetTimingError()
    {
        return Mathf.Abs(currentTime - perfectTime);
    }

    public void Hit(JudgementResult result)
    {
        if (isHit) return;

        Color hitColor = GetColorByResult(result);

        if (approachCircle != null)
        {
            Color c = hitColor;
            c.a = approachCircle.color.a;
            approachCircle.color = c;
        }
        if (targetCircle != null)
        {
            Color c = hitColor;
            c.a = targetCircle.color.a;
            targetCircle.color = c;
        }

        if (result != JudgementResult.Miss)
        {
            StartCoroutine(HitWithDelay());
        }
    }

    private IEnumerator HitWithDelay()
    {
        yield return new WaitForSeconds(hitDelay);
        isHit = true;
    }

    private Color GetColorByResult(JudgementResult result)
    {
        switch (result)
        {
            case JudgementResult.Excellent: return ExcellentColor;
            case JudgementResult.Solid: return SolidColor;
            case JudgementResult.Good: return GoodColor;
            case JudgementResult.Miss: return MissColor;
            default: return Color.white;
        }
    }

    private void ReturnToPool()
    {
        PoolManager.Instance.Return(poolPrefabRef, transform.parent.gameObject);
    }
}
