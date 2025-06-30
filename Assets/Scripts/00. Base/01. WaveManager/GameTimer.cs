using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public float RemainingTime { get; private set; } = 0f;
    public bool IsRunning { get; private set; } = false;

    void Update()
    {
        // 게임 상태가 Playing이 아니면 Update 수행하지 않음
        if (GameManager.Instance.CurrentGameState != EGameState.Playing) return;

        if (!IsRunning) return;

        RemainingTime -= Time.deltaTime;
        if (RemainingTime <= 0f)
        {
            RemainingTime = 0f;
            IsRunning = false;
        }
    }

    public void StartTimer(float duration)
    {
        RemainingTime = duration;
        IsRunning = true;
    }
}
