using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        PauseOrReturn();
    }

    public void PauseOrReturn()
    {
        if (GameManager.Instance.CurrentGameState == EGameState.Playing)
        {
            pauseUI.SetActive(true);
            GameManager.Instance.ChangeState(EGameState.Paused);
        }
        else
        {
            pauseUI.SetActive(false);
            GameManager.Instance.ChangeState(EGameState.Playing);
        }
    }
}
