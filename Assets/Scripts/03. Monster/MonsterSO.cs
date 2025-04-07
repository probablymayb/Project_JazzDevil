using UnityEngine;

[CreateAssetMenu(fileName = "MonsterSO", menuName = "Scriptable Objects/MonsterSO")]
public class MonsterSO : ScriptableObject
{
    public float speed;                 // 몬스터 이동 속도
    public int maxHealth;               // 몬스터 최대 체력
    public int attackWindup;            // 공격 준비 계수
    public float attackRange;           // 공격 범위
    public int attackDamage;            // 공격 데미지
}
