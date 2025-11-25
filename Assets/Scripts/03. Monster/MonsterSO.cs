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
    [Tooltip("웨이브당 추가 공격력")]
    public int attackPerWave = 1;
    [Tooltip("웨이브당 속도 증가율 (0.05 = 5%)")]
    public float speedRatePerWave = 0.0f;

    // 원본 캐시 (런타임에서만 사용)
    [System.NonSerialized] private bool _baseCached = false;
    [System.NonSerialized] private float _baseSpeed;
    [System.NonSerialized] private int _baseMaxHealth;
    [System.NonSerialized] private int _baseAttackDamage;

    /// <summary>
    /// 게임 시작 시 원본 값 저장
    /// </summary>
    public void CacheBase()
    {
        if (_baseCached) return;

        _baseSpeed = speed;
        _baseMaxHealth = maxHealth;
        _baseAttackDamage = attackDamage;
        _baseCached = true;

        Debug.Log($"[{name}] 원본 캐시 완료 - HP:{_baseMaxHealth}, ATK:{_baseAttackDamage}, SPD:{_baseSpeed}");
    }

    /// <summary>
    /// 웨이브별 스탯 계산 (원본 + 증가치)
    /// waveNumber: 1부터 시작
    /// </summary>
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
            atk = _baseAttackDamage + (attackPerWave * waveBonus);
            spd = _baseSpeed * (1f + speedRatePerWave * waveBonus);
        }
    }

    /// <summary>
    /// 원본으로 복구 (게임 종료 시 또는 테스트용)
    /// </summary>
    public void ResetToBase()
    {
        if (!_baseCached) return;

        speed = _baseSpeed;
        maxHealth = _baseMaxHealth;
        attackDamage = _baseAttackDamage;

        Debug.Log($"[{name}] 원본 복구 완료");
    }
}
