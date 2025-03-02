using UnityEngine;
using System.Collections.Generic;

//test
public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance { get; private set; }

    [Header("Music Settings")]
    [SerializeField] private AudioSource[] musicSources; // 여러 오디오 소스 배열
    [SerializeField] private float bpm = 120f;

    private double dspSongTime;
    private float secPerBeat;
    private float songPosition;
    private int activeTrackCount = 1; // 현재 활성화된 트랙 수

    private const int trackCount = 6;

    public float SongPosition => songPosition;
    public float SecPerBeat => secPerBeat;
    public float Bpm => bpm;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        secPerBeat = 60f / bpm;

        // 초기에 모든 트랙 음소거
        for (int i = 1; i < musicSources.Length; i++)
        {
            musicSources[i].mute = true;
        }
    }

    private void Start()
    {
        StartSong();
    }

    private void Update()
    {
        songPosition = (float)(AudioSettings.dspTime - dspSongTime);

        // Q 키를 누르면 다음 트랙 활성화
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddNextTrack();
        }
    }

    private void StartSong()
    {
        dspSongTime = AudioSettings.dspTime;

        // 모든 트랙을 동시에 재생 시작 (음소거 상태로)
        foreach (var source in musicSources)
        {
            source.Play();
        }
    }

    private void AddNextTrack()
    {
        if (activeTrackCount < musicSources.Length)
        {
            musicSources[activeTrackCount].mute = false;
            activeTrackCount++;
        }
    }

    // 모든 트랙 초기화 (필요한 경우)
    public void ResetTracks()
    {
        activeTrackCount = 1;
        for (int i = 1; i < musicSources.Length; i++)
        {
            musicSources[i].mute = true;
        }
    }
}