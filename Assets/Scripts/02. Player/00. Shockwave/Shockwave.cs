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
    
    [Header("Hit Effect")]
    [SerializeField] private GameObject hitEffectPrefab; // CFXR3 프리팹 할당

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
        damageCollider.radius = 0.1f;

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
        Destroy(gameObject, lifetime);
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
        Debug.Log("sw attack");
        // 적 레이어인지 확인 (비트 연산 사용)

        //if (other.CompareTag("Player"))
        //{
        //    return;
        //}

        // 충돌한 객체의 상세 정보 출력
        Debug.Log("충돌한 객체: " + other.gameObject.name +
                  "\n레이어: " + LayerMask.LayerToName(other.gameObject.layer) +
                  "\n태그: " + other.gameObject.tag +
                  "\n부모: " + (other.transform.parent ? other.transform.parent.name : "없음"));

        Debug.Log("enemyLayer.value: " + enemyLayer.value + ", Monster layer: " + other.gameObject.layer + ", Converted layer: " + (1 << other.gameObject.layer));

        if (((1 << other.gameObject.layer) & enemyLayer.value) != 0)
        {
            Debug.Log("sw Enemy Detected");
            Monster monster = other.GetComponentInParent<Monster>();
            if (monster != null)
            {
                int monsterId = monster.GetInstanceID();
                
                if (!damagedMonsterIds.Contains(monsterId))
                {
                    monster.TakeDamage(damageAmount);
                    damagedMonsterIds.Add(monsterId);
                    
                    // 피격 이펙트 생성
                    SpawnHitEffect(other.transform.position);
                    
                    Debug.Log($"[Shockwave] Monster damaged: {damageAmount}");
                }
            }
        }
    }

    /// <summary>
    /// 피격 위치에 이펙트 생성
    /// </summary>
    private void SpawnHitEffect(Vector3 hitPosition)
    {
        if (hitEffectPrefab == null)
        {
            Debug.LogWarning("[Shockwave] hitEffectPrefab이 할당되지 않음!");
            return;
        }

        // ✅ 어떤 프리팹이 사용되는지 확인
        Debug.Log($"[Shockwave] 사용 중인 이펙트: {hitEffectPrefab.name}");

        GameObject effect = PoolManager.Instance.Get(hitEffectPrefab);
        if (effect != null)
        {
            Debug.Log($"[Shockwave] 생성된 이펙트 인스턴스: {effect.name} at {hitPosition}");
            
            effect.transform.position = hitPosition + Vector3.up * 0.5f;
            effect.transform.rotation = Quaternion.identity;
            effect.SetActive(true);
            
            ParticleSystem[] particles = effect.GetComponentsInChildren<ParticleSystem>();
            Debug.Log($"[Shockwave] 파티클 시스템 수: {particles.Length}");
            
            foreach (var ps in particles)
            {
                ps.Clear();
                ps.Play();
                Debug.Log($"[Shockwave] 파티클 재생: {ps.name}");
            }
            
            float maxDuration = 0f;
            foreach (var ps in particles)
            {
                if (ps.main.duration > maxDuration)
                    maxDuration = ps.main.duration + ps.main.startLifetime.constantMax;
            }
            
            Debug.Log($"[Shockwave] 이펙트 지속시간: {maxDuration}초");
            StartCoroutine(ReturnEffectToPool(effect, maxDuration));
        }
    }

    private IEnumerator ReturnEffectToPool(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (effect != null && PoolManager.Instance != null)
        {
            PoolManager.Instance.Return(hitEffectPrefab, effect);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius);
    }
}
