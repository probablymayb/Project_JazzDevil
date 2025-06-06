using UnityEngine;
public interface IComboSkill
{
    void OnComboStart(int combo);
    void OnComboIncrease(int combo, JudgementResult judgement);
    void OnComboBreak(int maxComboReached);
    void OnComboTierUp(ComboTier newTier, int combo);
}
