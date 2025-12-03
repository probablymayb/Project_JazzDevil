using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private float noteDuration = 2f;
    [SerializeField] private RectTransform spawnPoint;

    [SerializeField] private int spawnInterval = 1;
    [SerializeField] private int nextSpawn = 0;

    private float lastSpawnTime = 0f;
    private List<Note> activeNotes = new List<Note>();

    private float nextSpawnTime;
    private RhythmManager rhythmManager;

    [SerializeField] private bool waitforString = false;
    [SerializeField] private string stringToWaitFor = "MusicBody";

    private void Awake()
    {
        nextSpawn = spawnInterval;
        rhythmManager = RhythmManager.Instance;

        RhythmManager.markerUpdated += WaitForMarker;
        RhythmManager.beatUpdated += SpawnNote;
        RhythmManager.beatUpdated += SpawnMonster;
    }

    private void Start()
    {
        PoolManager.Instance.CreatePool(notePrefab, 10);
    }

    private void OnDestroy()
    {
        RhythmManager.markerUpdated -= WaitForMarker;
        RhythmManager.beatUpdated -= SpawnNote;
        RhythmManager.beatUpdated -= SpawnMonster;
    }

    private void Update()
    {
        // 비활성화된 노트 리스트에서 제거
        CleanupInactiveNotes();
    }

    private void SpawnNote()
    {
        if (!waitforString)
        {
            if (nextSpawn > 0)
            {
                nextSpawn--;
            }
            else
            {
                GameObject note = PoolManager.Instance.Get(notePrefab);

                GameObject group = GameObject.Find("Note Group");
                note.transform.parent = group.transform;

                note.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                Note noteComp = note.transform.GetChild(0).GetComponent<Note>();
                if (noteComp != null)
                {
                    noteComp.poolPrefabRef = notePrefab;

                    // activeNotes에 추가
                    activeNotes.Add(noteComp);
                }

                nextSpawn = spawnInterval - 1;
            }
        }
    }

    private void SpawnMonster()
    {
        // 기존 코드 유지
    }

    private void WaitForMarker()
    {
        if (RhythmManager.Instance.timelineInfo.lastMarker == stringToWaitFor)
        {
            waitforString = false;
        }
    }

    /// <summary>
    /// 비활성화된 노트 정리
    /// </summary>
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

    /// <summary>
    /// 가장 오래된(먼저 생성된) 노트 반환
    /// </summary>
    public Note GetClosestNote()
    {
        CleanupInactiveNotes();
        return activeNotes.Count > 0 ? activeNotes[0] : null;
    }

    /// <summary>
    /// 노트 리스트에서 제거
    /// </summary>
    public void RemoveNote(Note note)
    {
        if (note != null)
        {
            activeNotes.Remove(note);
        }
    }

    public void StopSpawningNotes()
    {
        RhythmManager.beatUpdated -= SpawnNote;
    }
}
