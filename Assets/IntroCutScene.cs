using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroCutScene : MonoBehaviour
{
    [Header("컷씬 설정")]
    [SerializeField] private Sprite[] cutsceneSprites;                  // 동적 배열로 변경
    [SerializeField] private Image cutsceneImage;                       // 컷씬을 표시할 Image 컴포넌트
    [SerializeField] private float slideTransitionTime = 0.5f;          // 슬라이드 전환 시간

    [Header("페이드 설정")]
    [SerializeField] private ScreenFader screenFader;                   // 페이드 효과용
    [SerializeField] private float autoProgressDelay = 3f;              // 자동 진행 딜레이 (선택사항)

    [Header("UI 요소")]
    [SerializeField] private GameObject skipPrompt;                     // "아무 키나 눌러서 계속..." 텍스트
    [SerializeField] private Text slideCounterText;                     // "1/4" 같은 진행상황 표시 (선택사항)

    [Header("자동 진행 설정")]
    [SerializeField] private bool enableAutoProgress = false;           // 자동 진행 활성화 여부

    private int currentSlideIndex = 0;
    private bool isTransitioning = false;
    private bool canProgress = true;
    private Coroutine autoProgressCoroutine;
    private int totalSlides => cutsceneSprites?.Length ?? 0;            // 총 슬라이드 개수

    void Start()
    {
        // 초기 설정 및 유효성 검사
        if (!ValidateSetup())
        {
            // 설정이 유효하지 않으면 바로 메인 씬으로 이동
            Debug.LogWarning("컷씬 설정이 유효하지 않아 메인 씬으로 이동합니다.");
            StartCoroutine(GoToMainScene());
            return;
        }

        Debug.Log($"컷씬 시작: 총 {totalSlides}개의 슬라이드");

        // 화면 페이드 인으로 시작
        if (screenFader != null)
        {
            StartCoroutine(StartIntro());
        }
        else
        {
            ShowCurrentSlide();
        }
    }

    void Update()
    {
        // ESC 키로 전체 컷씬 스킵
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SkipCutscene();
            return;
        }

        // 아무 키나 눌렀을 때 다음 슬라이드로 진행 (ESC 제외)
        if (canProgress && !isTransitioning && Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
        {
            NextSlide();
        }
    }

    /// <summary>
    /// 설정 유효성 검사
    /// </summary>
    private bool ValidateSetup()
    {
        if (cutsceneSprites == null || cutsceneSprites.Length == 0)
        {
            Debug.LogError("컷씬 스프라이트가 설정되지 않았습니다!");
            return false;
        }

        if (cutsceneImage == null)
        {
            Debug.LogError("컷씬 이미지 컴포넌트가 설정되지 않았습니다!");
            return false;
        }

        // null 스프라이트 개수 체크
        int nullCount = 0;
        for (int i = 0; i < cutsceneSprites.Length; i++)
        {
            if (cutsceneSprites[i] == null)
            {
                nullCount++;
                Debug.LogWarning($"컷씬 스프라이트 {i}번이 비어있습니다!");
            }
        }

        if (nullCount == cutsceneSprites.Length)
        {
            Debug.LogError("모든 컷씬 스프라이트가 비어있습니다!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 인트로 시작 (페이드 인 효과와 함께)
    /// </summary>
    private IEnumerator StartIntro()
    {
        yield return screenFader.FadeIn();
        ShowCurrentSlide();
        canProgress = true;
    }

    /// <summary>
    /// 현재 슬라이드 표시
    /// </summary>
    private void ShowCurrentSlide()
    {
        if (currentSlideIndex >= totalSlides)
        {
            // 모든 슬라이드가 끝났으면 메인 씬으로 이동
            StartCoroutine(GoToMainScene());
            return;
        }

        // 현재 슬라이드가 null인 경우 다음 슬라이드로 건너뛰기
        if (cutsceneSprites[currentSlideIndex] == null)
        {
            Debug.LogWarning($"슬라이드 {currentSlideIndex}가 비어있어 건너뜁니다.");
            currentSlideIndex++;
            ShowCurrentSlide();
            return;
        }

        // 현재 슬라이드 이미지 설정
        if (cutsceneImage != null)
        {
            cutsceneImage.sprite = cutsceneSprites[currentSlideIndex];
            cutsceneImage.color = new Color(1, 1, 1, 0); // 투명하게 시작

            // 페이드 인 애니메이션
            StartCoroutine(FadeInSlide());
        }

        // 스킵 프롬프트 표시
        if (skipPrompt != null)
        {
            skipPrompt.SetActive(true);
        }

        // 슬라이드 카운터 업데이트
        UpdateSlideCounter();

        // 자동 진행 시작 (활성화된 경우)
        if (enableAutoProgress && autoProgressDelay > 0)
        {
            StartAutoProgress();
        }

        Debug.Log($"컷씬 {currentSlideIndex + 1}/{totalSlides} 표시 중");
    }

    /// <summary>
    /// 슬라이드 카운터 UI 업데이트
    /// </summary>
    private void UpdateSlideCounter()
    {
        if (slideCounterText != null)
        {
            slideCounterText.text = $"{currentSlideIndex + 1}/{totalSlides}";
        }
    }

    /// <summary>
    /// 자동 진행 시작
    /// </summary>
    private void StartAutoProgress()
    {
        // 기존 코루틴이 있다면 중단
        if (autoProgressCoroutine != null)
        {
            StopCoroutine(autoProgressCoroutine);
        }

        autoProgressCoroutine = StartCoroutine(AutoProgressSlide());
    }

    /// <summary>
    /// 자동 진행 코루틴
    /// </summary>
    private IEnumerator AutoProgressSlide()
    {
        yield return new WaitForSeconds(autoProgressDelay);

        if (canProgress && !isTransitioning)
        {
            NextSlide();
        }
    }

    /// <summary>
    /// 슬라이드 페이드 인 애니메이션
    /// </summary>
    private IEnumerator FadeInSlide()
    {
        float elapsed = 0f;
        Color startColor = new Color(1, 1, 1, 0);
        Color endColor = new Color(1, 1, 1, 1);

        while (elapsed < slideTransitionTime)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / slideTransitionTime;

            if (cutsceneImage != null)
            {
                cutsceneImage.color = Color.Lerp(startColor, endColor, progress);
            }

            yield return null;
        }

        if (cutsceneImage != null)
        {
            cutsceneImage.color = endColor;
        }
    }

    /// <summary>
    /// 슬라이드 페이드 아웃 애니메이션
    /// </summary>
    private IEnumerator FadeOutSlide()
    {
        float elapsed = 0f;
        Color startColor = new Color(1, 1, 1, 1);
        Color endColor = new Color(1, 1, 1, 0);

        while (elapsed < slideTransitionTime)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / slideTransitionTime;

            if (cutsceneImage != null)
            {
                cutsceneImage.color = Color.Lerp(startColor, endColor, progress);
            }

            yield return null;
        }

        if (cutsceneImage != null)
        {
            cutsceneImage.color = endColor;
        }
    }

    /// <summary>
    /// 다음 슬라이드로 진행
    /// </summary>
    public void NextSlide()
    {
        if (isTransitioning) return;

        // 자동 진행 코루틴 중단
        if (autoProgressCoroutine != null)
        {
            StopCoroutine(autoProgressCoroutine);
            autoProgressCoroutine = null;
        }

        StartCoroutine(TransitionToNextSlide());
    }

    /// <summary>
    /// 다음 슬라이드로 전환하는 코루틴
    /// </summary>
    private IEnumerator TransitionToNextSlide()
    {
        isTransitioning = true;
        canProgress = false;

        // 현재 슬라이드 페이드 아웃
        yield return StartCoroutine(FadeOutSlide());

        // 다음 슬라이드 인덱스로 이동
        currentSlideIndex++;

        // 다음 슬라이드 표시
        ShowCurrentSlide();

        isTransitioning = false;
        canProgress = true;
    }

    /// <summary>
    /// 메인 씬으로 이동
    /// </summary>
    private IEnumerator GoToMainScene()
    {
        canProgress = false;

        // 자동 진행 코루틴 중단
        if (autoProgressCoroutine != null)
        {
            StopCoroutine(autoProgressCoroutine);
            autoProgressCoroutine = null;
        }

        // 스킵 프롬프트 숨기기
        if (skipPrompt != null)
        {
            skipPrompt.SetActive(false);
        }

        Debug.Log($"컷씬 완료! 총 {totalSlides}개 슬라이드 중 {currentSlideIndex}개 표시함. 메인 씬으로 이동합니다.");

        // 페이드 아웃 후 씬 전환
        if (screenFader != null)
        {
            yield return screenFader.FadeOut();
        }

        SceneLoader.LoadScene(SceneLoader.SceneName.Main);
    }

    /// <summary>
    /// 컷씬 스킵 (모든 슬라이드를 건너뛰고 바로 메인 씬으로)
    /// </summary>
    public void SkipCutscene()
    {
        if (!isTransitioning)
        {
            Debug.Log("컷씬을 스킵합니다.");
            StartCoroutine(GoToMainScene());
        }
    }

    /// <summary>
    /// 특정 슬라이드로 점프 (디버그용)
    /// </summary>
    public void JumpToSlide(int slideIndex)
    {
        if (slideIndex >= 0 && slideIndex < totalSlides && !isTransitioning)
        {
            currentSlideIndex = slideIndex;
            ShowCurrentSlide();
            Debug.Log($"슬라이드 {slideIndex + 1}로 점프했습니다.");
        }
    }

    /// <summary>
    /// 현재 진행 상황 반환 (0.0 ~ 1.0)
    /// </summary>
    public float GetProgress()
    {
        if (totalSlides == 0) return 1f;
        return (float)currentSlideIndex / totalSlides;
    }

    /// <summary>
    /// 인스펙터에서 컷씬 스프라이트 배열 검증
    /// </summary>
    private void OnValidate()
    {
        if (cutsceneSprites == null || cutsceneSprites.Length == 0)
        {
            Debug.LogWarning("컷씬 스프라이트 배열이 비어있습니다!");
            return;
        }

        // null 스프라이트 개수 확인
        int nullCount = 0;
        for (int i = 0; i < cutsceneSprites.Length; i++)
        {
            if (cutsceneSprites[i] == null)
            {
                nullCount++;
            }
        }

        if (nullCount > 0)
        {
            Debug.LogWarning($"총 {cutsceneSprites.Length}개 슬라이드 중 {nullCount}개가 비어있습니다!");
        }

        // 유효한 슬라이드 개수 표시
        int validCount = cutsceneSprites.Length - nullCount;
        if (validCount > 0)
        {
            Debug.Log($"컷씬 설정 완료: {validCount}개의 유효한 슬라이드");
        }
    }

    /// <summary>
    /// 디버그 정보 표시
    /// </summary>
    private void OnGUI()
    {
        if (Application.isEditor)
        {
            GUILayout.BeginArea(new Rect(10, 10, 200, 100));
            GUILayout.Label($"슬라이드: {currentSlideIndex + 1}/{totalSlides}");
            GUILayout.Label($"진행률: {GetProgress() * 100:F1}%");
            GUILayout.Label($"전환 중: {isTransitioning}");
            GUILayout.Label($"입력 가능: {canProgress}");
            GUILayout.EndArea();
        }
    }
}
