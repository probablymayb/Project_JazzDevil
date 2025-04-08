public class Saxophone : Supporter
{
    protected override void Start()
    {
        ActPattern = new SaxophonePattern();
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
