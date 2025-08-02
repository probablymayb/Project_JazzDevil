using UnityEngine;

[CreateAssetMenu(fileName = "SupporterSO", menuName = "Scriptable Objects/SupporterSO")]
public class SupporterSO : ScriptableObject
{
    public float attackCooldown;    // ���� ��Ÿ�� (0�̸� ��� �ߵ�)
    public int attackDamage;        // ������ Ȥ�� ȸ����
    public float attackRange;       // ���� ����

    public Sprite sprite;            // 스프라이트 이미지
    [TextArea] public string desc;   // 설명

    public ESupporters supporterType;   // Supporter 타입 Enum 값
}
