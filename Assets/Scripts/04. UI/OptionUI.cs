using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    public enum SettingType { MasterVolume, SfxVolume }

    [Header("설정 타입")]
    public SettingType settingType;

    [Header("컴포넌트")]
    [SerializeField] private Slider slider;
    [SerializeField] private Text percentText;

    private void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
        if (percentText == null) percentText = GetComponent<Text>();
    }

    private void OnEnable()
    {
        UpdateUI();

        if (slider != null)
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    private void OnDisable()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }

    private void UpdateUI()
    {
        if (SettingsManager.Instance == null) return;

        GameSettings settings = SettingsManager.Instance.CurrentSettings;

        switch (settingType)
        {
            case SettingType.MasterVolume:
                if (slider != null) slider.value = settings.masterVolume;
                if (percentText != null) percentText.text = $"{settings.masterVolume * 100:F0}%";
                break;
            case SettingType.SfxVolume:
                if (slider != null) slider.value = settings.sfxVolume;
                if (percentText != null) percentText.text = $"{settings.sfxVolume * 100:F0}%";
                break;
        }
    }

    private void OnSliderValueChanged(float value)
    {
        if (SettingsManager.Instance == null) return;

        switch (settingType)
        {
            case SettingType.MasterVolume:
                SettingsManager.Instance.SetMasterVolume(value);
                break;
            case SettingType.SfxVolume:
                SettingsManager.Instance.SetSfxVolume(value);
                break;
        }
        UpdateUI();
    }
}
