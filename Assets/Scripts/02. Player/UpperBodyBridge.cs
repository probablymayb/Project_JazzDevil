using UnityEngine;

public class UpperBodyBridge : MonoBehaviour
{

    private PlayerController playerController;
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("no player controller ");
        }
    }


    //UpperBody Animation
    public void OnAttackStart()
    {
        Debug.Log("[PlayerController] Attack Start Event - 0% 지점");

        // 역방향 완료 시 isAttack을 false로 설정
        if (playerController != null)
        {
            playerController.OnAttackStart();
        }
    }
}
