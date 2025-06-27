using UnityEngine;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using System.Collections;

public class AudioManager : Singleton<AudioManager>
{
    [Header("볼륨 설정")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    // FMOD 버스
    private Bus masterBus;
    private Bus sfxBus;

    // 재생 중인 루프 사운드 관리를 위한 딕셔너리
    private Dictionary<string, EventInstance> loopingSounds = new Dictionary<string, EventInstance>();

    // 독립적인 원샷 사운드 인스턴스들을 관리하는 리스트
    private List<EventInstance> independentInstances = new List<EventInstance>();

    protected override void Awake()
    {
        base.Awake();
        InitializeBuses();
    }

    private void InitializeBuses()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        sfxBus.setVolume(sfxVolume);

        // 완료된 독립 인스턴스들 정리
        CleanupCompletedInstances();
    }

    private void OnDestroy()
    {
        StopAllLoopingSounds();
        StopAllIndependentInstances();
    }

    /// <summary>
    /// 독립적인 채널로 원샷 사운드 재생 (3D 위치)
    /// </summary>
    /// <param name="sound">FMOD 이벤트 레퍼런스</param>
    /// <param name="position">3D 위치</param>
    /// <returns>생성된 이벤트 인스턴스</returns>
    public EventInstance PlayOneShotIndependent(EventReference sound, Vector3 position)
    {
        if (sound.IsNull)
        {
            Debug.LogWarning("AudioManager: 유효하지 않은 이벤트 레퍼런스입니다");
            return default;
        }

        // 새로운 독립적인 인스턴스 생성
        EventInstance instance = RuntimeManager.CreateInstance(sound);

        // 3D 위치 설정
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

        // 즉시 재생
        instance.start();

        // 독립 인스턴스 목록에 추가
        independentInstances.Add(instance);

        Debug.Log($"독립 채널로 사운드 재생 at {position}");
        return instance;
    }

    /// <summary>
    /// 독립적인 채널로 원샷 사운드 재생 (2D)
    /// </summary>
    /// <param name="sound">FMOD 이벤트 레퍼런스</param>
    /// <returns>생성된 이벤트 인스턴스</returns>
    public EventInstance PlayOneShotIndependent(EventReference sound)
    {
        if (sound.IsNull)
        {
            Debug.LogWarning("AudioManager: 유효하지 않은 이벤트 레퍼런스입니다");
            return default;
        }

        EventInstance instance = RuntimeManager.CreateInstance(sound);
        instance.start();
        independentInstances.Add(instance);

        Debug.Log("독립 채널로 2D 사운드 재생");
        return instance;
    }

    /// <summary>
    /// 독립적인 채널로 원샷 사운드 재생 (경로 문자열, 3D)
    /// </summary>
    /// <param name="fmodPath">FMOD 이벤트 경로</param>
    /// <param name="position">3D 위치</param>
    /// <returns>생성된 이벤트 인스턴스</returns>
    public EventInstance PlayOneShotIndependent(string fmodPath, Vector3 position)
    {
        if (string.IsNullOrEmpty(fmodPath))
        {
            Debug.LogWarning("AudioManager: 유효하지 않은 FMOD 경로입니다");
            return default;
        }

        EventInstance instance = RuntimeManager.CreateInstance(fmodPath);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        instance.start();
        independentInstances.Add(instance);

        Debug.Log($"독립 채널로 사운드 재생: {fmodPath} at {position}");
        return instance;
    }

    /// <summary>
    /// 독립적인 채널로 원샷 사운드 재생 (경로 문자열, 2D)
    /// </summary>
    /// <param name="fmodPath">FMOD 이벤트 경로</param>
    /// <returns>생성된 이벤트 인스턴스</returns>
    public EventInstance PlayOneShotIndependent(string fmodPath)
    {
        if (string.IsNullOrEmpty(fmodPath))
        {
            Debug.LogWarning("AudioManager: 유효하지 않은 FMOD 경로입니다");
            return default;
        }

        EventInstance instance = RuntimeManager.CreateInstance(fmodPath);
        instance.start();
        independentInstances.Add(instance);

        Debug.Log($"독립 채널로 2D 사운드 재생: {fmodPath}");
        return instance;
    }

    /// <summary>
    /// 파라미터와 함께 독립적인 채널로 사운드 재생
    /// </summary>
    /// <param name="sound">FMOD 이벤트 레퍼런스</param>
    /// <param name="position">3D 위치</param>
    /// <param name="parameters">파라미터 딕셔너리</param>
    /// <returns>생성된 이벤트 인스턴스</returns>
    public EventInstance PlayOneShotIndependentWithParams(EventReference sound, Vector3 position, Dictionary<string, float> parameters = null)
    {
        EventInstance instance = PlayOneShotIndependent(sound, position);

        if (instance.isValid() && parameters != null)
        {
            foreach (var param in parameters)
            {
                instance.setParameterByName(param.Key, param.Value);
            }
        }

        return instance;
    }

