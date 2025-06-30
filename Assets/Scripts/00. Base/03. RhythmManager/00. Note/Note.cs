using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    private Image noteImage;
    [SerializeField] private float shrinkDuration = 1f;
    [SerializeField] private float startScale = 3f;
    [SerializeField] private float endScale = 0.2f;

    private float currentTime = 0f;
    private RectTransform rectTransform;

    [HideInInspector] public GameObject poolPrefabRef; // 풀 반환용 프리팹 참조

    private void Awake()
    {
        noteImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        // 타이머 초기화
        currentTime = 0f;

        // 시작할 때 이미지가 보이도록 알파값 설정
        Color startColor = noteImage.color;
        startColor.a = 1f;
        noteImage.color = startColor;

        // 시작 크기 설정
        rectTransform.localScale = Vector3.one * startScale;
    }

    private void Update()
    {
        // 게임 상태가 Playing이 아니면 Update 수행하지 않음
        if (GameManager.Instance.CurrentGameState != EGameState.Playing) return;

        currentTime += Time.deltaTime;
        float progress = currentTime / shrinkDuration;

        // 크기가 startScale에서 endScale로 줄어들도록 수정
        float currentScale = Mathf.Lerp(startScale, endScale, progress);
        rectTransform.localScale = Vector3.one * currentScale;

        // 알파값 변경 제거 (계속 보이도록)

        if (progress >= 1f)
        {
            // 풀로 반환
            PoolManager.Instance.Return(poolPrefabRef, transform.parent.gameObject);
        }
    }
}
