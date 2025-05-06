using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 몬스터 스폰, 웨이브 타이머, 승패 판정 등을 담당하는 매니저
/// </summary>
public class WaveManager : MonoBehaviour
{
    [SerializeField] private GameObject resultPopup;
    [SerializeField] private Button returnToTitleButton;

    [Header("웨이브 설정")]
    public int totalWaves = 4;
    public float waveDuration = 20f;
    public int maxEnemyThreshold = 10;
    public float checkInterval = 1f;

    [Header("참조")]
    public MonsterSpawner spawner;
    public GameTimer timer;
    public PlayerController player;
    public Text waveTextUI;

    private int currentWave = 0;
    private bool isWaveRunning = false;

    void Start()
    {
        // 필수 참조 자동 연결
        if (spawner == null) spawner = FindFirstObjectByType<MonsterSpawner>();
        if (timer == null) timer = FindFirstObjectByType<GameTimer>();
        if (player == null) player = FindFirstObjectByType<PlayerController>();

        if (resultPopup != null)
            resultPopup.SetActive(false);
        if (returnToTitleButton != null)
            returnToTitleButton.onClick.AddListener(OnReturnToTitleButtonClicked);

        GameManager.Instance.CurrentGameState = EGameState.Wave;
        StartNextWave();
    }

    void StartNextWave()
    {
        if (currentWave >= totalWaves)
        {
            Debug.Log("[WaveManager] 모든 웨이브 완료!");
            return;
        }

        Debug.Log($"[WaveManager] 웨이브 {currentWave + 1} 시작!");

        if (waveTextUI != null)
            waveTextUI.text = $"{currentWave + 1}";

        timer.StartTimer(waveDuration);
        spawner.SpawnWave(currentWave, waveDuration);

        isWaveRunning = true;
        InvokeRepeating(nameof(CheckWaveState), 1f, checkInterval);
    }

    void CheckWaveState()
    {
        if (player.CurrentHealth <= 0)
        {
            EndWave(false);
            return;
        }

        int monsterCount = 0;
        var monsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);
        foreach (var monster in monsters)
        {
            if (monster.isClone)
                monsterCount++;
        }

        if (monsterCount > maxEnemyThreshold)
        {
            EndWave(false);
            return;
        }

        if (!timer.IsRunning)
        {
            EndWave(monsterCount <= maxEnemyThreshold);
        }
    }

    void EndWave(bool success)
    {
        CancelInvoke(nameof(CheckWaveState));

        if (success)
        {
            Debug.Log("[WaveManager] 웨이브 성공");
            currentWave++;
            StartNextWave();
        }
        else
        {
            Debug.LogError("[WaveManager] 게임 오버!");
            isWaveRunning = false;

            GameManager.Instance.CurrentGameState = EGameState.Finish;

            var monsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);
            foreach (var monster in monsters)
            {
                if (monster.isClone)
                    Destroy(monster.gameObject);
            }

            ShowResultPopup();
        }
    }

    private void ShowResultPopup()
    {
        if (resultPopup != null && !resultPopup.activeSelf)
        {
            resultPopup.SetActive(true);
        }
    }

    public void OnReturnToTitleButtonClicked()
    {
        GameManager.Instance.CurrentGameState = EGameState.Title;
        SceneLoader.LoadScene(SceneLoader.SceneName.Title);
    }
}
