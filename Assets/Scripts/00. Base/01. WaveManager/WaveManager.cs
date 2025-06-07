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
    public int totalWaves = 6;                 // 총 웨이브 수
    public float waveDuration = 20f;           // 각 웨이브 유지 시간
    public int maxEnemyThreshold = 20;         // 실패 조건: 몬스터 수 초과 시 패배
    public float checkInterval = 1f;           // 상태 체크 주기
    
    [Header("참조")]
    public MonsterSpawner spawner;             // 몬스터 스포너 참조
    public GameTimer timer;                    // 타이머 참조
    public PlayerController player;            // 플레이어 참조
    public Text waveTextUI;                    // current Stage 참조
    public ShopManager shopManager;            // shopManager 참조
    public NoteSpawner noteSpawner;            // noteSpawner 참조


    public int currentWave = 0;
    public bool isWaveRunning = false;

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
            EndWave(false);
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
            if (shopManager != null)
            {
                shopManager.SpawnShopTrigger();
            }
            StartNextWave();
        }
        else
        {
            Debug.LogError("[WaveManager] 게임 오버!");
            isWaveRunning = false;

            GameManager.Instance.CurrentGameState = EGameState.Finish;

            // 몬스터 스폰 중단
            if (spawner != null)
                spawner.StopCurrentWave();
            // 생성되어 있는 몬스터 제거
            var monsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);
            foreach (var monster in monsters)
            {
                if (monster.isClone)
                    Destroy(monster.gameObject);
            }
            //노트 생성 중단
            if (noteSpawner != null)
                noteSpawner.StopSpawningNotes();

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
