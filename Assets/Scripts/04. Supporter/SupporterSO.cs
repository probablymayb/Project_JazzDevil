using UnityEngine;

[CreateAssetMenu(fileName = "SupporterSO", menuName = "Scriptable Objects/SupporterSO")]
public class SupporterSO : ScriptableObject
{
    public float attackCooldown;    // 공격 쿨타임 (0이면 상시 발동)
    public int attackDamage;        // 데미지 혹은 회복력
    public float attackRange;       // 공격 범위
}
