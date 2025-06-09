using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ComboManager : Singleton<ComboManager>
{
    [Header("콤보 설정")]
    [SerializeField] private ComboData comboData;

    [Header("현재 상태")]
    [SerializeField] private int currentCombo = 0;
    [SerializeField] private int maxComboThisSession = 0;
    [SerializeField] private ComboTier currentTier;

    // 콤보 리셋 타이머
    private Coroutine comboResetCoroutine;

    // 콤보 효과 시스템들
    private List<IComboSkill> comboEffects = new List<IComboSkill>();

    // 이벤트 시스템
    public event System.Action<int> OnComboChanged;
    public event System.Action<int, JudgementResult> OnComboIncreased;
    public event System.Action<int> OnComboBreak;
    public event System.Action<ComboTier, int> OnComboTierChanged;

    // 접근자
    public int CurrentCombo => currentCombo;
    public int MaxComboThisSession => maxComboThisSession;
    public ComboTier CurrentTier => currentTier;
    public float CurrentDamageMultiplier => currentTier?.damageMultiplier ?? 1f;
    public float CurrentScoreMultiplier => currentTier?.scoreMultiplier ?? 1f;

    protected override void Awake()
    {
        base.Awake();

        // 기본 콤보 효과들 등록
        RegisterComboEffect(new SessionAddSkill());
        RegisterComboEffect(new BPMTransitionSkill());
        //RegisterComboEffect(new UIComboEffect());
        //RegisterComboEffect(new AudioComboEffect());
    }

    private void Start()
    {
        // NoteJudge의 판정 이벤트 구독
        if (NoteJudge.Instance != null)
        {
            // 기존 NoteJudge에 이벤트 추가 필요
            NoteJudge.Instance.OnJudgement += HandleJudgement;
        }
    }

    private void OnDestroy()
    {
        if (NoteJudge.Instance != null)
        {
            NoteJudge.Instance.OnJudgement -= HandleJudgement;
        }
    }

    // 콤보 효과 등록
    public void RegisterComboEffect(IComboSkill effect)
    {
        if (!comboEffects.Contains(effect))
        {
            comboEffects.Add(effect);
        }
    }

    // 콤보 효과 해제
    public void UnregisterComboEffect(IComboSkill effect)
    {
        comboEffects.Remove(effect);
    }

    // 판정 결과 처리
    private void HandleJudgement(JudgementResult judgement)
    {
        switch (judgement)
        {
            case JudgementResult.Excellent:
            case JudgementResult.Solid:
                IncreaseCombo(judgement);
                break;

            case JudgementResult.Good:
                if (comboData.goodBreaksCombo)
                    BreakCombo();
                else
                    IncreaseCombo(judgement);
                break;

            case JudgementResult.Miss:
                if (comboData.missBreaksCombo)
                    BreakCombo();
                break;
        }
    }

    // 콤보 증가
    private void IncreaseCombo(JudgementResult judgement)
    {
        // 콤보 리셋 코루틴 취소
        if (comboResetCoroutine != null)
        {
            StopCoroutine(comboResetCoroutine);
            comboResetCoroutine = null;
        }

        // 콤보 시작
        if (currentCombo == 0)
        {
            foreach (var effect in comboEffects)
            {
                effect.OnComboStart(1);
            }
        }

        // 콤보 증가
        currentCombo++;
        maxComboThisSession = Mathf.Max(maxComboThisSession, currentCombo);

        // 최대 콤보 제한
        if (currentCombo > comboData.maxCombo)
        {
            currentCombo = comboData.maxCombo;
        }

        // 콤보 티어 체크
        CheckComboTier();

        // 이벤트 발생
        OnComboChanged?.Invoke(currentCombo);
        OnComboIncreased?.Invoke(currentCombo, judgement);

        // 콤보 효과 실행
        foreach (var effect in comboEffects)
        {
            effect.OnComboIncrease(currentCombo, judgement);
        }

        Debug.Log($"Combo: {currentCombo} (Tier: {currentTier?.tierName})");
    }

    // 콤보 브레이크
    private void BreakCombo()
    {
        if (currentCombo == 0) return;

        int brokenCombo = currentCombo;
        currentCombo = 0;
        currentTier = null;

        // 이벤트 발생
        OnComboBreak?.Invoke(brokenCombo);

        // 콤보 효과 실행
        foreach (var effect in comboEffects)
        {
            effect.OnComboBreak(brokenCombo);
        }

        Debug.Log($"Combo Break! Max was: {brokenCombo}");

        // 일정 시간 후 콤보 상태 초기화
        if (comboData.comboResetTime > 0)
        {
            comboResetCoroutine = StartCoroutine(ResetComboAfterTime());
        }
    }

    // 콤보 티어 체크
    private void CheckComboTier()
    {
        if (comboData.comboTiers == null) return;

        ComboTier newTier = null;

        // 현재 콤보에 맞는 최고 티어 찾기
        for (int i = comboData.comboTiers.Length - 1; i >= 0; i--)
        {
            if (currentCombo >= comboData.comboTiers[i].requiredCombo)
            {
                newTier = comboData.comboTiers[i];
                break;
            }
        }

        // 티어 변경 시
        if (newTier != currentTier && newTier != null)
        {
            currentTier = newTier;
            OnComboTierChanged?.Invoke(currentTier, currentCombo);

            // 콤보 효과 실행
            foreach (var effect in comboEffects)
            {
                effect.OnComboTierUp(currentTier, currentCombo);
            }

            Debug.Log($"Combo Tier Up! {currentTier.tierName} (x{currentTier.damageMultiplier})");
        }
    }

    // 시간 후 콤보 리셋
    private System.Collections.IEnumerator ResetComboAfterTime()
    {
        yield return new WaitForSeconds(comboData.comboResetTime);
        // 여기서 추가적인 리셋 로직 수행 가능
    }

    // 강제 콤보 리셋 (디버그용)
    public void ForceResetCombo()
    {
        currentCombo = 0;
        currentTier = null;
        OnComboChanged?.Invoke(currentCombo);
    }
}
