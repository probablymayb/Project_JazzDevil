using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Debug = UnityEngine.Debug;

public class RhythmManager : Singleton<RhythmManager>
{
    [Header("FMOD 다중 이벤트 설정")]
    [SerializeField] private EventReference[] musicEvents = new EventReference[9]; // 120, 125, 130, ..., 160
    [SerializeField] private float[] availableBpms = { 120f, 125f, 130f, 135f, 140f, 145f, 150f, 155f, 160f };

    [Header("세션 파라미터")]
    [SerializeField]
    private string[] sessionParameterNames = {
        "BassVolume",
        "PianoVolume",
        "BrassVolume",
        "MelodyVolume",
        "OtherVolume"
    };
    [SerializeField]
    private string[] sessionNames = {
        "Bass",
        "Piano",
        "Brass",
        "Melody",
        "Other"
    };

    [Header("반복 설정")]
    [SerializeField] private bool loopMusic = true;  // 음악 반복 여부

    [Header("전환 설정")]
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private bool crossFade = true; // 크로스페이드 여부

    // 현재 재생 중인 이벤트
    public FMOD.Studio.EventInstance CurrentMusicInstance { get; private set; }
    private FMOD.Studio.EventInstance nextMusicInstance; // 크로스페이드용
    private int currentBpmIndex = 0;
    private int targetBpmIndex = 0;

    // 세션 상태 관리
    private bool[] sessionActive;
    private int nextSessionToActivate = 0;

    // 리듬 관련 변수
    private float currentBpm;
    private float secPerBeat;
    private bool isTransitioning = false; // BPM 전환 중인지

    // Timeline Tracker
    public TimelineInfo timelineInfo = null;
    private GCHandle timelineHandle;
    private FMOD.Studio.EVENT_CALLBACK beatCallback;

    [StructLayout(LayoutKind.Sequential)]
    public class TimelineInfo
    {
        public int currentBeat = 0;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }

    // Beat Event
    public delegate void BeatEventDelegate();
    public static event BeatEventDelegate beatUpdated;

    public delegate void MarkerListenerDelegate();
    public static event MarkerListenerDelegate markerUpdated;

    public static int lastBeat = 0;
    public static string lastMarkerString = null;

    // 이벤트 시스템
    public event Action<float, float> OnBPMChanged; // (oldBPM, newBPM)

    // 접근 프로퍼티
    public float CurrentBpm => currentBpm;
    public float SecPerBeat => secPerBeat;
    public bool IsPlaying => CurrentMusicInstance.isValid();
    public bool IsTransitioning => isTransitioning;

    protected override void Awake()
    {
        base.Awake();

        // 배열 크기 검증
        if (musicEvents.Length != availableBpms.Length)
        {
            Debug.LogError("음악 이벤트 배열과 BPM 배열의 크기가 일치하지 않습니다!");
        }

        currentBpm = availableBpms[0];
        secPerBeat = 60f / currentBpm;
        sessionActive = new bool[sessionParameterNames.Length];

        // 첫 번째 음악 이벤트로 시작
        InitializeMusicEvent(0);

    }

    /// <summary>
    /// 🆕 특정 BPM 인덱스의 음악 이벤트 초기화
    /// </summary>
    private void InitializeMusicEvent(int bpmIndex)
    {
        if (bpmIndex < 0 || bpmIndex >= musicEvents.Length || musicEvents[bpmIndex].IsNull)
        {
            Debug.LogError($"유효하지 않은 BPM 인덱스 또는 음악 이벤트: {bpmIndex}");
            return;
        }

        CurrentMusicInstance = RuntimeManager.CreateInstance(musicEvents[bpmIndex]);
        currentBpmIndex = bpmIndex;
        currentBpm = availableBpms[bpmIndex];
        secPerBeat = 60f / currentBpm;

        // 모든 세션을 0으로 초기화
        InitializeAllSessionsToZero();

        Debug.Log($"음악 이벤트 초기화: {availableBpms[bpmIndex]} BPM");
    }

