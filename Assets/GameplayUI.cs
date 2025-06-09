using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [Header("UI 컴포넌트")]
    [SerializeField] private Text bpmText;
    [SerializeField] private Text comboText;

    [Header("BPM 색상 설정")]
    [SerializeField] private Color lowBPMColor = Color.cyan;      // 120 BPM (차가운 색)
    [SerializeField] private Color highBPMColor = Color.red;     // 160 BPM (뜨거운 색)

    [Header("콤보 색상")]
    [SerializeField] private Color defaultComboColor = Color.white;
    [SerializeField] private Color highComboColor = Color.yellow;  // 높은 콤보시
    [SerializeField] private int highComboThreshold = 50;         // 높은 콤보 기준

    private void Start()
    {
        // 매니저들의 이벤트 구독
        SubscribeToEvents();

        // 초기 UI 설정
        UpdateBPMDisplay(RhythmManager.Instance.CurrentBpm);
        UpdateComboDisplay(ComboManager.Instance.CurrentCombo);
    }

    private void SubscribeToEvents()
    {
        // RhythmManager 이벤트 구독
        if (RhythmManager.Instance != null)
        {
            RhythmManager.Instance.OnBPMChanged += OnBPMChanged;
        }

        // ComboManager 이벤트 구독
        if (ComboManager.Instance != null)
        {
            ComboManager.Instance.OnComboChanged += OnComboChanged;
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (RhythmManager.Instance != null)
        {
            RhythmManager.Instance.OnBPMChanged -= OnBPMChanged;
        }

        if (ComboManager.Instance != null)
        {
            ComboManager.Instance.OnComboChanged -= OnComboChanged;
        }
    }

    // BPM 변경 이벤트 처리
    private void OnBPMChanged(float oldBPM, float newBPM)
    {
        UpdateBPMDisplay(newBPM);
    }

    // 콤보 변경 이벤트 처리
    private void OnComboChanged(int newCombo)
    {
        UpdateComboDisplay(newCombo);
    }

    // BPM 표시 업데이트
    private void UpdateBPMDisplay(float bpm)
    {
        if (bpmText == null) return;

        // BPM 텍스트 업데이트
        bpmText.text = $"BPM: {bpm:F0}";

        // BPM에 따른 색상 계산 (120-160 범위)
        float normalizedBPM = Mathf.InverseLerp(120f, 160f, bpm);
        Color currentColor = Color.Lerp(lowBPMColor, highBPMColor, normalizedBPM);
        bpmText.color = currentColor;
    }

    // 콤보 표시 업데이트
    private void UpdateComboDisplay(int combo)
    {
        if (comboText == null) return;

        // 콤보 텍스트 업데이트
        if (combo > 0)
        {
            comboText.text = $"COMBO: {combo}";

            // 높은 콤보시 색상 변경
            comboText.color = combo >= highComboThreshold ? highComboColor : defaultComboColor;
        }
        else
        {
            comboText.text = "COMBO: 0";
            comboText.color = defaultComboColor;
        }
    }

    // 수동으로 UI 업데이트 (디버그용)
    [ContextMenu("Update UI")]
    public void ForceUpdateUI()
    {
        if (RhythmManager.Instance != null)
            UpdateBPMDisplay(RhythmManager.Instance.CurrentBpm);

        if (ComboManager.Instance != null)
            UpdateComboDisplay(ComboManager.Instance.CurrentCombo);
    }
}
