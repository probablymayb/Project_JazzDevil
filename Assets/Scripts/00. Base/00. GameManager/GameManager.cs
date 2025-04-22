using System;
using UnityEngine;

    public enum EGameState
    {
        Title,
        Wave,
        Shop,
        Finish
    };

    //test
    public class GameManager : Singleton<GameManager>
    {
        //[SerializeField] GameData gameData;
        public EGameState CurrentGameState { get; set; } = EGameState.Title;

        //public event Action<EGameState, EGameState> OnStartGameStateChange;
        //public event Action<EGameState, EGameState> OnFinishGameStateChange;


        public event Action OnGameOver;
        public event Action OnBossAppear; //Boss등장
        public event Action OnGamePause; //for Intro

        public void GameOver()
        {
            print("GAME OVER");
            OnGameOver?.Invoke();

            // 엔딩 띄우기

        }

        public void GamePause()
        {
            print("GAME Pause");
            OnGamePause?.Invoke();
        }

        //public void NotifyBossAppear()
        //{
        //    print("BOSS APPEAR");
        //    //CurrentGameState = EGameState.BossBattle;
        //    OnBossAppear?.Invoke();
        //}

    }
