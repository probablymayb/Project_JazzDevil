using System;
using NUnit.Framework;
using UnityEngine;

public interface ISupporterPattern
{
    void ActPattern(Transform transform, Transform player, SupporterManager.RuntimeStats stats);
}

public class TrumpetPattern : ISupporterPattern
{
    public void ActPattern(Transform transform, Transform player, SupporterManager.RuntimeStats stats)
    {
        // 공격 범위 내 콜라이더 가져오기
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, stats.attackRange);
        float angleRange = 90f; // 부채꼴 각

        Vector3 flatForwardDirection = transform.forward;
        flatForwardDirection.y = 0f;
        flatForwardDirection.Normalize();

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            {
                continue;
            }

            Monster monsterComp = hitCollider.GetComponentInParent<Monster>();
            if (monsterComp == null)
            {
                continue;
            }

            Vector3 directionToMonster = (monsterComp.transform.position - transform.position);
            directionToMonster.y = 0f;
            directionToMonster.Normalize();

            // 부채꼴 내에 존재하는지 확인
            float dotProdict = Vector3.Dot(flatForwardDirection, directionToMonster);

            if (dotProdict >= Mathf.Cos(angleRange / 2f * Mathf.Deg2Rad))
            {
                monsterComp.TakeDamage(stats.attackDamage);
            }
        }
    }
}

public class PianoPattern : ISupporterPattern
{
    public void ActPattern(Transform transform, Transform player, SupporterManager.RuntimeStats stats)
    {
        PlayerController playerCon = player.GetComponent<PlayerController>();
        playerCon.Heal(stats.attackDamage);
    }
}

public class SaxophonePattern : ISupporterPattern
{
    public void ActPattern(Transform transform, Transform player, SupporterManager.RuntimeStats stats)
    {
        PlayerController playerCon = player.GetComponent<PlayerController>();
        playerCon.UpgradeDamage(stats.attackDamage);
    }
}

public class GuitarPattern : ISupporterPattern
{
    private GameObject bulletPref;
    private BulletSO bulletSO;

    // 생성자 (탄환 프리팹 초기화)
    public GuitarPattern(GameObject bulletPref, BulletSO bulletSO)
    {
        this.bulletPref = bulletPref;
        this.bulletSO = bulletSO;
    }
    
    // 패턴
    public void ActPattern(Transform transform, Transform player, SupporterManager.RuntimeStats stats)
    {
        GameObject newBullet = PoolManager.Instance.Get(bulletPref);    // 풀 매니저로 탄 생성

        newBullet.SetActive(false);                                     // 잠시 비활성화

        newBullet.transform.position = transform.position;              // 위치 설정
        Bullet bulletComp = newBullet.GetComponent<Bullet>();           // 컴포넌트 가져오기
        bulletComp.PoolPrefRef = bulletPref;                            // 풀 반환용 프리팹 Set
        bulletComp.BulletSpeed = bulletSO.bulletSpeed;                  // 탄속 Set
        bulletComp.Damage = bulletSO.bulletDamage;                      // 공격력 Set (런타임 stats 아님)
        Vector3 direction = Finder.NearestObject(transform, "Monster").position - newBullet.transform.position;
        bulletComp.Direction = new Vector3(direction.x, 0f, direction.z); // 방향 Set
        bulletComp.Direction = bulletComp.Direction.normalized;
        bulletComp.IsPenetrable = true;
        bulletComp.Friendly = Bullet.EFriendly.Player;

        newBullet.SetActive(true);                                      // 초기화 완료했으니 다시 활성화
    }
}
