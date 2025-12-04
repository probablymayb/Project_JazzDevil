#define ALWAYS_TUTORIAL

using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private GameObject tutorialPopup;

    protected override void Awake()
    {
        base.Awake();
#if ALWAYS_TUTORIAL
        // 항상 튜토리얼 팝업 띄우기
        tutorialPopup.SetActive(true);

        // 게임 상태를 Paused로 변경
        GameManager.Instance.ChangeState(EGameState.Paused);
#else
        // PlayerPrefs에서 튜토리얼 정보를 불러오기
        if (PlayerPrefs.GetInt("HasSeenTutorial", 0) == 0)
        {
            // 팝업 띄우기
            tutorialPopup.SetActive(true);

            // 게임 상태를 Paused로 변경
            GameManager.Instance.ChangeState(EGameState.Paused);
        }
        else
        {
            // 튜토리얼을 이미 본 상태면 팝업을 띄우지 않는다.
            tutorialPopup.SetActive(false);

            // 게임 상태를 Playing으로 변경
            GameManager.Instance.ChangeState(EGameState.Playing);
        }
#endif
    }

    /// <summary>
    /// 튜토리얼 팝업을 닫습니다.
    /// </summary>
    public void CloseTutorial()
    {
        tutorialPopup.SetActive(false); // 팝업 비활성화
        PlayerPrefs.SetInt("HasSeenTutorial", 1);   // PlayerPrefs에 튜토리얼 정보 저장
        PlayerPrefs.Save(); // PlayerPrefs 저장
        GameManager.Instance.ChangeState(EGameState.Playing);   // 게임 상태를 Playing으로 변경
    }
}
