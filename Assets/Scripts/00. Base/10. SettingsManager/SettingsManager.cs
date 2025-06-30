using System.IO;
using UnityEngine;

public class SettingsManager : Singleton<SettingsManager>
{
    [SerializeField] private GameSettings currentSettings;
    public GameSettings CurrentSettings => currentSettings;

    private string saveFilePath;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        saveFilePath = Path.Combine(Application.persistentDataPath, "gameSettings.json");
        LoadSettings();
        ApplySettings();
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(currentSettings, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("설정 파일이 다음 경로에 저장됨: " + saveFilePath);
    }

    public void LoadSettings()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            JsonUtility.FromJsonOverwrite(json, currentSettings);
            Debug.Log("설정 파일이 다음으로부터 불러와짐: " + saveFilePath);
        }
        else
        {
            Debug.Log("설정 파일을 찾을 수 없음. 기본 스크립터블오브젝트 설정 값을 사용합니다.");
            SaveSettings();
        }
    }

    public void ApplySettings()
    {
        AudioManager.Instance.SetMasterVolume(currentSettings.masterVolume);
        AudioManager.Instance.SetSFXVolume(currentSettings.sfxVolume);
    }

    public void SetMasterVolume(float value)
    {
        currentSettings.masterVolume = value;
        ApplySettings();
    }

    public void SetSfxVolume(float value)
    {
        currentSettings.sfxVolume = value;
        ApplySettings();
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }
}
