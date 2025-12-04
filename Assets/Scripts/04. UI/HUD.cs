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

    [Header("몬스터 카운트 경고 설정")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField] private float warningBlinkSpeed = 0.5f;

    // 타이틀 화면용 깜빡임
    private Coroutine blinkCoroutine;
    private bool isBlinking = false;

    // 몬스터 카운트 경고용 깜빡임
    private Coroutine monsterWarningCoroutine;
    private bool isMonsterWarning = false;

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
                    
                    int maxCount = waveManager.maxEnemyThreshold;
                    myText.text = $"{cloneCount}ㅡ{maxCount}";

                    float ratio = cloneCount / (float)maxCount;
                    
                    if (ratio >= 0.8f)
                    {
                        // ✅ 80% 이상: 빨간색 깜빡임 시작
                        if (!isMonsterWarning)
                        {
                            monsterWarningCoroutine = StartCoroutine(BlinkMonsterWarning());
                            isMonsterWarning = true;
                        }
                    }
                    else
                    {
                        // ✅ 80% 미만: 깜빡임 중지 및 원래 색상 복구
                        if (isMonsterWarning)
                        {
                            StopCoroutine(monsterWarningCoroutine);
                            isMonsterWarning = false;
                            
                            var c = normalColor;
                            c.a = 1f;
                            myText.color = c;
                        }
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 타이틀 화면용 깜빡임 (기존)
    /// </summary>
    private System.Collections.IEnumerator BlinkText()
    {
        while (true)
        {
            var c = myText.color;
            c.a = 0.2f;
            myText.color = c;
            yield return new WaitForSeconds(0.5f);
            
            c.a = 1f;
            myText.color = c;
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// 몬스터 카운트 경고용 빨간색 깜빡임 (새로 추가)
    /// </summary>
    private System.Collections.IEnumerator BlinkMonsterWarning()
    {
        while (true)
        {
            // 빨간색 반투명
            var c = warningColor;
            c.a = 0.3f;
            myText.color = c;
            yield return new WaitForSeconds(warningBlinkSpeed);
            
            // 빨간색 불투명
            c.a = 1f;
            myText.color = c;
            yield return new WaitForSeconds(warningBlinkSpeed);
        }
    }

    /// <summary>
    /// 타이틀 화면에서 깜빡임 시작 (외부 호출용)
    /// </summary>
    public void StartTitleBlink()
    {
        if (!isBlinking && myText != null)
        {
            blinkCoroutine = StartCoroutine(BlinkText());
            isBlinking = true;
        }
    }

    /// <summary>
    /// 타이틀 화면 깜빡임 중지 (외부 호출용)
    /// </summary>
    public void StopTitleBlink()
    {
        if (isBlinking && blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            isBlinking = false;
            
            if (myText != null)
            {
                var c = myText.color;
                c.a = 1f;
                myText.color = c;
            }
        }
    }

    private void OnDisable()
    {
        // 비활성화 시 모든 코루틴 정리
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            isBlinking = false;
        }
        
        if (monsterWarningCoroutine != null)
        {
            StopCoroutine(monsterWarningCoroutine);
            isMonsterWarning = false;
        }
    }
}
