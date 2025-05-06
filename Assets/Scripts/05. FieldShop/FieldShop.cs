using UnityEngine;

/// <summary>
/// 플레이어가 접촉하면 상점 UI를 여는 트리거 오브젝트
/// </summary>
public class FieldShop : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LiveShopUIManager ui = FindFirstObjectByType<LiveShopUIManager>();
            if (ui != null) ui.OpenShop();
            else
            {
                Debug.Log(">LIVE SHOP UIO못찾음");
            }
            //Destroy(gameObject);
        }
    }
}
