using UnityEngine;

[CreateAssetMenu(fileName = "SupporterSO", menuName = "Scriptable Objects/SupporterSO")]
public class SupporterSO : ScriptableObject
{
    public float attackCooldown;    // 기본 쿨타임 (0이면 상시 발동)
    public int attackDamage;        // 기본 공격력 혹은 회복량
    public float attackRange;       // 기본 공격 범위

    public Sprite sprite;            // 스프라이트 이미지
    [TextArea] public string desc;   // 설명 (플레이스홀더 지원: {cooldown}, {damage}, {range})

    public ESupporters supporterType;   // Supporter 타입 Enum 값
}
