using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private GameObject shopPopup;              // 인스펙터에서 ShopPopup 연결
    [SerializeField] private Button startNextWaveButton;        // 버튼 참조
    [SerializeField] private GameObject resultPopup;            // 인스펙터에서 DefeatPopup 연결
    [SerializeField] private Button returnToTitleButton;        // 타이틀 복귀 버튼 참조

    [Header("웨이브 설정")]
    public int totalWaves = 4;                 // 총 웨이브 수
    public float waveDuration = 20f;           // 각 웨이브 유지 시간
    public int maxEnemyThreshold = 10;         // 실패 조건: 몬스터 수 초과 시 패배
    public float checkInterval = 1f;           // 상태 체크 주기
    
    [Header("참조")]
    public MonsterSpawner spawner;             // 몬스터 스포너 참조
    public GameTimer timer;                    // 타이머 참조
    public PlayerController player;            // 플레이어 참조
    public Text waveTextUI;                    // current Stage 참조
    public FieldShopSpawner fieldShopSpawner;  // 필드 상점 스포너 참조

    private int currentWave = 0;               // 현재 진행 중인 웨이브 인덱스
    private bool isWaveRunning = false;        // 웨이브 진행 여부

    void Start()
    {
        // 컴포넌트 자동 연결 (수동 할당도 가능)
        if (spawner == null) spawner = FindFirstObjectByType<MonsterSpawner>();
        if (timer == null) timer = FindFirstObjectByType<GameTimer>();
        if (player == null) player = FindFirstObjectByType<PlayerController>();

        //상점
        if (shopPopup != null)
            shopPopup.SetActive(false); // 게임 시작 시 상점 숨기기
        if (startNextWaveButton != null)
            startNextWaveButton.onClick.AddListener(OnStartNextWaveButtonClicked);  //버튼 이벤트 연결
        
        // 결과창
        if (resultPopup != null)
            resultPopup.SetActive(false); // 게임 시작 시 결과창 숨기기
        if (returnToTitleButton != null)
            returnToTitleButton.onClick.AddListener(OnReturnToTitleButtonClicked); // 버튼 이벤트 연결

        // 첫 웨이브 시작
        GameManager.Instance.CurrentGameState = EGameState.Wave;
        StartNextWave();
    }

    // 다음 웨이브를 시작
    void StartNextWave()
    {
        if (currentWave >= totalWaves)
        {
            Debug.Log("[WaveManager] 모든 웨이브 완료!");
            return;
        }

        Debug.Log($"[WaveManager] 웨이브 {currentWave + 1} 시작!");

        // 텍스트 UI 업데이트
        if (waveTextUI != null)
            waveTextUI.text = $"{currentWave + 1}";

        // 타이머 실행
        timer.StartTimer(waveDuration);

        // 몬스터 스폰 시작
        spawner.SpawnWave(currentWave, waveDuration);

        // 상태 체크 시작
        isWaveRunning = true;
        InvokeRepeating(nameof(CheckWaveState), 1f, checkInterval);
    }

    // 웨이브 상태 검사 (매 checkInterval 초마다 호출됨)
    void CheckWaveState()
    {
        // 1. 플레이어 사망 확인
        if (player.CurrentHealth <= 0)
        {
            Debug.LogWarning("[WaveManager] 플레이어 사망 - 패배!");
            EndWave(false);
            return;
        }

        // 2. 클론 몬스터 수 체크 (원본 제외)
        int monsterCount = 0;
        var monsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);
        foreach (var monster in monsters)
        {
            if (monster.isClone)
                monsterCount++;
        }

        // 3. 몬스터가 한도를 초과했는지 확인
        if (monsterCount > maxEnemyThreshold)
        {
            Debug.LogWarning("[WaveManager] 몬스터 과잉 - 패배!");
            EndWave(false);
            return;
        }

        // 3. 타이머가 끝났고 몬스터가 적으면 성공 처리
        if (!timer.IsRunning)
        {
            if (monsterCount <= maxEnemyThreshold)
            {
                Debug.Log($"[WaveManager] 웨이브 {currentWave + 1} 성공!");
                EndWave(true);
            }
            else
            {
                Debug.LogWarning("[WaveManager] 몬스터 너무 많아서 실패!");
                EndWave(false);
            }
        }
    }

    // 웨이브 종료 처리
    void EndWave(bool success)
    {
        CancelInvoke(nameof(CheckWaveState));
        isWaveRunning = false;

        if (success)
        {
            Debug.Log("[WaveManager] 웨이브 성공! 몬스터 제거 + 상점 진입");

            // 게임 상태 변경(GameManager - EGameState)
            GameManager.Instance.CurrentGameState = EGameState.Shop;

            // 모든 몬스터 제거 (보상 없이)
            var monsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);
            foreach (var monster in monsters)
            {
                if (monster.isClone)
                {
                    Destroy(monster.gameObject); // 골드 지급 없는 파괴
                }
            }

            // 상점 팝업 띄우기
            ShowShopPopup();

            // 필드 상점 스포너로 스폰
            fieldShopSpawner.Spawn();
        }
        else
        {     
            Debug.LogError("[WaveManager] 게임 오버!");

            // 게임 상태 변경
            GameManager.Instance.CurrentGameState = EGameState.Finish;

            // 모든 몬스터 제거 (보상 없이)
            var monsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);
            foreach (var monster in monsters)
            {
                if (monster.isClone)
                {
                    Destroy(monster.gameObject); // 골드 지급 없는 파괴
                }
            }

            // 패배 UI 표시
            ShowResultPopup();
        }
    }

    //상점 팝업 함수
    private void ShowShopPopup()
    {
        if (shopPopup != null)
        {
            shopPopup.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[WaveManager] ShopPopup 오브젝트가 할당되지 않았습니다.");
        }
    }

    //상점 다음 웨이브 버튼
    private void OnStartNextWaveButtonClicked()
    {
        // 게임 상태 복구
        GameManager.Instance.CurrentGameState = EGameState.Wave;

        shopPopup.SetActive(false);
        currentWave++;
        StartNextWave();
    }

    //패배 결과 화면 팝업 함수
    private void ShowResultPopup()
    {
        if (resultPopup == null)
        {
            Debug.LogWarning("[WaveManager] ResultPopup이 할당되지 않았습니다.");
            return;
        }

        if (resultPopup.activeSelf) return; // 이미 켜져 있다면 무시

        Debug.Log("[WaveManager] 결과 팝업 활성화");
        resultPopup.SetActive(true);
    }

    //Title로 복귀 버튼
    public void OnReturnToTitleButtonClicked()
    {
        GameManager.Instance.CurrentGameState = EGameState.Title;
        SceneLoader.LoadScene(SceneLoader.SceneName.Title); // 씬 로딩으로 되돌아감
    }
}
