using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;

    [Header("Fade Durations")]
    public float fadeInDuration = 1.5f;    // 화면 밝아지는 시간
    public float fadeOutDuration = 1f;   // 화면 어두워지는 시간

    void Start()
    {
        // 시작 시 페이드 인
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 1);
            StartCoroutine(FadeIn());
        }
    }

    public IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            float alpha = 1f - (t / fadeInDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0); // 완전 투명
    }

    public IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeOutDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1); // 완전 검정
    }
}
