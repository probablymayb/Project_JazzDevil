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

    //몬스터 수 깜빡임
    private Coroutine blinkCoroutine;
    private bool isBlinking = false;

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
                if (myText != null && waveManager != null)
                {
                    int cloneCount = 0;
                    var monsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);
                    foreach (var monster in monsters)
                    {
                        if (monster.isClone) cloneCount++;
                    }
                    // 최대치 표시
                    int maxCount = waveManager.maxEnemyThreshold;
                    myText.text = $"{cloneCount}ㅡ{maxCount}";

                    // 깜빡임 여부 판정
                    float ratio = cloneCount / (float)maxCount;
                    if (ratio >= 0.8f)
                    {
                        if (!isBlinking)
                        {
                            blinkCoroutine = StartCoroutine(BlinkText());
                            isBlinking = true;
                        }
                    }
                    else
                    {
                        if (isBlinking)
                        {
                            StopCoroutine(blinkCoroutine);
                            isBlinking = false;
                            // 원래 알파로 복구
                            var c = myText.color;
                            c.a = 1f;
                            myText.color = c;
                        }
                    }
                }
                break;
        }
    }

    // 1초 간격으로 깜빡이는 코루틴
    private System.Collections.IEnumerator BlinkText()
    {
        while (true)
        {
            var c = myText.color;
            // 투명→불투명→투명 반복
            c.a = 0.2f;
            myText.color = c;
            yield return new WaitForSeconds(0.5f);
            c.a = 1f;
            myText.color = c;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
