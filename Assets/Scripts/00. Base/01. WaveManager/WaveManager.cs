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
    public int totalWaves = 6;                 // 총 웨이브 수
    public float waveDuration = 20f;           // 각 웨이브 유지 시간
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

        // GameManager.Instance.ChangeState(EGameState.Playing);
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
            if (currentWave >= totalWaves) {
                // 모든 웨이브 클리어 → 클리어 연출!
                GameManager.Instance.ChangeState(EGameState.Finish);
                ShowClearAndResult();
            } else {
                if (shopManager != null)
                    shopManager.SpawnShopTrigger();
                StartNextWave();
            }
        }
        else
        {
            Debug.LogError("[WaveManager] 게임 오버!");
            isWaveRunning = false;

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
            //노트 생성 중단
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

            // Image 컴포넌트 가져오기
            var overlayImage = darkOverlay.GetComponent<Image>();
            if (overlayImage != null)
            {
                // 알파 0으로 초기화
                var color = overlayImage.color;
                color.a = 0f;
                overlayImage.color = color;

                // DOTween으로 알파값 0 → 0.7로
                overlayImage.DOFade(0.8f, 0.4f);
            }
        }

        if (gameOverTextObj != null)
        {
            gameOverTextObj.SetActive(true);
            var text = gameOverTextObj.GetComponent<Text>();

            // 1. 알파 0으로 초기화
            var color = text.color;
            color.a = 0;
            text.color = color;

            // 2. 페이드인
            Tween fadeIn = text.DOFade(1f, fadeDuration);
            yield return fadeIn.WaitForCompletion();

            yield return new WaitForSeconds(0.6f); // 잠깐 멈춤

            // 3. 페이드아웃
            Tween fadeOut = text.DOFade(0f, fadeDuration);
            yield return fadeOut.WaitForCompletion();

            gameOverTextObj.SetActive(false);
        }

        yield return new WaitForSeconds(0.3f);

        // 4. 결과 팝업 띄우기
        ShowResultPopup();
    }

    private void ShowResultPopup()
    {
        if (resultPopup != null && !resultPopup.activeSelf)
        {
            // 값 참조
            float accuracy = NoteJudge.Instance.Accuracy;
            int wave = currentWave;
            float surviveTime = Time.timeSinceLevelLoad;
            int kills = player.killCount;
            int maxCombo = ComboManager.Instance.MaxComboThisSession;

            // 랭크 계산 (임의 예시)
            string rank = CalculateRank(accuracy, wave, kills, maxCombo);

            // 텍스트 세팅
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
        gameOverTextObj.SetActive(false);
        clearTextObj.SetActive(false);

        GameManager.Instance.ChangeState(EGameState.Title);
        SceneLoader.LoadScene(SceneLoader.SceneName.Title);
    }
}
