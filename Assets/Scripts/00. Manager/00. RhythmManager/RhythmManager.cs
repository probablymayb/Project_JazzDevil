using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;

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
    }

    private void Start()
    {
        InitializeMusic();
    }

    private void Update()
    {
        if (!isPlaying) return;

        // 노래 위치 업데이트
        songPosition = (float)(AudioSettings.dspTime - dspStartTime);

        // 현재 비트 진행도 계산 (0~1)
        float beatProgress = (songPosition % secPerBeat) / secPerBeat;
        OnBeatProgress?.Invoke(beatProgress);

        // 다음 비트 감지
        if (songPosition >= nextBeatTime)
        {
            // 비트 카운터 업데이트
            currentBeat = (currentBeat + 1) % beatsPerBar;

            // 마디 첫 비트인 경우
            if (currentBeat == 0)
            {
                currentBar++;
                OnBar?.Invoke(currentBar);
            }

            // 비트 이벤트 발생
            OnBeat?.Invoke(currentBeat);

            // 다음 비트 시간 계산
            nextBeatTime += secPerBeat;
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
    /// 비트 판정 (Perfect, Good, Miss)
    /// </summary>
    public BeatAccuracy GetBeatAccuracy(float tolerancePerfect = 0.05f, float toleranceGood = 0.15f)
    {
        // 현재 비트 진행도 계산 (0~1)
        float beatProgress = (songPosition % secPerBeat) / secPerBeat;

        // 0 또는 1에 가까울수록 정확한 타이밍
        float accuracy = Mathf.Min(beatProgress, 1f - beatProgress) * 2;

        if (accuracy <= tolerancePerfect)
            return BeatAccuracy.Perfect;
        else if (accuracy <= toleranceGood)
            return BeatAccuracy.Good;
        else
            return BeatAccuracy.Miss;
    }

    public enum BeatAccuracy
    {
        Miss,
        Good,
        Perfect
    }

    private void OnDestroy()
    {
        // FMOD 인스턴스 정리
        if (musicEventInstance.isValid())
        {
            musicEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicEventInstance.release();
        }
    }
}