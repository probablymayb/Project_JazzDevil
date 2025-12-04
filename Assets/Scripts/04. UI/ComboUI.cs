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

    [Header("콤보 색상")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color bronzeColor = new Color(0.8f, 0.5f, 0.2f);    // 10+
    [SerializeField] private Color silverColor = new Color(0.75f, 0.75f, 0.8f);  // 20+
    [SerializeField] private Color goldColor = new Color(1f, 0.84f, 0f);         // 30+
    [SerializeField] private Color rainbowColor = new Color(1f, 0.4f, 0.7f);     // 50+

    private void Start()
    {
        if (ComboManager.Instance != null)
        {
            ComboManager.Instance.OnComboChanged += UpdateComboUI;
            ComboManager.Instance.OnComboBreak += OnComboBreak;
        }

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
            comboText.text = $"{combo} combo";
            comboText.color = GetComboColor(combo);
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

    private Color GetComboColor(int combo)
    {
        if (combo >= 50) return rainbowColor;
        if (combo >= 30) return goldColor;
        if (combo >= 20) return silverColor;
        if (combo >= 10) return bronzeColor;
        return defaultColor;
    }

    private void OnComboBreak(int lastCombo)
    {
        comboText.DOFade(0f, 0.3f).OnComplete(() =>
        {
            comboText.gameObject.SetActive(false);
            comboText.color = new Color(comboText.color.r, comboText.color.g, comboText.color.b, 1f);
        });
    }
}
