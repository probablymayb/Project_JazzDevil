using UnityEditor;
using UnityEngine;

public class Trumpet : Supporter
{
    protected override void Start()
    {
        base.Start();
        ActPattern = new TrumpetPattern();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // TODO : 나중에 삭제해야 함.
    // 트럼펫 범위 보기 테스트
    private void OnDrawGizmos()
    {
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, 45f, 3f);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -45f, 3f);
    }
}
