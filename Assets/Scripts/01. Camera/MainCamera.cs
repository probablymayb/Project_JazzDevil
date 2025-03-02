using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;  // �÷��̾��� Transform
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -7f);  // ī�޶�� �÷��̾� ���� �Ÿ�

    [Header("Movement Settings")]
    [SerializeField] private float smoothSpeed = 5f;  // ī�޶� �̵� �ε巯�� ����

    private void LateUpdate()
    {
        if (target == null)
            return;

        // ��ǥ ��ġ ��� (�÷��̾� ��ġ + ������)
        Vector3 desiredPosition = target.position + offset;

        // �ε巯�� �̵��� ���� Lerp ���
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // ī�޶� ��ġ ������Ʈ
        transform.position = smoothedPosition;

        // ī�޶� �÷��̾ �ٶ󺸵��� ����
        transform.LookAt(target);
    }
}
