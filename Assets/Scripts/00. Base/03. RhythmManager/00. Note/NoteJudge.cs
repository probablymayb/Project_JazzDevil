using UnityEngine;
using System;

public enum JudgementResult
{
    None,
    Miss,    // 완전 빗나감
    Good,    // 타이밍이 조금 빗나감 
    Solid,   // 타이밍이 좋음
    Excellent // 타이밍이 완벽함
}

public class NoteJudge : MonoBehaviour
{
    public static NoteJudge Instance { get; private set; }

    [Header("판정 설정")]
    [SerializeField] private float excellentWindow = 0.1f;  // ±50ms
    [SerializeField] private float solidWindow = 0.2f;      // ±100ms
    [SerializeField] private float goodWindow = 0.3f;       // ±200ms


    //kj
    [SerializeField] private NoteSpawner noteSpawner;
    [SerializeField] private JudgeNoteTextUI JudgeNoteTextUI;

    //판정 발생 이벤트
    public event Action<JudgementResult> OnJudgement;

    // FMOD 사운드 이벤트 경로
    [Header("FMOD 사운드 이벤트")]
    [SerializeField] private string excellentSoundPath = "event:/Judgement/Excellent";
    [SerializeField] private string solidSoundPath = "event:/Judgement/Solid";
    [SerializeField] private string goodSoundPath = "event:/Judgement/Good";
    [SerializeField] private string missSoundPath = "event:/Judgement/Miss";

    // 판정에 따른 데미지 배율
    [Header("데미지 배율")]
    [SerializeField] private float excellentDamageMultiplier = 2.0f;
    [SerializeField] private float solidDamageMultiplier = 1.5f;
    [SerializeField] private float goodDamageMultiplier = 1.0f;
    [SerializeField] private float missDamageMultiplier = 0.1f;

    // 디버깅용
    [Header("디버그")]
    [SerializeField] private bool showDebugInfo = true;

    // RhythmManager 참조
    private RhythmManager rhythmManager;

    private void Awake()
    {
        Instance = this;

        // RhythmManager 참조 찾기
        rhythmManager = RhythmManager.Instance;
        if (rhythmManager == null)
        {
            Debug.LogError("NoteJudge: RhythmManager를 찾을 수 없습니다!");
        }
    }

    //타이밍 판단
    public JudgementResult Judge()
    {
        if (rhythmManager == null) return JudgementResult.Miss;
        Debug.Log("CheckNoteHit ȣ���");
        Note closestNote = noteSpawner.GetClosestNote();

        // RhythmManager의 비트 정보를 사용하여 정확도 판정
        float beatProgress = GetBeatProgress();
        JudgementResult result = GetJudgementFromProgress(beatProgress);

        if (showDebugInfo)
        {
            Debug.Log($"Beat Progress: {beatProgress:F3}, Judgement: {result}");
        }

        OnJudgement?.Invoke(result);

        //판정 피드백
        PlayJudgementSound(result);
        ShowJudgementFeedback(result);

        return result;
    }

    // 현재 비트 진행도 계산 (0~0.5)
    private float GetBeatProgress()
    {
        // RhythmManager의 GetCurrentBeatProgress 함수 사용
        return rhythmManager.GetCurrentBeatProgress();
    }

    // 비트 진행도에 따른 판정 결정
    private JudgementResult GetJudgementFromProgress(float progress)
    {
        // 이미 0~0.5 범위로 정규화된 진행도를 받는다고 가정
        float normalizedProgress = progress;

        if (normalizedProgress <= excellentWindow)
        {
            return JudgementResult.Excellent;
        }
        else if (normalizedProgress <= solidWindow)
        {
            return JudgementResult.Solid;
        }
        else if (normalizedProgress <= goodWindow)
        {
            return JudgementResult.Good;
        }
        else
        {
            return JudgementResult.Miss;
        }
    }

    // 데미지 배율 반환
    public float GetDamageMultiplier(JudgementResult judgement)
    {

        if (JudgeNoteTextUI != null)
        {
            JudgeNoteTextUI.ShowJudge(judgement);
        }
       

        switch (judgement)
        {
            case JudgementResult.Excellent:
                return excellentDamageMultiplier;
            case JudgementResult.Solid:
                return solidDamageMultiplier;
            case JudgementResult.Good:
                return goodDamageMultiplier;
            default:
                return missDamageMultiplier;
        }
        //note.Hit();
        //noteSpawner.RemoveNote(note);
        //Destroy(note.gameObject);

        

        // �߰����� ���� ó�� (����, �޺� ��)

    }

    // 판정에 따른 사운드 재생
    private void PlayJudgementSound(JudgementResult result)
    {
        string eventPath = "";

        switch (result)
        {
            case JudgementResult.Excellent:
                eventPath = excellentSoundPath;
                break;
            case JudgementResult.Solid:
                eventPath = solidSoundPath;
                break;
            case JudgementResult.Good:
                eventPath = goodSoundPath;
                break;
            case JudgementResult.Miss:
                eventPath = missSoundPath;
                break;
        }

        if (!string.IsNullOrEmpty(eventPath))
        {
            FMODUnity.RuntimeManager.PlayOneShot(eventPath);
        }
    }

    // 판정 결과 UI 피드백
    private void ShowJudgementFeedback(JudgementResult result)
    {
        // UI 매니저를 통해 판정 텍스트 표시
        if (showDebugInfo)
        {
            Debug.Log($"Judgement UI: {result}");
        }

        // UIManager.Instance.ShowJudgement(result);
    }
}
