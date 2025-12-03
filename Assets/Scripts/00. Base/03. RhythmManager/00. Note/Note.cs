using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Note : MonoBehaviour
{
    [Header("이미지 참조")]
    [SerializeField] private Image approachCircle;
    [SerializeField] private Image targetCircle;

    [Header("타이밍 설정")]
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
    [SerializeField] private float hitDelay = 0.2f;  // 색상 유지 시간

    private float currentTime = 0f;
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
            approachDefaultColor = approachCircle.color;  // 기본 색상 저장
        }
        if (targetCircle != null)
        {
            targetDefaultColor = targetCircle.color;  // 기본 색상 저장
        }
    }

    private void OnEnable()
    {
        currentTime = 0f;
        isHit = false;
        hitFadeTimer = 0f;

        // 어프로치 서클 초기화 (색상 + 알파 + 크기)
        if (approachCircle != null)
        {
            Color c = approachDefaultColor;
            c.a = approachStartAlpha;
            approachCircle.color = c;
            approachRect.localScale = Vector3.one * startScale;
        }

        // 타겟 서클 초기화 (색상 + 알파 + 크기)
        if (targetCircle != null)
        {
            Color c = targetDefaultColor;
            c.a = targetStartAlpha;
            targetCircle.color = c;
            targetCircle.rectTransform.localScale = Vector3.one * targetScale;
        }
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

        // 일반 축소 로직
        currentTime += Time.deltaTime;
        float progress = currentTime / shrinkDuration;

        float currentScale = Mathf.Lerp(startScale, endScale, progress);

        if (approachRect != null)
            approachRect.localScale = Vector3.one * currentScale;

        // 타겟 지나면 페이드아웃
        if (progress > 0.7f)
        {
            float fadeProgress = (progress - 0.7f) / 0.3f;

            if (approachCircle != null)
            {
                Color c = approachCircle.color;
                c.a = approachStartAlpha * (1f - fadeProgress);
                approachCircle.color = c;
            }
            if (targetCircle != null)
            {
                Color c = targetCircle.color;
                c.a = targetStartAlpha * (1f - fadeProgress);
                targetCircle.color = c;
            }
        }

        if (progress >= 1f)
        {
            ReturnToPool();
        }
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
            if (result != JudgementResult.Miss)
            {
                // 바로 isHit = true 대신 코루틴으로 딜레이
                StartCoroutine(HitWithDelay());
            }
        }
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

    private IEnumerator HitWithDelay() //판정노트 살짝 유지시키기
    {
        yield return new WaitForSeconds(hitDelay);  // 색상 유지
        isHit = true;  // 이후 페이드아웃 시작
    }
}