    /// <summary>
    /// 모든 세션 파라미터를 0으로 초기화
    /// </summary>
    private void InitializeAllSessionsToZero()
    {
        if (!CurrentMusicInstance.isValid()) return;

        for (int i = 0; i < sessionParameterNames.Length; i++)
        {
            if (!string.IsNullOrEmpty(sessionParameterNames[i]))
            {
                CurrentMusicInstance.setParameterByName(sessionParameterNames[i], 0f);
                sessionActive[i] = false;
            }
        }
    }

    private void Start()
    {
        SetupTimelineCallback();
        StartCurrentMusic();
    }

    /// <summary>
    /// 🆕 Timeline Callback 설정
    /// </summary>
    private void SetupTimelineCallback()
    {
        if (CurrentMusicInstance.isValid())
        {
            timelineInfo = new TimelineInfo();
            beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
            timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);

            CurrentMusicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
            CurrentMusicInstance.setCallback(beatCallback,
                FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT |
                FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
        }
    }

    /// <summary>
    /// 🆕 현재 음악 시작
    /// </summary>
    private void StartCurrentMusic()
    {
        if (CurrentMusicInstance.isValid())
        {
            CurrentMusicInstance.start();
            Debug.Log($"음악 시작: {currentBpm} BPM");
        }
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != EGameState.Playing) return;

        // 음악 종료 체크
        if (loopMusic)
        {
            CheckAndRestartMusic();
        }

        // Timeline 마커 처리
        if (lastMarkerString != timelineInfo?.lastMarker)
        {
            lastMarkerString = timelineInfo?.lastMarker;
            markerUpdated?.Invoke();
        }

        // Beat 처리
        if (lastBeat != timelineInfo?.currentBeat)
        {
            lastBeat = timelineInfo?.currentBeat ?? 0;
            beatUpdated?.Invoke();
        }

#if UNITY_EDITOR

        // 테스트 키 입력
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RequestSessionActivation();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            RequestBPMIncrease();
        }
