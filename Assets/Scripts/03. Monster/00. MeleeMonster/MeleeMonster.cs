public class MeleeMonster : Monster
{
    protected override void Start()
    {
        base.Start();
        AttackPattern = new MeleePattern();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
