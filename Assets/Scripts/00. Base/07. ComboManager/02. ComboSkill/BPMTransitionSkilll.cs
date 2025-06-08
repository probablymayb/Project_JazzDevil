using UnityEngine;
public class BPMTransitionSkill : IComboSkill
{
    //BPM 5마다 TEST
    [Header("BPM 전환 설정")]
    [SerializeField] private int[] comboThresholds = { 0, 5, 10, 15, 20, 25, 30, 35, 40 };
    [SerializeField] private float[] targetBPMs = { 120f, 125f, 130f, 135f, 140f, 145f, 150f, 155f, 160f };
    [SerializeField] private bool resetOnComboBreak = true;
    [SerializeField] private bool transitionOnBeat = true;

    private int lastActivatedBPMIndex = -1;
    private bool bpmTransitionPending = false;

    public void OnComboStart(int combo)
    {
        Debug.Log("콤보 시작! BPM 전환 준비");

        // 콤보 시작 시 기본 BPM으로 설정
        if (targetBPMs.Length > 0)
        {
            RhythmManager.Instance.SetBPM(targetBPMs[0]);
            lastActivatedBPMIndex = 0;
        }
    }

    public void OnComboIncrease(int combo, JudgementResult judgement)
    {
        // 콤보 증가 시 BPM 전환 체크
        CheckAndTransitionBPM(combo);
    }

    public void OnComboBreak(int maxComboReached)
    {
        if (resetOnComboBreak)
        {
            if (transitionOnBeat && !bpmTransitionPending)
            {
                // 다음 비트에서 기본 BPM으로 리셋
                bpmTransitionPending = true;

                void OnNextBeat()
                {
                    if (targetBPMs.Length > 0)
                    {
                        RhythmManager.Instance.SetBPM(targetBPMs[0]);
                        lastActivatedBPMIndex = 0;
                    }
                    bpmTransitionPending = false;
                    RhythmManager.beatUpdated -= OnNextBeat;
                    Debug.Log($"💥 콤보 브레이크! BPM {targetBPMs[0]}으로 리셋");
                }

                RhythmManager.beatUpdated += OnNextBeat;
            }
            else if (!transitionOnBeat)
            {
                // 즉시 기본 BPM으로 리셋
                if (targetBPMs.Length > 0)
                {
                    RhythmManager.Instance.SetBPM(targetBPMs[0]);
                    lastActivatedBPMIndex = 0;
                }
                Debug.Log($"💥 콤보 브레이크! BPM {targetBPMs[0]}으로 리셋");
            }
        }
    }

    public void OnComboTierUp(ComboTier newTier, int combo)
    {
        Debug.Log($"콤보 티어 업! {newTier.tierName} - 현재 콤보: {combo}");
    }

    private void CheckAndTransitionBPM(int combo)
    {
        // 현재 콤보에 해당하는 BPM 인덱스 찾기
        int targetBPMIndex = GetBPMIndexForCombo(combo);

        // 새로운 BPM 전환이 필요한지 체크
        if (targetBPMIndex > lastActivatedBPMIndex && targetBPMIndex >= 0)
        {
            if (transitionOnBeat && !bpmTransitionPending)
            {
                // 다음 비트에서 BPM 전환
                RequestBPMTransitionOnBeat(targetBPMIndex, combo);
            }
            else if (!transitionOnBeat)
            {
                // 즉시 BPM 전환
                TransitionBPM(targetBPMIndex, combo);
            }
        }
    }

    private int GetBPMIndexForCombo(int combo)
    {
        // 콤보 임계값을 넘을 때마다 새로운 BPM으로 전환
        for (int i = 0; i < comboThresholds.Length; i++)
        {
            if (combo >= comboThresholds[i] && i > lastActivatedBPMIndex)
            {
                return i;
            }
        }
        return -1;
    }

    private void RequestBPMTransitionOnBeat(int bpmIndex, int combo)
    {
        bpmTransitionPending = true;

        void OnNextBeat()
        {
            TransitionBPM(bpmIndex, combo);
            bpmTransitionPending = false;
            RhythmManager.beatUpdated -= OnNextBeat;
        }

        RhythmManager.beatUpdated += OnNextBeat;
    }

    private void TransitionBPM(int bpmIndex, int combo)
    {
        if (bpmIndex >= 0 && bpmIndex < targetBPMs.Length)
        {
            float newBPM = targetBPMs[bpmIndex];
            RhythmManager.Instance.SetBPM(newBPM);
            lastActivatedBPMIndex = bpmIndex;

            Debug.Log($"🚀 콤보 {combo}로 BPM {newBPM} 전환!");
        }
    }
}
