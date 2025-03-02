using UnityEngine;
using System.Collections.Generic;

//test
public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance { get; private set; }

    [Header("Music Settings")]
    [SerializeField] private AudioSource[] musicSources; // ���� ����� �ҽ� �迭
    [SerializeField] private float bpm = 120f;

    private double dspSongTime;
    private float secPerBeat;
    private float songPosition;
    private int activeTrackCount = 1; // ���� Ȱ��ȭ�� Ʈ�� ��

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

        // �ʱ⿡ ��� Ʈ�� ���Ұ�
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

        // Q Ű�� ������ ���� Ʈ�� Ȱ��ȭ
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddNextTrack();
        }
    }

    private void StartSong()
    {
        dspSongTime = AudioSettings.dspTime;

        // ��� Ʈ���� ���ÿ� ��� ���� (���Ұ� ���·�)
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

    // ��� Ʈ�� �ʱ�ȭ (�ʿ��� ���)
    public void ResetTracks()
    {
        activeTrackCount = 1;
        for (int i = 1; i < musicSources.Length; i++)
        {
            musicSources[i].mute = true;
        }
    }
}