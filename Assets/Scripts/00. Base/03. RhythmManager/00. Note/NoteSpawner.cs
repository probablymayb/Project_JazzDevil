using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;

    [Header("노트 타이밍")]
    [SerializeField] private int beatsAhead = 2;
    [SerializeField] private int beatsPerNote = 1;

    [Header("대기 설정")]
    [SerializeField] private bool waitForMarker = false;
    [SerializeField] private string markerToWaitFor = "MusicBody";

    [Header("BPM 전환 설정")]
    [SerializeField] private float bpmChangePause = 1f;  // BPM 변경 시 멈춤 시간

    private List<Note> activeNotes = new List<Note>();
    private int beatCounter = 0;
    private bool isPaused = false;  // 노트 생성 일시정지

    private void Awake()
    {
        RhythmManager.beatUpdated += OnBeat;
        RhythmManager.markerUpdated += OnMarkerUpdated;
    }

    private void Start()
    {
        PoolManager.Instance.CreatePool(notePrefab, 10);

        // BPM 변경 이벤트 구독
        if (RhythmManager.Instance != null)
        {
            RhythmManager.Instance.OnBPMChanged += OnBPMChanged;
        }
    }

    private void OnDestroy()
    {
        RhythmManager.beatUpdated -= OnBeat;
        RhythmManager.markerUpdated -= OnMarkerUpdated;

        if (RhythmManager.Instance != null)
        {
            RhythmManager.Instance.OnBPMChanged -= OnBPMChanged;
        }
    }

    private void Update()
    {
        CleanupInactiveNotes();
    }

    private void OnBeat()
    {
        if (GameManager.Instance.CurrentGameState != EGameState.Playing) return;
        if (waitForMarker) return;
        if (isPaused) return;  // 일시정지 중이면 생성 안 함

        beatCounter++;

        if (beatCounter % beatsPerNote != 0) return;

        SpawnNote();
    }

    private void OnBPMChanged(float oldBpm, float newBpm)
    {
        Debug.Log($"[NoteSpawner] BPM 변경 감지: {oldBpm} → {newBpm}, {bpmChangePause}초 대기");
        StartCoroutine(PauseForBPMChange());
    }

    private IEnumerator PauseForBPMChange()
    {
        isPaused = true;
        yield return new WaitForSeconds(bpmChangePause);
        isPaused = false;
        beatCounter = 0;  // 카운터 리셋
        Debug.Log("[NoteSpawner] 노트 생성 재개");
    }

    private void SpawnNote()
    {
        float currentBpm = RhythmManager.Instance.CurrentBpm;
        float secPerBeat = 60f / currentBpm;
        float approachTime = secPerBeat * beatsAhead;

        GameObject note = PoolManager.Instance.Get(notePrefab);

        GameObject group = GameObject.Find("Note Group");
        if (group != null)
            note.transform.SetParent(group.transform);

        note.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        Note noteComp = note.transform.GetChild(0).GetComponent<Note>();
        if (noteComp != null)
        {
            noteComp.poolPrefabRef = notePrefab;
            noteComp.Initialize(approachTime);
            activeNotes.Add(noteComp);
        }
    }

    private void OnMarkerUpdated()
    {
        if (RhythmManager.Instance.timelineInfo.lastMarker == markerToWaitFor)
        {
            waitForMarker = false;
            beatCounter = 0;
        }
    }

    private void CleanupInactiveNotes()
    {
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            if (activeNotes[i] == null || !activeNotes[i].gameObject.activeInHierarchy)
            {
                activeNotes.RemoveAt(i);
            }
        }
    }

    public Note GetClosestNote()
    {
        CleanupInactiveNotes();
        return activeNotes.Count > 0 ? activeNotes[0] : null;
    }

    public void RemoveNote(Note note)
    {
        if (note != null)
            activeNotes.Remove(note);
    }

    public void StopSpawningNotes()
    {
        RhythmManager.beatUpdated -= OnBeat;
    }
}
