using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 캡슐화
    public GameObject PoolPrefRef { get; set; } // 풀 반환용 참조
    public Vector3 Direction { get; set; }      // 방향
    public float BulletSpeed { get; set; }      // 탄속
    public int Damage { get; set; }             // 탄 공격력

    private void FixedUpdate()
    {
        transform.position += Direction * BulletSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            Debug.Log("Player Damaged : " + Damage);
            playerController.TakeDamage(Damage); // 플레이어 체력 감소
            PoolManager.Instance.Return(PoolPrefRef, gameObject); // 데미지를 주면 풀로 반환
        }
    }
}
