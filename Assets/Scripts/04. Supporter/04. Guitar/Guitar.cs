using UnityEngine;

public class Guitar : Supporter
{
    [SerializeField] private GameObject bulletPref;
    [SerializeField] private BulletSO bulletSO;

    protected override void Start()
    {
        base.Start();
        ActPattern = new GuitarPattern(bulletPref, bulletSO);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
