using UnityEngine;
using System;

public enum JudgementResult
{
    None,
    Miss,
    Good,
    Solid,
    Excellent
}

public class NoteJudge : MonoBehaviour
{
    public static NoteJudge Instance { get; private set; }
    public int excellentCount = 0, solidCount = 0, goodCount = 0, missCount = 0;
    public int TotalJudged => excellentCount + solidCount + goodCount + missCount;

    [Header("판정 설정")]
    [SerializeField] private float excellentWindow = 0.2f;
    [SerializeField] private float solidWindow = 0.35f;
    [SerializeField] private float goodWindow = 0.5f;

    [Header("참조")]
    [SerializeField] private NoteSpawner noteSpawner;
    [SerializeField] private JudgeNoteTextUI JudgeNoteTextUI;

    public event Action<JudgementResult> OnJudgement;

    [Header("FMOD 사운드 이벤트")]
    [SerializeField] private string excellentSoundPath = "event:/Judgement/Excellent";
    [SerializeField] private string solidSoundPath = "event:/Judgement/Solid";
    [SerializeField] private string goodSoundPath = "event:/Judgement/Good";
    [SerializeField] private string missSoundPath = "event:/Judgement/Miss";

    [Header("데미지 배율")]
    [SerializeField] private float excellentDamageMultiplier = 2.0f;
    [SerializeField] private float solidDamageMultiplier = 1.5f;
    [SerializeField] private float goodDamageMultiplier = 1.0f;
    [SerializeField] private float missDamageMultiplier = 0.1f;

    [Header("디버그")]
    [SerializeField] private bool showDebugInfo = true;

    private RhythmManager rhythmManager;

    private void Awake()
    {
        Instance = this;
        rhythmManager = RhythmManager.Instance;

        if (rhythmManager == null)
            Debug.LogError("NoteJudge: RhythmManager를 찾을 수 없습니다!");
    }

 
    public JudgementResult Judge()
    {
        Note closestNote = noteSpawner.GetClosestNote();

        // 노트가 없으면 Miss
        if (closestNote == null)
        {
            missCount++;
            OnJudgement?.Invoke(JudgementResult.Miss);
            PlayJudgementSound(JudgementResult.Miss);
            return JudgementResult.Miss;
        }

        // 노트의 타이밍 오차로 판정
        float timingError = closestNote.GetTimingError();
        JudgementResult result = GetJudgementFromError(timingError);

        switch (result)
        {
            case JudgementResult.Excellent: excellentCount++; break;
            case JudgementResult.Solid: solidCount++; break;
            case JudgementResult.Good: goodCount++; break;
            case JudgementResult.Miss: missCount++; break;
        }

        if (showDebugInfo)
            Debug.Log($"Timing Error: {timingError:F3}s, Judgement: {result}");

        closestNote.Hit(result);

        if (result != JudgementResult.Miss)
            noteSpawner.RemoveNote(closestNote);

        OnJudgement?.Invoke(result);
        PlayJudgementSound(result);
        ShowJudgementFeedback(result);

        return result;
    }

    private JudgementResult GetJudgementFromError(float error)
    {
        if (error <= excellentWindow)
            return JudgementResult.Excellent;
        else if (error <= solidWindow)
            return JudgementResult.Solid;
        else if (error <= goodWindow)
            return JudgementResult.Good;
        else
            return JudgementResult.Miss;
    }
    public float Accuracy => TotalJudged > 0 ? (excellentCount + solidCount + goodCount) * 100f / TotalJudged : 0f;

    private float GetBeatProgress()
    {
        return rhythmManager.GetCurrentBeatProgress();
    }

    private JudgementResult GetJudgementFromProgress(float progress)
    {
        float normalizedProgress = progress;

        if (normalizedProgress <= excellentWindow)
            return JudgementResult.Excellent;
        else if (normalizedProgress <= solidWindow)
            return JudgementResult.Solid;
        else if (normalizedProgress <= goodWindow)
            return JudgementResult.Good;
        else
            return JudgementResult.Miss;
    }

    public float GetDamageMultiplier(JudgementResult judgement)
    {
        if (JudgeNoteTextUI != null)
            JudgeNoteTextUI.ShowJudge(judgement);

        switch (judgement)
        {
            case JudgementResult.Excellent: return excellentDamageMultiplier;
            case JudgementResult.Solid: return solidDamageMultiplier;
            case JudgementResult.Good: return goodDamageMultiplier;
            default: return missDamageMultiplier;
        }
    }

    private void PlayJudgementSound(JudgementResult result)
    {
        string eventPath = result switch
        {
            JudgementResult.Excellent => excellentSoundPath,
            JudgementResult.Solid => solidSoundPath,
            JudgementResult.Good => goodSoundPath,
            JudgementResult.Miss => missSoundPath,
            _ => ""
        };

        if (!string.IsNullOrEmpty(eventPath))
            FMODUnity.RuntimeManager.PlayOneShot(eventPath);
    }

    private void ShowJudgementFeedback(JudgementResult result)
    {
        if (showDebugInfo)
            Debug.Log($"Judgement UI: {result}");
    }
}
