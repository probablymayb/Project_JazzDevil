using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Health, Timer, MonsterCount, Gold }

    [Header("설정")]
    public InfoType type;

    [Header("UI 컴포넌트")]
    public Text myText;
    public Image myImage;

    [Header("게임 참조")]
    public PlayerController player;
    public GameTimer timer;
    public WaveManager waveManager;

    void LateUpdate()
    {
        if (player == null) return;

        switch (type)
        {
            case InfoType.Health:
                if (myImage != null && myImage.type == Image.Type.Filled)
                {
                    float ratio = player.CurrentHealth / (float)player.MaxHealth;
                    myImage.fillAmount = ratio;
                }
                break;

            case InfoType.Gold:
                if (myText != null)
                {
                    myText.text = $"{player.Gold}";
                }
                break;

            case InfoType.Timer:
                if (timer != null)
                {
                    if (myText != null)
                    {
                        float t = timer.RemainingTime;
                        int min = Mathf.FloorToInt(t / 60);
                        int sec = Mathf.FloorToInt(t % 60);
                        myText.text = $"{min:D2}:{sec:D2}";
                    }
                    else if (myImage != null && myImage.type == Image.Type.Filled)
                    {
                        float ratio = (timer.RemainingTime > 0f) ? (timer.RemainingTime / waveManager.waveDuration) : 0f;
                        myImage.fillAmount = Mathf.Clamp01(ratio);
                    }
                }
                break;

            case InfoType.MonsterCount:
                if (myText != null)
                {
                    int cloneCount = 0;
                    var monsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);
                    foreach (var monster in monsters)
                    {
                        if (monster.isClone) cloneCount++;
                    }
                    myText.text = $"{cloneCount}";
                }
                break;
        }
    }
}
