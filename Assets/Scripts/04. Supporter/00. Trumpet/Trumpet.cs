using UnityEditor;
using UnityEngine;

public class Trumpet : Supporter
{
    private float attackRange = 5f; 
    private float angleRange = 90f; 
    private Color gizmoColor = Color.red;

    protected override void Start()
    {
        base.Start();
        ActPattern = new TrumpetPattern();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // 트럼펫 범위 보기 테스트
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Vector3 origin = transform.position;

        // **여기서도 평평한 '앞' 방향을 계산합니다.**
        Vector3 flatForwardDirection = transform.forward;
        flatForwardDirection.y = 0; // Y축 값을 0으로 만들어 평평하게 만듭니다.
        if (flatForwardDirection == Vector3.zero) // 완전히 수직으로 서서 forward가 (0,1,0)이 되는 경우 방지
        {
            flatForwardDirection = Vector3.forward; // 기본값 설정 또는 다른 예외 처리
        }
        flatForwardDirection.Normalize(); 

        // 부채꼴의 양쪽 끝 방향 벡터 계산
        // Y축을 기준으로 회전하도록 오일러 각도 사용
        Vector3 rightDir = Quaternion.Euler(0, angleRange / 2f, 0) * flatForwardDirection; 
        Vector3 leftDir = Quaternion.Euler(0, -angleRange / 2f, 0) * flatForwardDirection; 
        
        Gizmos.DrawRay(origin, rightDir * attackRange);
        Gizmos.DrawRay(origin, leftDir * attackRange);

        // 부채꼴의 호(arc) 그리기
        int segments = 20; 
        Vector3 previousPoint = origin + rightDir * attackRange;
        for (int i = 0; i <= segments; i++)
        {
            // 각도를 기준으로 회전시킬 때도 flatForwardDirection을 사용합니다.
            float currentAngle = angleRange / 2f - (angleRange / segments) * i; 
            Vector3 currentDir = Quaternion.Euler(0, currentAngle, 0) * flatForwardDirection; 
            
            Vector3 currentPoint = origin + currentDir * attackRange;
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }
}
