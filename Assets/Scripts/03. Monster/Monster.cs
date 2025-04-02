using UnityEngine;

abstract public class Monster : MonoBehaviour
{
    public MonsterSO monsterData;   // ½ºÅ©¸³ÅÍºí ¿ÀºêÁ§Æ® ¿¬°á
    private Transform player;       // ÇÃ·¹ÀÌ¾î
    private float currentHealth;    // ÇöÀç Ã¼·Â
    private float attackTimer = 0f;      // °ø°İ Å¸ÀÌ¸Ó

<<<<<<< HEAD
    protected IMonsterPattern AttackPattern = null;
=======
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private int goldReward = 10; // ì´ ëª¬ìŠ¤í„°ë¥¼ ì²˜ì¹˜í•˜ë©´ ì£¼ëŠ” ê³¨ë“œ
    [HideInInspector] public bool isClone = false; // ë³µì œëœ ëª¬ìŠ¤í„° ì—¬ë¶€
    public Vector3 fixedPosition = new Vector3(0f, 0f, 0f); // ì›ë³¸ ëª¬ìŠ¤í„° ìœ„ì¹˜ ê³ ì •
>>>>>>> test

    private Animator animator;

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();

        // "Player" ÅÂ±×°¡ ÀÖ´Â ¿ÀºêÁ§Æ® Ã£±â
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
            Debug.LogError("EnemySO ¿¡¼ÂÀÌ ÇÒ´çµÇÁö ¾ÊÀ½.");
        }
    }

    protected virtual void FixedUpdate()
    {
        Move();
        Attack();
    }

    // ¿òÁ÷ÀÓ
    private void Move()
    {
        // TODO : ºñÆ®¿¡ ¸ÂÃç ÀÌµ¿ÇÏµµ·Ï ¼öÁ¤ ÇÊ¿ä.
        // ÇÃ·¹ÀÌ¾î¸¦ ÇâÇØ ÀÌµ¿
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * monsterData.speed * Time.deltaTime;

        // ¸ó½ºÅÍ°¡ ÇÃ·¹ÀÌ¾î¸¦ ¹Ù¶óº¸°Ô È¸Àü
        transform.LookAt(player);
    }

    // °ø°İ ·ÎÁ÷
    private void Attack()
    {
        // °Å¸® È®ÀÎ
        float distance = Vector3.Distance(transform.position, player.position);
        
        // TODO : ÃÊ ´ÜÀ§°¡ ¾Æ´Ï¶ó ºñÆ® ´ÜÀ§·Î º¯°æ ÇÊ¿ä
        // WindupÃÊ¸¶´Ù ÇÃ·¹ÀÌ¾î¿¡°Ô µ¥¹ÌÁö
        if (distance <= monsterData.attackRange)
        {
            animator.SetBool("isWindup", true); // ÁØºñ µ¿ÀÛ ¾Ö´Ï¸ŞÀÌ¼Ç

            attackTimer += Time.deltaTime;

            if (attackTimer >= monsterData.attackWindup)
            {
                AttackPattern?.AttackPattern(player, animator, monsterData);
                attackTimer = 0f; // Å¸ÀÌ¸Ó ÃÊ±âÈ­
            }
        }
        else
        {
            attackTimer = 0f; // °Å¸®°¡ ¸Ö¾îÁö¸é Å¸ÀÌ¸Ó ÃÊ±âÈ­
            animator.SetBool("isWindup", false);
            animator.SetBool("isAttack", false);
        }
    }

    // ¸ó½ºÅÍ Ã¼·Â °¨¼Ò
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Monster took {damage} damage, current HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ¸ó½ºÅÍ Á¦°Å (¿ÀºêÁ§Æ® Ç®·Î ¹İÈ¯)
    private void Die()
    {
        Debug.Log("Monster is Dead!");
<<<<<<< HEAD
        // TODO : ¿ÀºêÁ§Æ® Ç®¸µ Ãß°¡ ÈÄ ¾Æ·¡¿Í °°ÀÌ Ç®·Î ¹İÈ¯ÇÏ´Â ÄÚµå ÀÛ¼º ÇØ¾ß ÇÔ.
        // EnemyPool.Instance.ReturnToPool(this);
=======

        // í”Œë ˆì´ì–´ì—ê²Œ ê³¨ë“œ ì§€ê¸‰*********
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerController pc = playerObj.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.AddGold(goldReward);
            }
        }

        Destroy(gameObject); // ì‚­ì œëŠ” í´ë¡ ë§Œ ê°€ëŠ¥ (ì›ë³¸ì€ ì›€ì§ì´ì§€ ì•Šì•„ì„œ ê³µê²©ë°›ì§€ ì•ŠìŒ)
>>>>>>> test
    }


    // ê³µê²©ë ¥ ì„¤ì • í•¨ìˆ˜ (ìŠ¤í¬ë„ˆì—ì„œ í˜¸ì¶œ)*****
    public void SetAttackDamage(int damage)
    {
        attackDamage = damage;
    }

    // ì²´ë ¥ì„ maxHealthë¡œ ì´ˆê¸°í™”*****
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}
