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

        // ������ �� �̹����� ���̵��� ���İ� ����
        Color startColor = noteImage.color;
        startColor.a = 1f;
        noteImage.color = startColor;

        // ���� ũ�� ����
        rectTransform.localScale = Vector3.one * startScale;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        float progress = currentTime / shrinkDuration;

        // ũ�Ⱑ startScale���� endScale�� �پ�鵵�� ����
        float currentScale = Mathf.Lerp(startScale, endScale, progress);
        rectTransform.localScale = Vector3.one * currentScale;

        // ���İ� ���� ���� (��� ���̵���)

        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }
}