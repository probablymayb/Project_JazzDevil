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
    }

    private void OnBeat()
    {
        beatCount++;

        // DOTween 애니메이션 설정
        transform.DOKill(); // 기존 Tween 중복 방지

        if (beatCount % 2 == 0)
        {
            // 위로 늘어났다 돌아오기 (Y축)
            transform.DOScale(new Vector3(originalScale.x, originalScale.y * 1.2f, originalScale.z), 0.1f)
                     .SetEase(Ease.OutQuad)
                     .OnComplete(() =>
                     {
                         transform.DOScale(originalScale, 0.2f).SetEase(Ease.InOutQuad);
                     });
        }
        else
        {
            // 옆으로 늘어났다 돌아오기 (X축)
            transform.DOScale(new Vector3(originalScale.x * 1.2f, originalScale.y, originalScale.z), 0.1f)
                     .SetEase(Ease.OutQuad)
                     .OnComplete(() =>
                     {
                         transform.DOScale(originalScale, 0.2f).SetEase(Ease.InOutQuad);
                     });
        }
    }
}
