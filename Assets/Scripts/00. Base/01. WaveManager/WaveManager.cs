using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

/// <summary>
/// 몬스터 스폰, 웨이브 타이머, 승패 판정 등을 담당하는 매니저
/// </summary>
public class WaveManager : MonoBehaviour
{
    [SerializeField] private GameObject resultPopup;
    [SerializeField] private GameObject darkOverlay;
    [SerializeField] private Button returnToTitleButton;
    [SerializeField] private GameObject gameOverTextObj;
    [SerializeField] private GameObject clearTextObj;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float delayBeforeResult = 1f;

    [Header("웨이브 설정")]
    public int totalWaves = 12;                // 총 웨이브 수
    public float waveDuration = 30f;           // 각 웨이브 유지 시간
    public int maxEnemyThreshold = 50;         // 실패 조건: 몬스터 수 초과 시 패배
    public float checkInterval = 1f;           // 상태 체크 주기
    
    [Header("참조")]
    public MonsterSpawner spawner;             // 몬스터 스포너 참조
    public GameTimer timer;                    // 타이머 참조
    public PlayerController player;            // 플레이어 참조
    public Text waveTextUI;                    // current Stage 참조
    public ShopManager shopManager;            // shopManager 참조
    public NoteSpawner noteSpawner;            // noteSpawner 참조

    [Header("결과창 변수")]
    public Text txtAccuracy;
    public Text txtWave;
    public Text txtKills;
    public Text txtMaxCombo;
    public Text txtRank;

    public int currentWave { get; private set; } = 0; // ✅ 프로퍼티로 변경 (외부 읽기 가능)
    public bool isWaveRunning { get; private set; } = false;

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

        StartNextWave();
    }

    void StartNextWave()
    {
        currentWave++; // ✅ 웨이브 증가 (1부터 시작)
        
        if (currentWave > totalWaves)
        {
            // 모든 웨이브 클리어
            GameManager.Instance.ChangeState(EGameState.Finish);
            ShowClearAndResult();
            return;
        }

        Debug.Log($"[WaveManager] 웨이브 {currentWave} 시작!");

        if (waveTextUI != null)
            waveTextUI.text = $"{currentWave}";

        timer.StartTimer(waveDuration);
        spawner.SpawnWave(waveDuration); // ✅ duration만 전달 (spawner가 currentWave 직접 참조)

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
        isWaveRunning = false;

        if (success)
        {
            Debug.Log($"[WaveManager] 웨이브 {currentWave} 성공");
            
            if (currentWave >= totalWaves)
            {
                // 모든 웨이브 클리어
                GameManager.Instance.ChangeState(EGameState.Finish);
                ShowClearAndResult();
            }
            else
            {
                // 다음 웨이브로
                if (shopManager != null)
                    shopManager.SpawnShopTrigger();
                StartNextWave();
            }
        }
        else
        {
            Debug.LogError("[WaveManager] 게임 오버!");
            GameManager.Instance.ChangeState(EGameState.Finish);

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
            
            // 노트 생성 중단
            if (noteSpawner != null)
                noteSpawner.StopSpawningNotes();

            ShowGameOverAndResult();
        }
    }

    private void ShowClearAndResult()
    {
        StartCoroutine(ClearRoutine());
    }

    private IEnumerator ClearRoutine()
    {
        if (darkOverlay != null)
        {
            darkOverlay.SetActive(true);
            var overlayImage = darkOverlay.GetComponent<Image>();
            if (overlayImage != null)
            {
                var color = overlayImage.color;
                color.a = 0f;
                overlayImage.color = color;
                overlayImage.DOFade(0.8f, 0.4f);
            }
        }

        if (clearTextObj != null)
        {
            clearTextObj.SetActive(true);
            var text = clearTextObj.GetComponent<Text>();
            var color = text.color;
            color.a = 0;
            text.color = color;

            Tween fadeIn = text.DOFade(1f, fadeDuration);
            yield return fadeIn.WaitForCompletion();

            yield return new WaitForSeconds(0.6f);

            Tween fadeOut = text.DOFade(0f, fadeDuration);
            yield return fadeOut.WaitForCompletion();

            clearTextObj.SetActive(false);
        }

        yield return new WaitForSeconds(0.3f);
        ShowResultPopup();
    }

    private void ShowGameOverAndResult()
    {
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        if (darkOverlay != null)
        {
            darkOverlay.SetActive(true);

            var overlayImage = darkOverlay.GetComponent<Image>();
            if (overlayImage != null)
            {
                var color = overlayImage.color;
                color.a = 0f;
                overlayImage.color = color;
                overlayImage.DOFade(0.8f, 0.4f);
            }
        }

        if (gameOverTextObj != null)
        {
            gameOverTextObj.SetActive(true);
            var text = gameOverTextObj.GetComponent<Text>();

            var color = text.color;
            color.a = 0;
            text.color = color;

            Tween fadeIn = text.DOFade(1f, fadeDuration);
            yield return fadeIn.WaitForCompletion();

            yield return new WaitForSeconds(0.6f);

            Tween fadeOut = text.DOFade(0f, fadeDuration);
            yield return fadeOut.WaitForCompletion();

            gameOverTextObj.SetActive(false);
        }

        yield return new WaitForSeconds(0.3f);
        ShowResultPopup();
    }

    private void ShowResultPopup()
    {
        if (resultPopup != null && !resultPopup.activeSelf)
        {
            float accuracy = NoteJudge.Instance.Accuracy;
            int wave = currentWave;
            int kills = player.killCount;
            int maxCombo = ComboManager.Instance.MaxComboThisSession;

            string rank = CalculateRank(accuracy, wave, kills, maxCombo);

            txtAccuracy.text = $"Accuracy: {accuracy:F1}%";
            txtWave.text = $"Wave: {wave}";
            txtKills.text = $"Kills: {kills}";
            txtMaxCombo.text = $"MaxCombo: {maxCombo}";
            txtRank.text = $"{rank}";

            resultPopup.SetActive(true);
        }
    }

    string CalculateRank(float accuracy, int wave, int kills, int maxCombo)
    {
        float score = accuracy * 0.4f + wave * 10f * 0.2f + kills * 0.2f + maxCombo * 0.2f;
        if (score >= 90) return "S";
        if (score >= 80) return "A";
        if (score >= 65) return "B";
        if (score >= 50) return "C";
        return "D";
    }

    public void OnReturnToTitleButtonClicked()
    {
        if (darkOverlay != null)
            darkOverlay.SetActive(false);
        if (gameOverTextObj != null)
            gameOverTextObj.SetActive(false);
        if (clearTextObj != null)
            clearTextObj.SetActive(false);

        GameManager.Instance.ChangeState(EGameState.Title);
        SceneLoader.LoadScene(SceneLoader.SceneName.Title);
    }
}
