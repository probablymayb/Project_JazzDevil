using UnityEngine;

[System.Serializable]
public class ComboTier
{
    public int requiredCombo;                     // 필요 콤보 수
    public string tierName;                       // 단계 이름 (예: "Great", "Amazing", "Legendary")
    public float damageMultiplier = 1f;           // 데미지 배율
    public float scoreMultiplier = 1f;            // 점수 배율
    public Color uiColor = Color.white;           // UI 색상
    public GameObject effectPrefab;               // 이펙트 프리팹
}
