using UnityEngine;

[CreateAssetMenu(fileName = "MonsterSO", menuName = "Scriptable Objects/MonsterSO")]
public class MonsterSO : ScriptableObject
{
    [Header("기본 스탯")]
    public float speed;
    public int maxHealth;
    public int attackWindup;
    public float attackRange;
    public int attackDamage;

    [Header("웨이브 스케일 설정")]
    public bool useWaveScaling = true;
    [Tooltip("웨이브당 추가 HP")]
    public int healthPerWave = 1;
    [Tooltip("웨이브당 추가 공격력 (근접 몬스터만 사용)")]
    public int attackPerWave = 1;
    [Tooltip("웨이브당 속도 증가율 (0.05 = 5%)")]
    public float speedRatePerWave = 0.0f;
    
    [Header("원거리 몬스터 설정")]
    [Tooltip("원거리 몬스터는 자체 공격력 증가 안 함 (총알 데미지만 증가)")]
    public bool isRangedMonster = false;

    // 원본 캐시
    [System.NonSerialized] private bool _baseCached = false;
    [System.NonSerialized] private float _baseSpeed;
    [System.NonSerialized] private int _baseMaxHealth;
    [System.NonSerialized] private int _baseAttackDamage;

    public void CacheBase()
    {
        if (_baseCached) return;
        
        _baseSpeed = speed;
        _baseMaxHealth = maxHealth;
        _baseAttackDamage = attackDamage;
        _baseCached = true;
        
        Debug.Log($"[{name}] 원본 캐시 완료 - HP:{_baseMaxHealth}, ATK:{_baseAttackDamage}, SPD:{_baseSpeed}");
    }

    public void GetStatsForWave(int waveNumber, out int hp, out int atk, out float spd)
    {
        if (!_baseCached) CacheBase();

        if (!useWaveScaling || waveNumber <= 1)
        {
            hp = _baseMaxHealth;
            atk = _baseAttackDamage;
            spd = _baseSpeed;
        }
        else
        {
            int waveBonus = waveNumber - 1;
            hp = _baseMaxHealth + (healthPerWave * waveBonus);
            
            // 원거리 몬스터는 자체 공격력 증가 안 함
            atk = isRangedMonster ? _baseAttackDamage : _baseAttackDamage + (attackPerWave * waveBonus);
            
            spd = _baseSpeed * (1f + speedRatePerWave * waveBonus);
        }
    }

    public void ResetToBase()
    {
        if (!_baseCached) return;
        
        speed = _baseSpeed;
        maxHealth = _baseMaxHealth;
        attackDamage = _baseAttackDamage;
        
        Debug.Log($"[{name}] 원본 복구 완료");
    }
}