#endif

    }

    /// <summary>
    /// 음악 종료 시 재시작
    /// </summary>
    private void CheckAndRestartMusic()
    {
        if (!CurrentMusicInstance.isValid()) return;
        if (isTransitioning) return;

        FMOD.Studio.PLAYBACK_STATE state;
        CurrentMusicInstance.getPlaybackState(out state);

        if (state == FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            Debug.Log($"[RhythmManager] 음악 종료 감지, 재시작 (BPM: {currentBpm})");
            RestartCurrentMusic();
        }
    }

    /// <summary>
    /// 현재 BPM 유지하며 음악 재시작
    /// </summary>
    private void RestartCurrentMusic()
    {
        // 기존 인스턴스 정리
        if (CurrentMusicInstance.isValid())
        {
            CurrentMusicInstance.setUserData(IntPtr.Zero);
            CurrentMusicInstance.release();
        }

        // 같은 BPM 인덱스로 새 인스턴스 생성
        CurrentMusicInstance = RuntimeManager.CreateInstance(musicEvents[currentBpmIndex]);

        // 세션 상태 복원
        CopySessionStates(CurrentMusicInstance);

        // 콜백 재설정
        SetupTimelineCallback();

        // 재생 시작
        CurrentMusicInstance.start();

        // 비트 카운터 리셋
        lastBeat = 0;

        Debug.Log($"[RhythmManager] 음악 재시작 완료 (BPM: {currentBpm})");
    }
    /// <summary>
    /// 🆕 BPM 변경 요청 (다른 음원으로 전환)
    /// </summary>
    public void SetBPM(float targetBPM)
    {
        if (isTransitioning)
        {
            Debug.LogWarning("이미 BPM 전환 중입니다.");
            return;
        }

        int newBpmIndex = GetBPMIndex(targetBPM);
        if (newBpmIndex == currentBpmIndex || newBpmIndex == -1)
        {
            return; // 같은 BPM이거나 유효하지 않은 BPM
        }

        StartCoroutine(TransitionToBPM(newBpmIndex));
    }

    /// <summary>
    /// 🆕 BPM에 해당하는 인덱스 찾기
    /// </summary>
    private int GetBPMIndex(float bpm)
    {
        for (int i = 0; i < availableBpms.Length; i++)
        {
            if (Mathf.Abs(availableBpms[i] - bpm) < 0.1f)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 🆕 BPM 전환 코루틴 (음원 교체)
    /// </summary>
    private IEnumerator TransitionToBPM(int newBpmIndex)
    {
        if (newBpmIndex < 0 || newBpmIndex >= musicEvents.Length)
        {
            yield break;
        }

        isTransitioning = true;
        float oldBPM = currentBpm;
        float newBPM = availableBpms[newBpmIndex];

        Debug.Log($"🎵 BPM 전환 시작: {oldBPM} → {newBPM}");

        // 새 음악 이벤트 준비
        if (!musicEvents[newBpmIndex].IsNull)
        {
            nextMusicInstance = RuntimeManager.CreateInstance(musicEvents[newBpmIndex]);

            // 현재 세션 상태를 새 인스턴스에 복사
            CopySessionStates(nextMusicInstance);
        }
        else
        {
            Debug.LogError($"BPM {newBPM}에 해당하는 음악 이벤트가 없습니다!");
            isTransitioning = false;
            yield break;
        }

        if (crossFade)
        {
            // 크로스페이드 전환
            yield return StartCoroutine(CrossFadeTransition(newBpmIndex, newBPM, oldBPM));
        }
        else
        {
            // 즉시 전환
            yield return StartCoroutine(ImmediateTransition(newBpmIndex, newBPM, oldBPM));
        }

        isTransitioning = false;
    }

    /// <summary>
    /// 🆕 크로스페이드 전환
    /// </summary>
    private IEnumerator CrossFadeTransition(int newBpmIndex, float newBPM, float oldBPM)
    {
        // 새 음악 시작 (볼륨 0에서)
        nextMusicInstance.setVolume(0f);
        nextMusicInstance.start();

        float elapsed = 0f;
        float duration = Mathf.Max(fadeOutDuration, fadeInDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // 볼륨 크로스페이드
            if (elapsed < fadeOutDuration)
            {
                CurrentMusicInstance.setVolume(1f - (elapsed / fadeOutDuration));
            }

            if (elapsed >= (duration - fadeInDuration))
            {
                float fadeInProgress = (elapsed - (duration - fadeInDuration)) / fadeInDuration;
                nextMusicInstance.setVolume(fadeInProgress);
            }

            yield return null;
        }

        // 전환 완료
        CompleteTransition(newBpmIndex, newBPM, oldBPM);
    }

    /// <summary>
    /// 🆕 즉시 전환
    /// </summary>
    private IEnumerator ImmediateTransition(int newBpmIndex, float newBPM, float oldBPM)
    {
        // 현재 음악 정지
        if (CurrentMusicInstance.isValid())
        {
            CurrentMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        // 새 음악 시작
        nextMusicInstance.start();

        yield return null;

        // 전환 완료
        CompleteTransition(newBpmIndex, newBPM, oldBPM);
    }

    /// <summary>
    /// 🆕 전환 완료 처리
    /// </summary>
    private void CompleteTransition(int newBpmIndex, float newBPM, float oldBPM)
    {
        // 이전 인스턴스 정리
        if (CurrentMusicInstance.isValid())
        {
            CurrentMusicInstance.setUserData(IntPtr.Zero);
            CurrentMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            CurrentMusicInstance.release();
        }

        // 새 인스턴스를 현재로 설정
        CurrentMusicInstance = nextMusicInstance;
        nextMusicInstance = default;
        currentBpmIndex = newBpmIndex;
        currentBpm = newBPM;
        secPerBeat = 60f / currentBpm;

        // Timeline Callback 재설정
        SetupTimelineCallback();

        // 이벤트 발생
        OnBPMChanged?.Invoke(oldBPM, newBPM);

        Debug.Log($"🎵 BPM 전환 완료: {newBPM}");
    }

    /// <summary>
    /// 🆕 세션 상태를 새 인스턴스에 복사
    /// </summary>
    private void CopySessionStates(EventInstance newInstance)
    {
        if (!newInstance.isValid()) return;

        for (int i = 0; i < sessionParameterNames.Length; i++)
        {
            if (!string.IsNullOrEmpty(sessionParameterNames[i]))
            {
                float value = sessionActive[i] ? 1f : 0f;
                newInstance.setParameterByName(sessionParameterNames[i], value);
            }
        }
    }

    /// <summary>
    /// 콤보 시스템에서 호출 - 세션 활성화
    /// </summary>
    public void RequestSessionActivation()
    {
        ActivateNextSession();
    }

    /// <summary>
    /// 다음 세션 활성화
    /// </summary>
    public void ActivateNextSession()
    {
        if (nextSessionToActivate >= sessionParameterNames.Length)
        {
            Debug.Log("모든 세션이 이미 활성화되었습니다.");
            return;
        }

        if (!CurrentMusicInstance.isValid()) return;

        string paramName = sessionParameterNames[nextSessionToActivate];
        if (!string.IsNullOrEmpty(paramName))
        {
            CurrentMusicInstance.setParameterByName(paramName, 1f);
            sessionActive[nextSessionToActivate] = true;

            string sessionName = nextSessionToActivate < sessionNames.Length
                ? sessionNames[nextSessionToActivate]
                : $"Session {nextSessionToActivate + 1}";

            Debug.Log($"{sessionName} 세션 활성화 ({currentBpm} BPM)");
            nextSessionToActivate++;
        }
    }

    /// <summary>
    /// 콤보 시스템에서 호출 - BPM 증가
    /// </summary>
    public void RequestBPMIncrease()
    {
        if (currentBpmIndex < availableBpms.Length - 1)
        {
            SetBPM(availableBpms[currentBpmIndex + 1]);
        }
    }

    /// <summary>
    /// 모든 세션 비활성화
    /// </summary>
    public void DeactivateAllSessions()
    {
        if (!CurrentMusicInstance.isValid()) return;

        for (int i = 0; i < sessionParameterNames.Length; i++)
        {
            if (!string.IsNullOrEmpty(sessionParameterNames[i]) && sessionActive[i])
            {
                CurrentMusicInstance.setParameterByName(sessionParameterNames[i], 0f);
                sessionActive[i] = false;
            }
        }

        nextSessionToActivate = 0;
        Debug.Log("모든 세션 비활성화");
    }

    /// <summary>
    /// 콤보 브레이크 시 호출
    /// </summary>
    public void ResetSessionsOnComboBreak()
    {
        DeactivateAllSessions();
        SetBPM(availableBpms[0]); // 120 BPM으로 리셋
    }

    /// <summary>
    /// 현재 비트 진행도 반환
    /// </summary>
    public float GetCurrentBeatProgress()
    {
        if (CurrentMusicInstance.isValid())
        {
            int timelinePosition = 0;
            CurrentMusicInstance.getTimelinePosition(out timelinePosition);

            float positionInSeconds = timelinePosition / 1000.0f;
            float beatsPerSecond = CurrentBpm / 60.0f;
            float currentBeatPosition = positionInSeconds * beatsPerSecond;

            float closestBeat = Mathf.Round(currentBeatPosition);
            float beatDistance = Mathf.Abs(currentBeatPosition - closestBeat);

            return beatDistance;
        }

        return 0.5f;
    }

    private void OnDestroy()
    {
        // 현재 인스턴스 정리
        if (CurrentMusicInstance.isValid())
        {
            CurrentMusicInstance.setUserData(IntPtr.Zero);
            CurrentMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            CurrentMusicInstance.release();
        }

        // 다음 인스턴스 정리 (전환 중이었다면)
        if (nextMusicInstance.isValid())
        {
            nextMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            nextMusicInstance.release();
        }

        if (timelineHandle.IsAllocated)
        {
            timelineHandle.Free();
        }
    }

    // 디버그용 GUI
    private void OnGUI()
    {
        GUILayout.Box($"Current Beat = {timelineInfo?.currentBeat}, BPM = {currentBpm}");

        if (isTransitioning)
        {
            GUILayout.Box("🔄 BPM 전환 중...");
        }

        string activeSessions = "Active Sessions: ";
        for (int i = 0; i < sessionActive.Length; i++)
        {
            if (sessionActive[i])
            {
                string name = i < sessionNames.Length ? sessionNames[i] : $"Session {i + 1}";
                activeSessions += name + ", ";
            }
        }
        GUILayout.Box(activeSessions);
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);

        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("timeline callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentBeat = parameter.beat;
                    }
                    break;

                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.lastMarker = parameter.name;
                    }
                    break;
            }
        }

        return FMOD.RESULT.OK;
    }
}
