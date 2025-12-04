using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ComboUI : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private Text comboText;

    [Header("애니메이션 설정")]
    [SerializeField] private float punchScale = 1.3f;
    [SerializeField] private float punchDuration = 0.15f;

    private void Start()
    {
        if (ComboManager.Instance != null)
        {
            ComboManager.Instance.OnComboChanged += UpdateComboUI;
            ComboManager.Instance.OnComboBreak += OnComboBreak;
        }

        // 초기 상태
        UpdateComboUI(0);
    }

    private void OnDestroy()
    {
        if (ComboManager.Instance != null)
        {
            ComboManager.Instance.OnComboChanged -= UpdateComboUI;
            ComboManager.Instance.OnComboBreak -= OnComboBreak;
        }
    }

    private void UpdateComboUI(int combo)
    {
        if (combo > 0)
        {
            comboText.text = $"{combo}";
            comboText.gameObject.SetActive(true);

            // 펀치 애니메이션
            comboText.transform.DOKill();
            comboText.transform.localScale = Vector3.one;
            comboText.transform.DOPunchScale(Vector3.one * (punchScale - 1f), punchDuration);
        }
        else
        {
            comboText.gameObject.SetActive(false);
        }
    }

    private void OnComboBreak(int lastCombo)
    {
        // 콤보 끊겼을 때 페이드아웃
        comboText.DOFade(0f, 0.3f).OnComplete(() =>
        {
            comboText.gameObject.SetActive(false);
            comboText.color = new Color(comboText.color.r, comboText.color.g, comboText.color.b, 1f);
        });
    }
}
