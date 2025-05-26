using UnityEngine;
using UnityEngine.PlayerLoop;

public interface IMonsterPattern
{
    void AttackPattern(Transform transform, Transform player, Animator animator, MonsterSO monsterData);
}

public class MeleePattern : IMonsterPattern
{
    public void AttackPattern(Transform transform, Transform player, Animator animator, MonsterSO monsterData)
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

public class RangedPattern : IMonsterPattern
{
    private GameObject bulletPref;
    private BulletSO bulletSO;

    // 생성자 (탄환 프리팹 초기화)
    public RangedPattern(GameObject bulletPref, BulletSO bulletSO)
    {
        this.bulletPref = bulletPref;
        this.bulletSO = bulletSO;
    }
    
    // 패턴
    public void AttackPattern(Transform transform, Transform player, Animator animator, MonsterSO monsterData)
    {
        GameObject newBullet = PoolManager.Instance.Get(bulletPref);    // 풀 매니저로 탄 생성

        newBullet.SetActive(false);                                     // 잠시 비활성화

        newBullet.transform.position = transform.position;              // 위치 설정
        Bullet bulletComp = newBullet.GetComponent<Bullet>();           // 컴포넌트 가져오기
        bulletComp.PoolPrefRef = bulletPref;                            // 풀 반환용 프리팹 Set
        bulletComp.BulletSpeed = bulletSO.bulletSpeed;                  // 탄속 Set
        bulletComp.Damage = bulletSO.bulletDamage;                      // 공격력 Set
        Vector3 direction = player.position - newBullet.transform.position;
        bulletComp.Direction = new Vector3(direction.x, 0f, direction.z); // 방향 Set
        bulletComp.Direction = bulletComp.Direction.normalized;

        newBullet.SetActive(true);                                      // 초기화 완료했으니 다시 활성화

        animator.SetBool("isWindup", false);
        animator.SetBool("isAttack", true);
    }
}
