using UnityEngine;

/// <summary>
/// 플레이어가 접촉하면 상점 UI를 여는 트리거 오브젝트
/// </summary>
public class ShopTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        float distance = Vector3.Distance(transform.position, other.transform.position);
        if (distance > 2.0f) return; // 너무 멀면 무시
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("[ShopTrigger] 플레이어가 트리거에 닿음");
            ShopUIManager.Instance.OpenShop(); // 싱글톤 호출
            Destroy(gameObject); // 트리거 제거
        }
    }
}
