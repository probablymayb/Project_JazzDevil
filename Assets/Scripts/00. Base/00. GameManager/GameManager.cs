using System;
using UnityEditor;
using UnityEngine;

public enum EGameState
{
    Title,      // 타이틀
    Playing,    // in-game 상태
    Paused,     // 일시 정지
    Finish      // 게임 끝
};

public class GameManager : Singleton<GameManager>
{
    //[SerializeField] GameData gameData;
    //public event Action<EGameState, EGameState> OnStartGameStateChange;
    //public event Action<EGameState, EGameState> OnFinishGameStateChange;
    // public event Action OnGameOver;
    // public event Action OnBossAppear; //Boss등장
    // public event Action OnGamePause; //for Intro

    /// <summary>
    /// 현재 게임 상태
    /// </summary>
    /// <returns>현재 게임 상태 EGameState Enum 값</returns>
    public EGameState CurrentGameState { get; private set; } = EGameState.Title;

    // public static event Action<EGameState> OnGameStateChange;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ChangeState(EGameState.Title);
    }

    /// <summary>
    /// 게임 상태를 변경
    /// </summary>
    /// <param name="newState">새로 변경할 EGameState Enum 값</param>
    public void ChangeState(EGameState newState)
    {
        if (CurrentGameState == newState) return;

        CurrentGameState = newState;

        // 새로 변경 된 값에 따라 핸들링해줄 메서드를 호출
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

    /// <summary>
    /// 게임 상태가 Playing으로 전환될 때 수행
    /// </summary>
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

    /// <summary>
    /// 게임 상태가 Pause로 전환될 때 수행
    /// </summary>
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

    /// <summary>
    /// 게임 상태가 Finish로 전환될 때 수행
    /// </summary>
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
