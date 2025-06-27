using System;
using UnityEditor;
using UnityEngine;

public enum EGameState
{
    Title,
    Playing,
    Paused,
    Finish
};

//test
public class GameManager : Singleton<GameManager>
{
    //[SerializeField] GameData gameData;
    public EGameState CurrentGameState { get; private set; } = EGameState.Title;

    public static event Action<EGameState> OnGameStateChange;

    //public event Action<EGameState, EGameState> OnStartGameStateChange;
    //public event Action<EGameState, EGameState> OnFinishGameStateChange;

    // public event Action OnGameOver;
    // public event Action OnBossAppear; //Boss등장
    // public event Action OnGamePause; //for Intro

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ChangeState(EGameState.Title);
    }

    public void ChangeState(EGameState newState)
    {
        if (CurrentGameState == newState) return;

        CurrentGameState = newState;

        switch (newState)
        {
            case EGameState.Playing:
                HandlePlaying();
                break;
            case EGameState.Paused:
                HandlePaused();
                break;
            case EGameState.Finish:
                HandleFinish();
                break;
        }
    }

    private void HandlePlaying()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.ResumeAllLoopingSounds();
        RhythmManager.Instance.CurrentMusicInstance.getPaused(out bool isPaused);
        if (isPaused)
        {
            RhythmManager.Instance.CurrentMusicInstance.setPaused(false);
        }
    }

    private void HandlePaused()
    {
        Time.timeScale = 0f;
        AudioManager.Instance.PauseAllLoopingSounds();
        RhythmManager.Instance.CurrentMusicInstance.getPaused(out bool isPaused);
        if (!isPaused)
        {
            RhythmManager.Instance.CurrentMusicInstance.setPaused(true);
        }
    }

    private void HandleFinish() { /* ... */ }

    // public void GameOver()
    // {
    //     print("GAME OVER");
    //     OnGameOver?.Invoke();

    //     // 엔딩 띄우기

    // }

    // public void GamePause()
    // {
    //     print("GAME Pause");
    //     OnGamePause?.Invoke();
    // }

    //public void NotifyBossAppear()
    //{
    //    print("BOSS APPEAR");
    //    //CurrentGameState = EGameState.BossBattle;
    //    OnBossAppear?.Invoke();
    //}

}
