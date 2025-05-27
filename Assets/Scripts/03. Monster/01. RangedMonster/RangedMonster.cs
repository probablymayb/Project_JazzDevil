using UnityEngine;

public class RangedMonster : Monster
{
    [SerializeField] private GameObject bulletPref;
    [SerializeField] private BulletSO bulletSO;

    protected override void Start()
    {
        base.Start();
        AttackPattern = new RangedPattern(bulletPref, bulletSO); // 탄환 프리팹을 생성자의 매개변수로 넘김
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
