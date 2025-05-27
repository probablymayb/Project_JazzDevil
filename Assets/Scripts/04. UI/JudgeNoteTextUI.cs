using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class JudgeNoteTextUI : MonoBehaviour
{
    public Text judgeText;
    public float showTime = 0.7f;

    private Sequence seq;

    void Start()
    {
        var c = judgeText.color;
        c.a = 0;
        judgeText.color = c;
        judgeText.text = "";
    }

    public void ShowJudge(string result)
    {
        if (seq != null) seq.Kill();

        judgeText.text = result;

        // 판정별 색상
        switch (result)
        {
            case "Perfect":
                judgeText.color = new Color(1f, 0.92f, 0.016f, 1f); // 노랑
                break;
            case "Good":
                judgeText.color = new Color(0.3f, 1f, 0.3f, 1f); // 연두
                break;
            case "Miss":
                judgeText.color = new Color(1f, 0.2f, 0.2f, 1f); // 빨강
                break;
            default:
                judgeText.color = Color.white;
                break;
        }

        // 애니메이션 준비
        var c = judgeText.color;
        c.a = 1;
        judgeText.color = c;
        judgeText.transform.localScale = Vector3.one * 1.5f;

        seq = DOTween.Sequence();
        seq.Append(judgeText.transform.DOScale(1.0f, 0.2f).SetEase(Ease.OutBack))
           .Join(judgeText.DOFade(1, 0.2f))
           .AppendInterval(showTime)
           .Append(judgeText.DOFade(0, 0.3f))
           .OnComplete(() => judgeText.text = "");
    }
}
