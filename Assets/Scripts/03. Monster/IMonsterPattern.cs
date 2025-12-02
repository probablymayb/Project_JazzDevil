using UnityEngine;

public interface IMonsterPattern
{
    void AttackPattern(Transform transform, Transform player, Animator animator, MonsterSO monsterData);
}

public class MeleePattern : IMonsterPattern
{
    public void AttackPattern(Transform transform, Transform player, Animator animator, MonsterSO monsterData)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            Debug.Log("Player Damaged : " + monsterData.attackDamage);
            playerController.TakeDamage(monsterData.attackDamage); // 플레이어 체력 감소
            animator.SetBool("isWindup", false);
            animator.SetBool("isAttack", true);
        }
    }
}

public class RangedPattern : IMonsterPattern
{
    private GameObject bulletPref;
    private BulletSO bulletSO;

    // 생성자 (탄환 프리팹 초기화)
    public RangedPattern(GameObject bulletPref, BulletSO bulletSO)
    {
        this.bulletPref = bulletPref;
        this.bulletSO = bulletSO;
    }
    
    // 패턴
    public void AttackPattern(Transform transform, Transform player, Animator animator, MonsterSO monsterData)
    {
        GameObject newBullet = PoolManager.Instance.Get(bulletPref);    // 풀 매니저로 탄 생성

        newBullet.SetActive(false);                                     // 잠시 비활성화

        newBullet.transform.position = transform.position + new Vector3(0f, 0.5f, 0f);  // 위치 설정
        Bullet bulletComp = newBullet.GetComponent<Bullet>();                           // 컴포넌트 가져오기
        bulletComp.PoolPrefRef = bulletPref;                                            // 풀 반환용 프리팹 Set
        bulletComp.BulletSpeed = bulletSO.bulletSpeed;                                  // 탄속 Set

        // 웨이브별 총알 데미지 계산
        WaveManager waveManager = Object.FindFirstObjectByType<WaveManager>();
        int currentWave = waveManager != null ? waveManager.currentWave : 1;
        bulletComp.Damage = bulletSO.GetDamageForWave(currentWave);
        
        bulletComp.Damage = bulletSO.bulletDamage;                                      // 공격력 Set
        Vector3 direction = player.position - newBullet.transform.position;
        bulletComp.Direction = new Vector3(direction.x, 0f, direction.z);               // 방향 Set
        bulletComp.Direction = bulletComp.Direction.normalized;
        bulletComp.IsPenetrable = false;
        bulletComp.Friendly = Bullet.EFriendly.Monster;

        newBullet.SetActive(true);                                      // 초기화 완료했으니 다시 활성화

        animator.SetBool("isWindup", false);
        animator.SetBool("isAttack", true);
    }
}

public class BossPattern : IMonsterPattern
{
    private Transform transform;
    private Transform player;
    private Animator animator;
    private MonsterSO monsterData;

    public void AttackPattern(Transform transform, Transform player, Animator animator, MonsterSO monsterData)
    {
        this.transform = transform;
        this.player = player;
        this.animator = animator;
        this.monsterData = monsterData;

        int randomPattern = Random.Range(0, 1);
        switch (randomPattern)
        {
            case 0:
                CircleAttack();
                break;
            default:
                Debug.LogError("보스 패턴이 올바르게 선택되지 않음.");
                break;
        }
    }

    // 원형 공격
    private void CircleAttack()
    {
        Transform circleAttack = transform.Find("Circle Attack");
        if (circleAttack == null)
        {
            Debug.LogError("Circle Attack 찾지 못함");
            return;
        }

        CapsuleCollider col = circleAttack.GetComponent<CapsuleCollider>();
        if (col == null)
        {
            Debug.LogError("보스의 Circle Attack에 있는 콜라이더를 찾지 못함.");
            return;
        }

        // 콜라이더를 잠깐 키고 꺼서 플레이어와 닿아있으면 데미지를 줌
        col.enabled = true;

        if (col.isTrigger)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Debug.Log("Player Damaged : " + monsterData.attackDamage);
                playerController.TakeDamage(monsterData.attackDamage); // 플레이어 체력 감소
                animator.SetBool("isWindup", false);
                animator.SetBool("isAttack", true);
            }
        }

        col.enabled = false;
    }
}
