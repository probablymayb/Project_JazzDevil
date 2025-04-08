using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // 표시할 정보 유형 정의
    public enum InfoType { Health, Timer, MonsterCount, Gold }
    public InfoType type;

    // 텍스트 또는 이미지 컴포넌트 참조
    private Text myText;
    private Image myImage;

    // 플레이어와 타이머 참조
    private PlayerController player;
    private GameTimer timer;

    void Awake()
    {
        // 현재 오브젝트에서 Text 또는 Image 컴포넌트 찾기
        myText = GetComponent<Text>();
        myImage = GetComponent<Image>();

        // 씬에서 PlayerController, GameTimer 찾아서 참조
        player = FindFirstObjectByType<PlayerController>();
        timer = FindFirstObjectByType<GameTimer>();
    }

    void LateUpdate()
    {
        // 플레이어가 없으면 실행하지 않음
        if (player == null) return;

        // 선택한 정보 유형에 따라 UI 갱신
        switch (type)
        {
            case InfoType.Health:
                // 체력 비율 반영(이미지 filled타입)
                if (myImage != null && myImage.type == Image.Type.Filled)
                {
                    float fillAmount = player.CurrentHealth / (float)player.MaxHealth;
                    myImage.fillAmount = fillAmount;
                }
                break;

            case InfoType.Gold:
                // 골드를 텍스트로 표시
                if (myText != null)
                {
                    myText.text = $"{player.Gold}";
                }
                break;

            case InfoType.Timer:
                // 타이머 시간을 [분 초] 형식으로 표시
                if (timer != null && myText != null)
                {
                    float t = timer.RemainingTime;
                    int min = Mathf.FloorToInt(t / 60);
                    int sec = Mathf.FloorToInt(t % 60);
                    myText.text = $"{min:D2}:{sec:D2}";
                }
                break;
            
            case InfoType.MonsterCount:
                if (myText != null)
                {
                    // 오직 복제된 몬스터만 카운트
                    int cloneCount = 0;
                    var monsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);
                    foreach (var monster in monsters)
                    {
                        if (monster.isClone)
                            cloneCount++;
                    }

                    myText.text = $"{cloneCount}";
                }
                break;
        }
    }
}
