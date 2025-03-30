using UnityEngine;

public class Monster : MonoBehaviour
{
    public MonsterSO monsterData;   // 스크립터블 오브젝트 연결
    private Transform player;       // 플레이어
    private float currentHealth;    // 현재 체력
    private float attackTimer;      // 공격 타이머

    private void Start()
    {
        // "Player" 태그가 있는 오브젝트 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        if (monsterData != null)
        {
            currentHealth = monsterData.maxHealth;
        }
        else
        {
            Debug.LogError("EnemySO 에셋이 할당되지 않음.");
        }
        attackTimer = 0f;
    }

    private void FixedUpdate()
    {
        Move();
        Attack();
    }

    // 움직임
    private void Move()
    {
        // TODO : 비트에 맞춰 이동하도록 수정 필요.
        // 플레이어를 향해 이동
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * monsterData.speed * Time.deltaTime;

        // 몬스터가 플레이어를 바라보게 회전
        transform.LookAt(player);
    }

    // 공격 로직
    private void Attack()
    {
        // 거리 확인
        float distance = Vector3.Distance(transform.position, player.position);

        // 1초마다 플레이어에게 데미지
        if (distance <= monsterData.attackRange)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= monsterData.attackWindup)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    Debug.Log("Player Damaged : " + monsterData.attackDamage);
                    playerController.TakeDamage(monsterData.attackDamage); // 플레이어 체력 1 감소
                }
                attackTimer = 0f; // 타이머 초기화
            }
        }
        else
        {
            attackTimer = 0f; // 거리가 멀어지면 타이머 초기화
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

    // 몬스터 제거 (오브젝트 풀로 반환)
    private void Die()
    {
        Debug.Log("Monster is Dead!");
        // TODO : 오브젝트 풀링 추가 후 아래와 같이 풀로 반환하는 코드 작성 해야 함.
        // EnemyPool.Instance.ReturnToPool(this);
    }
}
