using Unity.VisualScripting;
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

    // 레이어 인덱스 캐싱
    private int playerLayer;
    private int enemyLayer;

    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    private void FixedUpdate()
    {
        transform.position += Direction * BulletSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        bool hitHostileObject = false;

        // 플레이어 공격
        if (other.gameObject.layer == playerLayer && other.name == "Capsule" && Friendly == EFriendly.Monster)
        {
            PlayerController playerController = other.gameObject.GetComponentInParent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(Damage); // 플레이어 체력 감소
                hitHostileObject = true;
            }
        }

        // 적 공격
        else if (other.gameObject.layer == enemyLayer && Friendly == EFriendly.Player)
        {
            Monster monsterComp = other.gameObject.GetComponentInParent<Monster>();
            if (monsterComp != null)
            {
                monsterComp.TakeDamage(Damage);
                hitHostileObject = true;
            }
        }

        // 비관통 탄환은 풀로 반환
        if (hitHostileObject && !IsPenetrable)
        {
            PoolManager.Instance.Return(PoolPrefRef, gameObject); // 풀로 반환
        }
    }
}