    /// <summary>
    /// 완료된 독립 인스턴스들 정리
    /// </summary>
    private void CleanupCompletedInstances()
    {
        for (int i = independentInstances.Count - 1; i >= 0; i--)
        {
            if (i >= independentInstances.Count) continue; // 안전 체크

            EventInstance instance = independentInstances[i];

            if (!instance.isValid())
            {
                independentInstances.RemoveAt(i);
                continue;
            }

            // 재생 상태 확인
            PLAYBACK_STATE state;
            FMOD.RESULT result = instance.getPlaybackState(out state);

            if (result != FMOD.RESULT.OK || state == PLAYBACK_STATE.STOPPED)
            {
                // 인스턴스 해제 및 목록에서 제거
                instance.release();
                independentInstances.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 모든 독립 인스턴스 중지
    /// </summary>
    public void StopAllIndependentInstances()
    {
        foreach (var instance in independentInstances)
        {
            if (instance.isValid())
            {
                instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                instance.release();
            }
        }
        independentInstances.Clear();
    }

    /// <summary>
    /// 특정 독립 인스턴스 중지
    /// </summary>
    /// <param name="instance">중지할 인스턴스</param>
    public void StopIndependentInstance(EventInstance instance)
    {
        if (instance.isValid())
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            independentInstances.Remove(instance);
        }
    }

    // 기존 PlayOneShot 메서드들은 그대로 유지...
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

    // 기존 루핑 사운드 메서드들도 그대로 유지...
    public EventInstance PlayLooping(EventReference sound, string key)
    {
        return PlayLooping(sound, key, Vector3.zero, null);
    }

    public EventInstance PlayLooping(EventReference sound, string key, Vector3 position)
    {
        return PlayLooping(sound, key, position, null);
    }

    public EventInstance PlayLooping(EventReference sound, string key, GameObject followTarget)
    {
        return PlayLooping(sound, key, followTarget.transform.position, followTarget);
    }

    private EventInstance PlayLooping(EventReference sound, string key, Vector3 position, GameObject followTarget)
    {
        if (sound.IsNull)
        {
            Debug.LogWarning("AudioManager: 유효하지 않은 이벤트 레퍼런스입니다");
            return default;
        }

        StopLoopingSound(key);

        EventInstance instance = RuntimeManager.CreateInstance(sound);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

        if (followTarget != null)
        {
            StartCoroutine(FollowTarget(instance, followTarget));
        }

        instance.start();
        loopingSounds[key] = instance;

        return instance;
    }

    public EventInstance PlayLooping(string fmodPath, string key, Vector3 position = default, GameObject followTarget = null)
    {
        if (string.IsNullOrEmpty(fmodPath))
        {
            Debug.LogWarning("AudioManager: 유효하지 않은 FMOD 경로입니다");
            return default;
        }

        StopLoopingSound(key);

        EventInstance instance = RuntimeManager.CreateInstance(fmodPath);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

        if (followTarget != null)
        {
            StartCoroutine(FollowTarget(instance, followTarget));
        }

        instance.start();
        loopingSounds[key] = instance;

        return instance;
    }

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

    public void StopLoopingSound(string key)
    {
        if (loopingSounds.TryGetValue(key, out EventInstance instance))
        {
            instance.getPlaybackState(out PLAYBACK_STATE state);

            if (state != PLAYBACK_STATE.STOPPED)
            {
                instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                instance.release();
            }

            loopingSounds.Remove(key);
        }
    }

    public void StopAllLoopingSounds()
    {
        foreach (var instance in loopingSounds.Values)
        {
            instance.getPlaybackState(out PLAYBACK_STATE state);

            if (state != PLAYBACK_STATE.STOPPED)
            {
                instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                instance.release();
            }
        }

        loopingSounds.Clear();
    }

    /// <summary>
    /// 모든 루프 사운드 일시정지
    /// </summary>
    public void PauseAllLoopingSounds()
    {
        foreach (var instance in loopingSounds.Values)
        {
            instance.getPaused(out bool isPaused);

            if (!isPaused)
            {
                instance.setPaused(true);
            }
        }
    }

    /// <summary>
    /// 일시정지된 모든 루프 사운드 일시정지 해재.
    /// </summary>
    public void ResumeAllLoopingSounds()
    {
        foreach (var instance in loopingSounds.Values)
        {
            instance.getPaused(out bool isPaused);

            if (isPaused)
            {
                instance.setPaused(false);
            }
        }
    }

    public void SetLoopingParameter(string key, string parameterName, float value)
    {
        if (loopingSounds.TryGetValue(key, out EventInstance instance))
        {
            instance.setParameterByName(parameterName, value);
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    // 디버그 정보 표시
    [Header("디버그 정보")]
    [SerializeField] private bool showDebugInfo = true;

    private void OnGUI()
    {
        if (!showDebugInfo) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label($"독립 인스턴스 수: {independentInstances.Count}");
        GUILayout.Label($"루핑 사운드 수: {loopingSounds.Count}");

        if (GUILayout.Button("모든 독립 인스턴스 중지"))
        {
            StopAllIndependentInstances();
        }

        GUILayout.EndArea();
    }
}
