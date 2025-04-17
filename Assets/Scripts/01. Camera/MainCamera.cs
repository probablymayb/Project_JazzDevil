using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;  // 플레이어의 Transform
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -7f);  // 카메라와 플레이어 간의 거리

    [Header("Movement Settings")]
    [SerializeField] private float smoothSpeed = 5f;  // 카메라 이동 부드러움 정도

    private void LateUpdate()
    {
        if (target == null)
            return;

        // 목표 위치 계산 (플레이어 위치 + 오프셋)
        Vector3 desiredPosition = target.position + offset;

        // 부드러운 이동을 위해 Lerp 사용
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 카메라 위치 업데이트
        transform.position = desiredPosition;

        // 카메라가 플레이어를 바라보도록 설정
        transform.LookAt(target);
    }
}
