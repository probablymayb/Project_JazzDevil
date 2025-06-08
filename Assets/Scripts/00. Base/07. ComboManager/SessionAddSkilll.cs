using UnityEngine;
public class SessionAddSkill : IComboSkill
{
    [Header("세션 활성화 설정")]
    [SerializeField] private int[] comboThresholds = { 10, 25, 50, 75, 100 };
    [SerializeField] private bool resetOnComboBreak = true;
    [SerializeField] private bool activateOnBeat = true;

    private int lastActivatedSessionIndex = -1;
    private bool sessionActivationPending = false;

    public void OnComboStart(int combo)
    {
        Debug.Log("콤보 시작! 세션 활성화 준비");
    }

    public void OnComboIncrease(int combo, JudgementResult judgement)
    {
        // 콤보 증가 시 세션 활성화 체크
        CheckAndActivateSession(combo);
    }

    public void OnComboBreak(int maxComboReached)
    {
        if (resetOnComboBreak)
        {
            if (activateOnBeat && !sessionActivationPending)
            {
                // 다음 비트에서 리셋
                sessionActivationPending = true;

                void OnNextBeat()
                {
                    RhythmManager.Instance.DeactivateAllSessions();
                    lastActivatedSessionIndex = -1;
                    sessionActivationPending = false;
                    RhythmManager.beatUpdated -= OnNextBeat;
                }

                RhythmManager.beatUpdated += OnNextBeat;
            }
            else if (!activateOnBeat)
            {
                // 즉시 리셋
                RhythmManager.Instance.DeactivateAllSessions();
                lastActivatedSessionIndex = -1;
            }
        }
    }

    public void OnComboTierUp(ComboTier newTier, int combo)
    {
        Debug.Log($"콤보 티어 업! {newTier.tierName} - 현재 콤보: {combo}");
    }

    private void CheckAndActivateSession(int combo)
    {
        // 현재 콤보에 해당하는 세션 인덱스 찾기
        int targetSessionIndex = GetSessionIndexForCombo(combo);

        // 새로운 세션 활성화가 필요한지 체크
        if (targetSessionIndex > lastActivatedSessionIndex && targetSessionIndex >= 0)
        {
            if (activateOnBeat && !sessionActivationPending)
            {
                // 다음 비트에서 활성화
                RequestSessionActivationOnBeat(targetSessionIndex, combo);
            }
            else if (!activateOnBeat)
            {
                // 즉시 활성화
                ActivateSession(targetSessionIndex, combo);
            }
        }
    }

    private int GetSessionIndexForCombo(int combo)
    {
        // 콤보 임계값을 넘을 때마다 새로운 세션 활성화
        for (int i = 0; i < comboThresholds.Length; i++)
        {
            if (combo >= comboThresholds[i] && i > lastActivatedSessionIndex)
            {
                return i;
            }
        }
        return -1;
    }

    private void RequestSessionActivationOnBeat(int sessionIndex, int combo)
    {
        sessionActivationPending = true;

        void OnNextBeat()
        {
            ActivateSession(sessionIndex, combo);
            sessionActivationPending = false;
            RhythmManager.beatUpdated -= OnNextBeat;
        }

        RhythmManager.beatUpdated += OnNextBeat;
    }

    private void ActivateSession(int sessionIndex, int combo)
    {
        RhythmManager.Instance.ActivateNextSession();
        lastActivatedSessionIndex = sessionIndex;

        Debug.Log($"🎵 콤보 {combo}로 세션 {sessionIndex + 1} 활성화!");
    }
}
