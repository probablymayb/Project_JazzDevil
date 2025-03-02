using UnityEngine;

public class Monster : MonoBehaviour
{
    public float speed = 1.0f;           // 몬스터 이동 속도
    public int maxHealth = 3;            // 몬스터 최대 체력
    private int currentHealth;
    private Transform player;
    private float damageTimer = 0f;
    private float damageInterval = 1f;   // 1초 간격으로 데미지
    private float damageRange = 1f;      // 플레이어와 1 이내 거리에서 데미지

    [SerializeField] private int attackDamage = 1;
    [HideInInspector] public bool isClone = false; // 복제된 몬스터 여부
    public Vector3 fixedPosition = new Vector3(0f, 0f, 0f); // 원본 몬스터 위치 고정

    void Start()
    {
        // "Player" 태그가 있는 오브젝트 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // 몬스터 개별 체력 초기화
        currentHealth = maxHealth;

        // 클론 여부에 따라 동작 결정
        if (!isClone)
        {
            transform.position = fixedPosition; // 원본 몬스터 고정 위치
            speed = 0f; // 원본 몬스터는 움직임 불가
        }
    }

    void Update()
    {
        // 원본 몬스터는 움직이지 않음
        if (!isClone) return;

        if (player != null)
        {
            // 플레이어를 향해 이동
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // 몬스터가 플레이어를 바라보게 회전
            transform.LookAt(player);

            // 거리 확인
            float distance = Vector3.Distance(transform.position, player.position);

            // 1초마다 플레이어에게 데미지
            if (distance <= damageRange)
            {
                damageTimer += Time.deltaTime;

                if (damageTimer >= damageInterval)
                {
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        Debug.Log("Player Damaged : " + attackDamage);
                        playerController.TakeDamage(attackDamage); // 플레이어 체력 1 감소
                    }
                    damageTimer = 0f; // 타이머 초기화
                }
            }
            else
            {
                damageTimer = 0f; // 거리가 멀어지면 타이머 초기화
            }
        }
    }

    // 몬스터 체력 감소
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Monster took {damage} damage, current HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 몬스터 제거
    private void Die()
    {
        Debug.Log("Monster is Dead!");
        Destroy(gameObject); // 삭제는 클론만 가능 (원본은 움직이지 않아서 공격받지 않음)
    }
}
