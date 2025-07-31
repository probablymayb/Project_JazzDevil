using UnityEngine;
using DG.Tweening;

public class ShopBouncer : MonoBehaviour
{
    private Vector3 originalScale;
    private int beatCount = 0;

    private void Start()
    {
        originalScale = transform.localScale;
        RhythmManager.beatUpdated += OnBeat;
    }

    private void OnDestroy()
    {
        RhythmManager.beatUpdated -= OnBeat;
        DOTween.Kill(transform);
    }

    private void OnBeat()
    {
        transform.DOKill();

        // z는 원래 값, x/y만 1.2배로
        Vector3 targetScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 1.2f, originalScale.z);

        transform.DOScale(targetScale, 0.1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                transform.DOScale(originalScale, 0.2f).SetEase(Ease.InOutQuad);
            });
    }
}
