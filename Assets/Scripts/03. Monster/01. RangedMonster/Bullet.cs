using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum EFriendly { Player, Monster }

    // 캡슐화
    public GameObject PoolPrefRef { get; set; } // 풀 반환용 참조
    public Vector3 Direction { get; set; }      // 방향
    public float BulletSpeed { get; set; }      // 탄속
    public int Damage { get; set; }             // 탄 공격력
    public bool IsPenetrable { get; set; }      // 관통 가능 여부
    public EFriendly Friendly { get; set; }

    private void FixedUpdate()
    {
        transform.position += Direction * BulletSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player") && Friendly == EFriendly.Monster)
        {
            return;
        }
        if (!collision.gameObject.CompareTag("Monster") && Friendly == EFriendly.Player)
        {
            return;
        }

        if (Friendly == EFriendly.Monster)
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(Damage); // 플레이어 체력 감소
            }
        }
        if (Friendly == EFriendly.Player)
        {
            Debug.Log("test");
            Monster monsterComp = collision.gameObject.GetComponentInParent<Monster>();
            if (monsterComp != null)
            {
                monsterComp.TakeDamage(Damage);
            }
        }

        if (!IsPenetrable)
        {
            PoolManager.Instance.Return(PoolPrefRef, gameObject); // 풀로 반환
        }
    }
}
