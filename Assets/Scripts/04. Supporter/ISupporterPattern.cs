using NUnit.Framework;
using UnityEngine;

public interface ISupporterPattern
{
    void ActPattern(Transform transform, Transform player, SupporterSO supporterData);
}

public class TrumpetPattern : ISupporterPattern
{
    public void ActPattern(Transform transform, Transform player, SupporterSO supporterData)
    {
        GameObject[] activatingMonsters = GameObject.FindGameObjectsWithTag("Monster");
        float angleRange = 90f;     // ���հ�

        foreach (GameObject monster in activatingMonsters)
        {
            Vector3 interV = monster.transform.position - transform.position;

            if (interV.magnitude <= supporterData.attackRange)
            {
                float dot = Vector2.Dot(interV.normalized, transform.forward);
                float theta = Mathf.Acos(dot);
                float degree = Mathf.Rad2Deg * theta;

                if (degree <= angleRange / 2f)
                {
                    monster.GetComponent<Monster>().TakeDamage(supporterData.attackDamage);
                }
            }
        }
    }
}

public class PianoPattern : ISupporterPattern
{
    public void ActPattern(Transform transform, Transform player, SupporterSO supporterData)
    {
        PlayerController playerCon = player.GetComponent<PlayerController>();
        playerCon.Heal(supporterData.attackDamage);
    }
}

public class SaxophonePattern : ISupporterPattern
{
    public void ActPattern(Transform transform, Transform player, SupporterSO supporterData)
    {
        PlayerController playerCon = player.GetComponent<PlayerController>();
        playerCon.UpgradeDamage(supporterData.attackDamage);
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
    public void ActPattern(Transform transform, Transform player, SupporterSO supporterData)
    {
        GameObject newBullet = PoolManager.Instance.Get(bulletPref);    // 풀 매니저로 탄 생성

        newBullet.SetActive(false);                                     // 잠시 비활성화

        newBullet.transform.position = transform.position;              // 위치 설정
        Bullet bulletComp = newBullet.GetComponent<Bullet>();           // 컴포넌트 가져오기
        bulletComp.PoolPrefRef = bulletPref;                            // 풀 반환용 프리팹 Set
        bulletComp.BulletSpeed = bulletSO.bulletSpeed;                  // 탄속 Set
        bulletComp.Damage = bulletSO.bulletDamage;                      // 공격력 Set
        Vector3 direction = Finder.NearestObject(transform, "Monster").position - newBullet.transform.position;
        bulletComp.Direction = new Vector3(direction.x, 0f, direction.z); // 방향 Set
        bulletComp.Direction = bulletComp.Direction.normalized;
        bulletComp.IsPenetrable = true;
        bulletComp.Friendly = Bullet.EFriendly.Player;

        newBullet.SetActive(true);                                      // 초기화 완료했으니 다시 활성화
    }
}
