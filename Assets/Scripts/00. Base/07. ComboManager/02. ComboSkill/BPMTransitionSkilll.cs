using UnityEngine;
using FMODUnity;

public class BPMTransitionSkill : IComboSkill
{
    //BPM 5마다 TEST
    [Header("BPM 전환 설정")]
    [SerializeField] private int[] comboThresholds = { 0, 5, 10, 15, 20, 25, 30, 35, 40 };
    [SerializeField] private float[] targetBPMs = { 120f, 125f, 130f, 135f, 140f, 145f, 150f, 155f, 160f };
    [SerializeField] private bool resetOnComboBreak = true;
    [SerializeField] private bool transitionOnBeat = true;

    [Header("🥁 드럼 필인 설정")]
    [SerializeField] private string drumFillEventPath = "event:/Percurssion_FillIn";
    [SerializeField] private bool enableDrumFill = true;
    [SerializeField] private float drumFillVolume = 0.9f;

    [Header("🎵 불협화음 설정")]
    [SerializeField] private string dissonanceSFXPath = "event:/Combo_Fail";
    [SerializeField] private bool enableDissonance = true;
    [SerializeField] private float dissonanceVolume = 0.7f;
    [SerializeField] private bool playDissonanceOnMiss = true;

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
        // 🎵 Miss 판정 시 불협화음 재생
        if (judgement == JudgementResult.Miss && enableDissonance && playDissonanceOnMiss)
        {
            PlayDissonanceSFX("Miss 판정");
        }

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

                    // 🎵 콤보 브레이크 시 불협화음 재생
                    if (enableDissonance)
                    {
                        PlayDissonanceSFX("콤보 브레이크");
                    }

                    bpmTransitionPending = false;
                    RhythmManager.beatUpdated -= OnNextBeat;
                    Debug.Log($"💥 콤보 브레이크! BPM {targetBPMs[0]}으로 리셋 + 불협화음");
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

                // 🎵 콤보 브레이크 시 불협화음 재생
                if (enableDissonance)
                {
                    PlayDissonanceSFX("콤보 브레이크");
                }

                Debug.Log($"💥 콤보 브레이크! BPM {targetBPMs[0]}으로 리셋 + 불협화음");
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
            float oldBPM = lastActivatedBPMIndex >= 0 ? targetBPMs[lastActivatedBPMIndex] : targetBPMs[0];
            float newBPM = targetBPMs[bpmIndex];

            // 🥁 BPM 전환 시 드럼 필인 재생 (BPM 변경 전에!)
            if (enableDrumFill && bpmIndex > 0)
            {
                PlayDrumFill(oldBPM, newBPM);
            }

            // BPM 전환
            RhythmManager.Instance.SetBPM(newBPM);
            lastActivatedBPMIndex = bpmIndex;

            Debug.Log($"🚀 콤보 {combo}로 BPM {oldBPM} → {newBPM} 전환! + 드럼 필인");
        }
    }

    /// <summary>
    /// 🥁 드럼 필인 재생
    /// </summary>
    private void PlayDrumFill(float fromBPM, float toBPM)
    {
        if (string.IsNullOrEmpty(drumFillEventPath)) return;

        if (AudioManager.Instance != null)
        {
            // 독립적인 채널로 재생 (다른 사운드와 겹치지 않게)
            var instance = AudioManager.Instance.PlayOneShotIndependent(drumFillEventPath);
            if (instance.isValid())
            {
                instance.setVolume(drumFillVolume);

                // 🎵 필요시 BPM 파라미터 설정 (FMOD에서 BPM 파라미터가 있다면)
                instance.setParameterByName("FromBPM", fromBPM);
                instance.setParameterByName("ToBPM", toBPM);
            }
            Debug.Log($"🥁 드럼 필인 재생: {fromBPM} → {toBPM} BPM");
        }
        else
        {
            // AudioManager가 없으면 직접 재생
            RuntimeManager.PlayOneShot(drumFillEventPath);
            Debug.Log($"🥁 드럼 필인 재생: {fromBPM} → {toBPM} BPM (직접 재생)");
        }
    }

    /// <summary>
    /// 🎵 불협화음 SFX 재생
    /// </summary>
    private void PlayDissonanceSFX(string context)
    {
        if (string.IsNullOrEmpty(dissonanceSFXPath)) return;

        if (AudioManager.Instance != null)
        {
            // 독립적인 채널로 재생
            var instance = AudioManager.Instance.PlayOneShotIndependent(dissonanceSFXPath);
            if (instance.isValid())
            {
                instance.setVolume(dissonanceVolume);

                // 🎵 현재 BPM에 맞춰 불협화음 조정 (필요시)
                float currentBPM = RhythmManager.Instance.CurrentBpm;
                instance.setParameterByName("CurrentBPM", currentBPM);
            }
            Debug.Log($"🎵 불협화음 재생: {context} (BPM: {RhythmManager.Instance.CurrentBpm})");
        }
        else
        {
            // AudioManager가 없으면 직접 재생
            RuntimeManager.PlayOneShot(dissonanceSFXPath);
            Debug.Log($"🎵 불협화음 재생: {context} (직접 재생)");
        }
    }
}
