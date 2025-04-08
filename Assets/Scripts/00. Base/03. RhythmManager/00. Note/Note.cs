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

    private void Awake()
    {
        noteImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        // 시작할 때 이미지가 보이도록 알파값 설정
        Color startColor = noteImage.color;
        startColor.a = 1f;
        noteImage.color = startColor;

        // 시작 크기 설정
        rectTransform.localScale = Vector3.one * startScale;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        float progress = currentTime / shrinkDuration;

        // 크기가 startScale에서 endScale로 줄어들도록 수정
        float currentScale = Mathf.Lerp(startScale, endScale, progress);
        rectTransform.localScale = Vector3.one * currentScale;

        // 알파값 변경 제거 (계속 보이도록)

        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }
}