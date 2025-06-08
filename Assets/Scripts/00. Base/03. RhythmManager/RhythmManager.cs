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
    [Header("FMOD 설정")]
    [SerializeField] private string bpmParameterName = "BPM";

    [Header("리듬 설정")]
    [SerializeField] private float defaultBpm = 120f;
    [SerializeField] private int beatsPerBar = 4;

    [Header("세션 파라미터")]
    [SerializeField]
    private string[] sessionParameterNames = {
        "BassVolume",
        "PianoVolume",
        "BrassVolume",
        "MelodyVolume",
        "OtherVolume"
    }; // FMOD Studio에서 생성한 파라미터명과 일치해야 함
    [SerializeField]
    private string[] sessionNames = {
        "Bass",
        "Piano",
        "Brass",
        "Melody",
        "Other"
    }; // 디버그용 이름

    [Header("BPM 설정")]
    [SerializeField] private float[] availableBpms = { 120f, 125f, 130f, 135f, 140f, 145f, 150f, 155f, 160f };
    private int currentBpmIndex = 0;

    // 세션 상태 관리
    private bool[] sessionActive;
    private int nextSessionToActivate = 0;

    // 리듬 관련 변수
    private float currentBpm;
    private float secPerBeat;
    private float songPosition;
    private float nextBeatTime;
    private int currentBeat = 0;
    private int currentBar = 0;
    private double dspStartTime;

    // 키 입력 관련
    private bool keyPressed = false;
    private float keyPressTime = 0f;
    private bool bpmIncreaseRequested = false;

    // 이벤트 시스템
    public event Action<int> OnBeat;
    public event Action<int> OnBar;
    public event Action<float> OnBeatProgress;

    // FMOD Timeline Tracker (단일 인스턴스)
    [SerializeField] private EventReference music; // 120_Drum 이벤트 (모든 트랙 포함)
    private FMOD.Studio.EventInstance musicInstance;
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

    // 접근 프로퍼티
    public float CurrentBpm => currentBpm;
    public float SecPerBeat => secPerBeat;
    public float SongPosition => songPosition;
    public int CurrentBeat => currentBeat;
    public int CurrentBar => currentBar;
    public bool IsPlaying => musicInstance.isValid();

    protected override void Awake()
    {
        base.Awake();

        currentBpm = availableBpms[currentBpmIndex];
        secPerBeat = 60f / currentBpm;

        // 세션 상태 배열 초기화
        sessionActive = new bool[sessionParameterNames.Length];

        // 단일 음악 이벤트 초기화
        if (!music.IsNull)
        {
            musicInstance = RuntimeManager.CreateInstance(music);
            InitializeAllSessionsToZero(); // 모든 세션 볼륨을 0으로 초기화
        }
        else
        {
            Debug.LogWarning("음악 이벤트 레퍼런스가 설정되지 않았습니다.");
        }
    }

    /// <summary>
    /// 모든 세션 파라미터를 0으로 초기화 (비활성화 상태)
    /// </summary>
    private void InitializeAllSessionsToZero()
    {
        if (!musicInstance.isValid()) return;

        for (int i = 0; i < sessionParameterNames.Length; i++)
        {
            if (!string.IsNullOrEmpty(sessionParameterNames[i]))
            {
                musicInstance.setParameterByName(sessionParameterNames[i], 0f);
                sessionActive[i] = false;
                Debug.Log($"{sessionParameterNames[i]} 파라미터를 0으로 초기화");
            }
        }
    }

    private void Start()
    {
        if (!music.IsNull && musicInstance.isValid())
        {
            timelineInfo = new TimelineInfo();
            beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
            timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);

            musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
            musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

            // 음악 시작 (모든 트랙이 포함되어 있지만 볼륨은 0 상태)
            musicInstance.start();
            Debug.Log("통합 음악 이벤트 시작됨 (모든 세션 비활성화 상태)");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !keyPressed)
        {
            keyPressed = true;
            keyPressTime = Time.time;
            Debug.Log("Q 키 입력 감지: 다음 비트에서 세션 활성화 준비");
        }

        if (lastMarkerString != timelineInfo.lastMarker)
        {
            lastMarkerString = timelineInfo.lastMarker;
            markerUpdated?.Invoke();
        }

        if (lastBeat != timelineInfo.currentBeat)
        {
            lastBeat = timelineInfo.currentBeat;
            beatUpdated?.Invoke();

            // 키 입력이 있었고, 다음 비트가 도달했다면 세션 활성화
            if (keyPressed)
            {
                ActivateNextSession();
                keyPressed = false;
            }

            // BPM 증가 요청이 있었다면
            if (bpmIncreaseRequested)
            {
                IncreaseBpm();
                bpmIncreaseRequested = false;
            }
        }

        // E 키: BPM 증가 요청
        if (Input.GetKeyDown(KeyCode.E) && !bpmIncreaseRequested)
        {
            bpmIncreaseRequested = true;
            Debug.Log("E 키 입력 감지: 다음 비트에서 BPM 증가 준비");
        }
    }

    /// <summary>
    /// 다음 세션을 파라미터로 활성화 (볼륨 0 → 1)
    /// </summary>
    public void ActivateNextSession()
    {
        if (nextSessionToActivate >= sessionParameterNames.Length)
        {
            Debug.Log("모든 세션이 이미 활성화되었습니다.");
            return;
        }

        if (!musicInstance.isValid()) return;

        string paramName = sessionParameterNames[nextSessionToActivate];
        if (!string.IsNullOrEmpty(paramName))
        {
            // 파라미터 값을 1로 설정하여 세션 활성화
            musicInstance.setParameterByName(paramName, 1f);
            sessionActive[nextSessionToActivate] = true;

            string sessionName = nextSessionToActivate < sessionNames.Length
                ? sessionNames[nextSessionToActivate]
                : $"Session {nextSessionToActivate + 1}";

            Debug.Log($"{sessionName} 세션이 파라미터 '{paramName}'를 통해 활성화됨");
            nextSessionToActivate++;
        }
    }

    /// <summary>
    /// BPM 증가
    /// </summary>
    private void IncreaseBpm()
    {
        if (availableBpms == null || availableBpms.Length == 0)
            return;

        currentBpmIndex = (currentBpmIndex + 1) % availableBpms.Length;
        float newBpm = availableBpms[currentBpmIndex];

        SetBPM(newBpm);
        Debug.Log($"BPM 변경됨: {newBpm}");
    }

    /// <summary>
    /// BPM 설정
    /// </summary>
    public void SetBPM(float newBpm)
    {
        if (newBpm <= 0)
        {
            Debug.LogWarning($"유효하지 않은 BPM 값: {newBpm}");
            return;
        }

        currentBpm = newBpm;
        secPerBeat = 60f / currentBpm;

        // 음악 인스턴스에 BPM 파라미터 설정
        if (musicInstance.isValid())
        {
            musicInstance.setParameterByName(bpmParameterName, newBpm);
            Debug.Log($"BPM 변경: {newBpm}");
        }
    }

    /// <summary>
    /// 특정 세션 비활성화
    /// </summary>
    public void DeactivateSession(int sessionIndex)
    {
        if (sessionIndex < 0 || sessionIndex >= sessionParameterNames.Length)
            return;

        if (!musicInstance.isValid()) return;

        string paramName = sessionParameterNames[sessionIndex];
        if (!string.IsNullOrEmpty(paramName) && sessionActive[sessionIndex])
        {
            musicInstance.setParameterByName(paramName, 0f);
            sessionActive[sessionIndex] = false;

            // 다음 활성화할 세션 인덱스 업데이트
            if (sessionIndex < nextSessionToActivate)
            {
                nextSessionToActivate = sessionIndex;
            }

            string sessionName = sessionIndex < sessionNames.Length
                ? sessionNames[sessionIndex]
                : $"Session {sessionIndex + 1}";

            Debug.Log($"{sessionName} 세션 비활성화됨");
        }
    }

    /// <summary>
    /// 모든 세션 비활성화
    /// </summary>
    public void DeactivateAllSessions()
    {
        if (!musicInstance.isValid()) return;

        for (int i = 0; i < sessionParameterNames.Length; i++)
        {
            if (!string.IsNullOrEmpty(sessionParameterNames[i]) && sessionActive[i])
            {
                musicInstance.setParameterByName(sessionParameterNames[i], 0f);
                sessionActive[i] = false;
            }
        }

        nextSessionToActivate = 0;
        Debug.Log("모든 세션이 비활성화되었습니다.");
    }

    /// <summary>
    /// 특정 세션의 파라미터 설정
    /// </summary>
    public void SetSessionParameter(int sessionIndex, string paramName, float value)
    {
        if (!musicInstance.isValid()) return;

        musicInstance.setParameterByName(paramName, value);
        Debug.Log($"세션 {sessionIndex} 파라미터 '{paramName}' = {value}");
    }

    /// <summary>
    /// 현재 비트 진행도 반환
    /// </summary>
    public float GetCurrentBeatProgress()
    {
        if (musicInstance.isValid())
        {
            FMOD.Studio.PLAYBACK_STATE state;
            musicInstance.getPlaybackState(out state);

            if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
                return 0.5f;

            int timelinePosition = 0;
            musicInstance.getTimelinePosition(out timelinePosition);

            float positionInSeconds = timelinePosition / 1000.0f;
            float beatsPerSecond = CurrentBpm / 60.0f;
            float currentBeatPosition = positionInSeconds * beatsPerSecond;

            float closestBeat = Mathf.Round(currentBeatPosition);
            float beatDistance = Mathf.Abs(currentBeatPosition - closestBeat);

            return beatDistance;
        }

        return 0.5f;
    }

    /// <summary>
    /// 음악 중지
    /// </summary>
    public void StopMusic()
    {
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Debug.Log("음악 재생 중단");
        }
    }

    private void OnDestroy()
    {
        // 단일 인스턴스 정리
        if (musicInstance.isValid())
        {
            musicInstance.setUserData(IntPtr.Zero);
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicInstance.release();
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
            Debug.LogError("timeline callback error :  " + result);
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
