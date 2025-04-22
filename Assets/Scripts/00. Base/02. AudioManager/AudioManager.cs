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

    // 재생 중인 루프 사운드 관리를 위한 딕셔너리
    private Dictionary<string, EventInstance> loopingSounds = new Dictionary<string, EventInstance>();

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

    private void OnDestroy()
    {
        // 앱 종료 시 모든 루프 사운드 정리
        StopAllLoopingSounds();
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
    /// 반복 재생되는 사운드 시작 (2D)
    /// </summary>
    /// <param name="sound">FMOD 이벤트 레퍼런스</param>
    /// <param name="key">사운드 식별을 위한 고유 키</param>
    /// <returns>생성된 이벤트 인스턴스</returns>
    public EventInstance PlayLooping(EventReference sound, string key)
    {
        return PlayLooping(sound, key, Vector3.zero, null);
    }

    /// <summary>
    /// 반복 재생되는 3D 사운드 시작 (위치 정보 포함)
    /// </summary>
    /// <param name="sound">FMOD 이벤트 레퍼런스</param>
    /// <param name="key">사운드 식별을 위한 고유 키</param>
    /// <param name="position">3D 위치</param>
    /// <returns>생성된 이벤트 인스턴스</returns>
    public EventInstance PlayLooping(EventReference sound, string key, Vector3 position)
    {
        return PlayLooping(sound, key, position, null);
    }

    /// <summary>
    /// 반복 재생되는 3D 사운드 시작 (위치 자동 업데이트)
    /// </summary>
    /// <param name="sound">FMOD 이벤트 레퍼런스</param>
    /// <param name="key">사운드 식별을 위한 고유 키</param>
    /// <param name="followTarget">위치를 따라갈 게임 오브젝트</param>
    /// <returns>생성된 이벤트 인스턴스</returns>
    public EventInstance PlayLooping(EventReference sound, string key, GameObject followTarget)
    {
        return PlayLooping(sound, key, followTarget.transform.position, followTarget);
    }

    /// <summary>
    /// 반복 재생되는 3D 사운드 시작 (내부 구현)
    /// </summary>
    private EventInstance PlayLooping(EventReference sound, string key, Vector3 position, GameObject followTarget)
    {
        if (sound.IsNull)
        {
            Debug.LogWarning("AudioManager: 유효하지 않은 이벤트 레퍼런스입니다");
            return default;
        }

        // 이미 해당 키로 재생 중인 사운드가 있으면 중지
        StopLoopingSound(key);

        // 새 이벤트 인스턴스 생성
        EventInstance instance = RuntimeManager.CreateInstance(sound);

        // 3D 위치 설정
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

        // 위치 추적 코루틴 시작 (타겟이 있는 경우)
        if (followTarget != null)
        {
            StartCoroutine(FollowTarget(instance, followTarget));
        }

        // 사운드 시작
        instance.start();

        // 딕셔너리에 추가
        loopingSounds[key] = instance;

        return instance;
    }

    /// <summary>
    /// 경로 문자열로 반복 재생되는 사운드 시작
    /// </summary>
    /// <param name="fmodPath">FMOD 이벤트 경로</param>
    /// <param name="key">사운드 식별을 위한 고유 키</param>
    /// <param name="position">3D 위치 (선택사항)</param>
    /// <param name="followTarget">위치를 따라갈 게임 오브젝트 (선택사항)</param>
    /// <returns>생성된 이벤트 인스턴스</returns>
    public EventInstance PlayLooping(string fmodPath, string key, Vector3 position = default, GameObject followTarget = null)
    {
        if (string.IsNullOrEmpty(fmodPath))
        {
            Debug.LogWarning("AudioManager: 유효하지 않은 FMOD 경로입니다");
            return default;
        }

        // 이미 해당 키로 재생 중인 사운드가 있으면 중지
        StopLoopingSound(key);

        // 새 이벤트 인스턴스 생성
        EventInstance instance = RuntimeManager.CreateInstance(fmodPath);

        // 3D 위치 설정
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

        // 위치 추적 코루틴 시작 (타겟이 있는 경우)
        if (followTarget != null)
        {
            StartCoroutine(FollowTarget(instance, followTarget));
        }

        // 사운드 시작
        instance.start();

        // 딕셔너리에 추가
        loopingSounds[key] = instance;

        return instance;
    }

    /// <summary>
    /// 특정 대상을 따라가는 3D 사운드를 위한 코루틴
    /// </summary>
    private System.Collections.IEnumerator FollowTarget(EventInstance instance, GameObject target)
    {
        PLAYBACK_STATE playbackState = PLAYBACK_STATE.PLAYING;

        while (target != null && instance.isValid() &&
              (instance.getPlaybackState(out playbackState) == FMOD.RESULT.OK) &&
              (playbackState != PLAYBACK_STATE.STOPPED))
        {
            instance.set3DAttributes(RuntimeUtils.To3DAttributes(target));
            yield return null;
        }
    }

    /// <summary>
    /// 특정 키로 재생 중인 루프 사운드 중지
    /// </summary>
    /// <param name="key">중지할 사운드의 키</param>
    public void StopLoopingSound(string key)
    {
        if (loopingSounds.TryGetValue(key, out EventInstance instance))
        {
            // 재생 중인지 확인
            instance.getPlaybackState(out PLAYBACK_STATE state);

            if (state != PLAYBACK_STATE.STOPPED)
            {
                // 사운드 정지 및 리소스 해제
                instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                instance.release();
            }

            // 딕셔너리에서 제거
            loopingSounds.Remove(key);
        }
    }

    /// <summary>
    /// 모든 루프 사운드 중지
    /// </summary>
    public void StopAllLoopingSounds()
    {
        foreach (var instance in loopingSounds.Values)
        {
            // 재생 중인지 확인
            instance.getPlaybackState(out PLAYBACK_STATE state);

            if (state != PLAYBACK_STATE.STOPPED)
            {
                // 사운드 정지 및 리소스 해제
                instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                instance.release();
            }
        }

        // 딕셔너리 비우기
        loopingSounds.Clear();
    }

    /// <summary>
    /// 특정 키로 재생 중인 루프 사운드의 파라미터 설정
    /// </summary>
    /// <param name="key">사운드 키</param>
    /// <param name="parameterName">파라미터 이름</param>
    /// <param name="value">파라미터 값</param>
    public void SetLoopingParameter(string key, string parameterName, float value)
    {
        if (loopingSounds.TryGetValue(key, out EventInstance instance))
        {
            instance.setParameterByName(parameterName, value);
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
