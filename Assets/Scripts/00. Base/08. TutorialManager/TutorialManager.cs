using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private GameObject tutorialPopup;

    protected override void Awake()
    {
        base.Awake();
        // PlayerPrefs에 튜토리얼
        if (PlayerPrefs.GetInt("HasSeenTutorial", 0) == 0)
        {
            tutorialPopup.SetActive(true);
            GameManager.Instance.ChangeState(EGameState.Paused);
        }
        else
        {
            tutorialPopup.SetActive(false);
        }
    }

    public void CloseTutorial()
    {
        tutorialPopup.SetActive(false);
        PlayerPrefs.SetInt("HasSeenTutorial", 1);
        PlayerPrefs.Save();
        GameManager.Instance.ChangeState(EGameState.Playing);
    }
}
