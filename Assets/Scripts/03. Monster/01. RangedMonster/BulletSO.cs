using UnityEngine;

[CreateAssetMenu(fileName = "BulletSO", menuName = "Scriptable Objects/BulletSO")]
public class BulletSO : ScriptableObject
{
    [Header("기본 스탯")]
    public float bulletSpeed;
    public int bulletDamage;

    [Header("웨이브 스케일 설정")]
    public bool useWaveScaling = true;
    [Tooltip("웨이브당 추가 데미지")]
    public int damagePerWave = 1;

    // 원본 캐시
    [System.NonSerialized] private bool _baseCached = false;
    [System.NonSerialized] private int _baseBulletDamage;

    public void CacheBase()
    {
        if (_baseCached) return;
        _baseBulletDamage = bulletDamage;
        _baseCached = true;
    }

    /// <summary>
    /// 웨이브별 총알 데미지 계산
    /// </summary>
    public int GetDamageForWave(int waveNumber)
    {
        if (!_baseCached) CacheBase();

        if (!useWaveScaling || waveNumber <= 1)
            return _baseBulletDamage;

        return _baseBulletDamage + (damagePerWave * (waveNumber - 1));
    }

    public void ResetToBase()
    {
        if (!_baseCached) return;
        bulletDamage = _baseBulletDamage;
    }
}
