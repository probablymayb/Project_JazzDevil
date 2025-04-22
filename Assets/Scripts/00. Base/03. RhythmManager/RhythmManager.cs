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
    //박자 타이밍 계산
    //비트 이벤트
    //BPM 관리
    //리듬 판정

    [Header("FMOD 설정")]
    [SerializeField] private EventReference musicEventReference;
    [SerializeField] private string bpmParameterName = "BPM"; // FMOD에서 BPM 파라미터명

    [Header("리듬 설정")]
    [SerializeField] private float defaultBpm = 120f;
    [SerializeField] private int beatsPerBar = 4; // 마디당 박자 수

    // FMOD 이벤트 인스턴스
    private EventInstance musicEventInstance;
    private bool isPlaying = false;

    // 리듬 관련 변수
    private float currentBpm;
    private float secPerBeat;
    private float songPosition;
    private float nextBeatTime;
    private int currentBeat = 0;
    private int currentBar = 0;
    private double dspStartTime;

    // 이벤트 시스템
    public event Action<int> OnBeat; // 각 비트마다 발생하는 이벤트
    public event Action<int> OnBar; // 각 마디의 첫 비트에서 발생하는 이벤트
    public event Action<float> OnBeatProgress; // 현재 비트 진행도 (0~1)



    //FMOD Timeline Tracker
    [SerializeField] private EventReference music;

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

    //Beat Event
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
    public bool IsPlaying => isPlaying;

    protected override void Awake()
    {
        base.Awake();

        // 기본값 초기화
        currentBpm = defaultBpm;
        secPerBeat = 60f / currentBpm;

        if (!music.IsNull)
        {
            musicInstance = RuntimeManager.CreateInstance(music);
            musicInstance.start();
        }
        else
        {
            Debug.LogWarning("음악 이벤트 레퍼런스가 설정되지 않았습니다.");
        }
    }

    private void Start()
    {
        if (!music.IsNull)
        {
            timelineInfo = new TimelineInfo();
            beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
            //Ignore Garbage Collection !!!
            timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
            musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
            musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
        }
        //InitializeMusic();
    }

    private void Update()
    {
        if (lastMarkerString != timelineInfo.lastMarker)
        {
            lastMarkerString = timelineInfo.lastMarker;

            if (markerUpdated != null)
            {
                markerUpdated();
            }
        }

        if (lastBeat != timelineInfo.currentBeat)
        {
            lastBeat = timelineInfo.currentBeat;

            if (beatUpdated != null)
            {
                beatUpdated();
            }
        }
    }

    /// <summary>
    /// FMOD 음악 이벤트 초기화
    /// </summary>
    private void InitializeMusic()
    {
        if (musicEventReference.IsNull)
        {
            Debug.LogWarning("음악 이벤트 레퍼런스가 설정되지 않았습니다.");
            return;
        }

        // 기존 음악 인스턴스 정리
        if (musicEventInstance.isValid())
        {
            musicEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicEventInstance.release();
        }

        // 새 음악 인스턴스 생성
        musicEventInstance = RuntimeManager.CreateInstance(musicEventReference);

        try
        {
            // 현재 BPM 값 가져오기
            float bpmValue;
            RESULT result = musicEventInstance.getParameterByName(bpmParameterName, out bpmValue);

            if (result == RESULT.OK)
            {
                currentBpm = bpmValue;
                Debug.Log($"FMOD에서 BPM 값을 가져왔습니다: {currentBpm}");
            }
            else
            {
                currentBpm = defaultBpm;
                Debug.Log($"BPM 파라미터를 찾을 수 없어 기본값을 사용합니다: {currentBpm}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"BPM 값을 가져오는 중 오류 발생: {e.Message}");
            currentBpm = defaultBpm;
        }

        // BPM 기반 타이밍 계산
        UpdateBPMSettings();
    }

    /// <summary>
    /// BPM이 변경되었을 때 관련 설정 업데이트
    /// </summary>
    private void UpdateBPMSettings()
    {
        secPerBeat = 60f / currentBpm;
        Debug.Log($"비트당 시간 업데이트: {secPerBeat} 초 (BPM: {currentBpm})");
    }

    /// <summary>
    /// 음악 재생 시작
    /// </summary>
    public void StartMusic()
    {
        if (!musicEventInstance.isValid())
        {
            InitializeMusic();
        }

        dspStartTime = AudioSettings.dspTime;
        musicEventInstance.start();
        isPlaying = true;

        // 첫 비트 시간 설정
        nextBeatTime = 0;
        currentBeat = -1; // 첫 업데이트에서 0으로 증가
        currentBar = 0;

        Debug.Log("음악 재생 시작");
    }

    /// <summary>
    /// 음악 재생 중단
    /// </summary>
    public void StopMusic()
    {
        if (musicEventInstance.isValid())
        {
            musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isPlaying = false;
            Debug.Log("음악 재생 중단");
        }
    }

    /// <summary>
    /// 음악 BPM 변경
    /// </summary>
    public void SetBPM(float newBpm)
    {
        if (newBpm <= 0)
        {
            Debug.LogWarning($"유효하지 않은 BPM 값: {newBpm}");
            return;
        }

        currentBpm = newBpm;
        UpdateBPMSettings();

        // FMOD 이벤트에 BPM 파라미터 설정
        if (musicEventInstance.isValid())
        {
            musicEventInstance.setParameterByName(bpmParameterName, newBpm);
            Debug.Log($"BPM 변경: {newBpm}");
        }
    }

    /// <summary>
    /// 트랙 활성화 (FMOD 파라미터 설정)
    /// </summary>
    public void SetTrackParameter(string paramName, float value)
    {
        if (musicEventInstance.isValid())
        {
            musicEventInstance.setParameterByName(paramName, value);
            Debug.Log($"트랙 파라미터 설정: {paramName} = {value}");
        }
    }

    /// <summary>
    /// 특정 비트 수만큼 기다리는 코루틴
    /// </summary>
    public IEnumerator WaitForBeats(int beats)
    {
        int startingBeat = currentBeat;
        int targetBeatCount = 0;

        while (targetBeatCount < beats)
        {
            yield return null;

            // 비트가 변경되었는지 확인
            if (currentBeat != startingBeat)
            {
                targetBeatCount++;
                startingBeat = currentBeat;
            }
        }
    }

    /// <summary>
    /// 현재 비트 진행도 반환 (0 = 정확히 비트, 0.5 = 비트 사이 중간)
    /// </summary>
    public float GetCurrentBeatProgress()
    {
        // FMOD를 사용하여 현재 음악의 재생 위치 가져오기
        if (musicInstance.isValid())
        {
            FMOD.Studio.PLAYBACK_STATE state;
            musicInstance.getPlaybackState(out state);

            if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
                return 0.5f; // 재생 중이 아닌 경우 기본값 반환

            int timelinePosition = 0;
            musicInstance.getTimelinePosition(out timelinePosition);

            //밀리초를 초로 변환
            float positionInSeconds = timelinePosition / 1000.0f;

            //현재 비트 위치 계산 (BPM 기반)
            float beatsPerSecond = CurrentBpm / 60.0f;
            float currentBeatPosition = positionInSeconds * beatsPerSecond;

            //가장 가까운 비트와의 거리 계산
            float closestBeat = Mathf.Round(currentBeatPosition);
            float beatDistance = Mathf.Abs(currentBeatPosition - closestBeat);

            //정규화된 값으로 반환 (0 = 정확히 비트, 0.5 = 비트 사이 중간)
            return beatDistance;
        }

        Debug.LogWarning("FMOD 인스턴스 없음");

        //FMOD 인스턴스 유효하지 않은 경우
        if (timelineInfo != null)
        {
            float beatDuration = SecPerBeat;
            float timeSinceStart = Time.time; // 또는 더 정확한 시간 측정 사용

            float beatsElapsed = timeSinceStart / beatDuration;
            float currentBeatPartial = beatsElapsed - Mathf.Floor(beatsElapsed);

            // 0.5 기준으로 비트 거리 계산
            float normalizedDistance = Mathf.Abs(currentBeatPartial - 0.5f);
            return normalizedDistance;
        }

        return 0.5f; // 기본값 (가장 부정확한 값)
    }

    /// <summary>
    /// 가장 최근 비트 시간 반환
    /// </summary>
    public float GetLastBeatTime()
    {
        //구현이 필요함 - FMOD에서 마지막 비트 정보를 가져와야 함
        return Time.time - (Time.time % SecPerBeat);
    }

    private void OnDestroy()
    {
        musicInstance.setUserData(IntPtr.Zero);
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
        timelineHandle.Free();

        // FMOD 인스턴스 정리
        if (musicEventInstance.isValid())
        {
            musicEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicEventInstance.release();
        }

    }

    //FOR DEBUG
    private void OnGUI()
    {
        GUILayout.Box($"Current Beat =  {timelineInfo.currentBeat}, Last Marker = {(string)timelineInfo.lastMarker}");
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
