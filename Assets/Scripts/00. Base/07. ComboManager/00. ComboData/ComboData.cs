using UnityEngine;

[CreateAssetMenu(fileName = "ComboData", menuName = "Scriptable Objects/ComboData")]
public class ComboData : ScriptableObject
{
    [Header("콤보 기본 설정")]
    public int maxCombo = 999;                    // 최대 콤보 수
    public float comboResetTime = 2f;             // 콤보 리셋 시간 (Miss 후)

    [Header("콤보 브레이크 조건")]
    public bool missBreaksCombo = true;           // Miss 시 콤보 브레이크 여부
    public bool goodBreaksCombo = false;          // Good 판정도 콤보 브레이크할지

    [Header("콤보 단계별 설정")]
    public ComboTier[] comboTiers;                // 콤보 단계별 효과
}
