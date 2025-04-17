using UnityEngine;

public class Piano : Supporter
{
    protected override void Start()
    {
        base.Start();
        ActPattern = new PianoPattern();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
