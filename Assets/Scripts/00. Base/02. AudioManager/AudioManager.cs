using UnityEngine;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
public class AudioManager : Singleton<AudioManager>
{
    //일반 오디오 관리: 일회성 효과음, 환경음 등 재생
    //FMOD 리소스 관리: 이벤트 인스턴스 생성, 이미터 초기화
    //볼륨 컨트롤: 전체 음량 및 각 카테고리별 음량 관리

    [Header("볼륨 설정")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    // FMOD 버스
    private Bus masterBus;
    private Bus sfxBus;

    protected override void Awake()
    {
        base.Awake();

        // FMOD 버스 초기화
        InitializeBuses();
    }

    private void InitializeBuses()
    {
        // 기본 버스 초기화
        masterBus = RuntimeManager.GetBus("bus:/");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void Update()
    {
        // 볼륨 적용
        masterBus.setVolume(masterVolume);
        sfxBus.setVolume(sfxVolume);
    }

    /// <summary>
    /// 위치 정보가 있는 원샷 사운드 재생
    /// </summary>
    /// <param name="sound">FMOD 이벤트 레퍼런스</param>
    /// <param name="position">3D 위치</param>
    public void PlayOneShot(EventReference sound, Vector3 position)
    {
        if (!sound.IsNull)
        {
            RuntimeManager.PlayOneShot(sound, position);
        }
        else
        {
            Debug.LogWarning("AudioManager: 유효하지 않은 이벤트 레퍼런스입니다");
        }
    }

    /// <summary>
    /// 위치 정보 없이 원샷 사운드 재생 (2D 사운드)
    /// </summary>
    /// <param name="sound">FMOD 이벤트 레퍼런스</param>
    public void PlayOneShot(EventReference sound)
    {
        if (!sound.IsNull)
        {
            RuntimeManager.PlayOneShot(sound);
        }
        else
        {
            Debug.LogWarning("AudioManager: 유효하지 않은 이벤트 레퍼런스입니다");
        }
    }

    /// <summary>
    /// 경로 문자열로 원샷 사운드 재생
    /// </summary>
    /// <param name="fmodPath">FMOD 이벤트 경로 (예: "event:/SFX/Explosion")</param>
    /// <param name="position">3D 위치 (선택사항)</param>
    public void PlayOneShot(string fmodPath, Vector3 position = default)
    {
        if (!string.IsNullOrEmpty(fmodPath))
        {
            RuntimeManager.PlayOneShot(fmodPath, position);
        }
        else
        {
            Debug.LogWarning("AudioManager: 유효하지 않은 FMOD 경로입니다");
        }
    }

    /// <summary>
    /// 마스터 볼륨 설정
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// SFX 볼륨 설정
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
}