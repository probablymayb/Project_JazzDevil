using UnityEngine;

public interface IMonsterPattern
{
    void AttackPattern(Transform player, Animator animator, MonsterSO monsterData);
}

public class MeleePattern : IMonsterPattern
{
    public void AttackPattern(Transform player, Animator animator, MonsterSO monsterData)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            Debug.Log("Player Damaged : " + monsterData.attackDamage);
            playerController.TakeDamage(monsterData.attackDamage); // 플레이어 체력 감소
            animator.SetBool("isWindup", false);
            animator.SetBool("isAttack", true);
        }
    }
}
