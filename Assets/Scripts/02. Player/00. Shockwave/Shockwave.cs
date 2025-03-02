using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shockwave : MonoBehaviour
{
    [Header("Effect Settings")]
    [SerializeField] private float lifetime = 1.0f;
    [SerializeField] private float expandSpeed = 2.0f;
    [SerializeField] private float maxRadius = 1.5f;

    [Header("Damage Settings")]
    [SerializeField] private int defaultDamage = 1;
    private int damageAmount;

    [Header("References")]
    [SerializeField] private LayerMask enemyLayer;

    // 충돌체 참조
    private SphereCollider damageCollider;

    // 이미 데미지를 준 몬스터 추적
    private HashSet<int> damagedMonsterIds = new HashSet<int>();

    private void Awake()
    {
        // 데미지 적용용 콜라이더 추가
        damageCollider = GetComponent<SphereCollider>();
        if (damageCollider == null)
        {
            damageCollider = gameObject.AddComponent<SphereCollider>();
        }

        damageCollider.isTrigger = true;
        damageCollider.radius = 0.1f; // 시작 크기는 작게

        // 기본 적 레이어 설정
        if (enemyLayer.value == 0)
        {
            enemyLayer = 1 << LayerMask.NameToLayer("Enemy");
        }

        // 기본 데미지 설정
        damageAmount = defaultDamage;
    }

    private void Start()
    {
        // 시간이 지나면 자동 제거
        Destroy(gameObject, lifetime);

        // 점점 커지는 콜라이더
        StartCoroutine(ExpandCollider());
    }

    // 외부에서 데미지 값 설정
    public void Initialize(int damage)
    {
        damageAmount = damage;
    }

    // 콜라이더를 점점 키우는 코루틴
    private System.Collections.IEnumerator ExpandCollider()
    {
        float elapsed = 0;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / lifetime;

            // 콜라이더 크기 업데이트
            float currentRadius = Mathf.Lerp(0, maxRadius, normalizedTime);
            damageCollider.radius = currentRadius;

            // 시각적 효과 스케일 조정 (Visual이 있는 경우)
            Transform visual = transform.Find("Visual");
            if (visual != null)
            {
                float scale = currentRadius * 2; // 직경으로 변환
                visual.localScale = new Vector3(scale, scale, scale);
            }

            yield return null;
        }
    }

    // 충격파 콜라이더와 충돌하는 몬스터에 데미지
    private void OnTriggerEnter(Collider other)
    {
        // 적 레이어인지 확인
        if (((1 << other.gameObject.layer) & enemyLayer.value) != 0)
        {
            Monster monster = other.GetComponent<Monster>();
            if (monster != null)
            {
                int monsterId = monster.GetInstanceID();

                // 같은 몬스터에 중복 데미지 방지
                if (!damagedMonsterIds.Contains(monsterId))
                {
                    monster.TakeDamage(damageAmount);
                    damagedMonsterIds.Add(monsterId);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius);
    }
}