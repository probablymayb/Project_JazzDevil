using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private float noteDuration = 2f;  // 노트가 생성되고 판정점까지 걸리는 시간
    [SerializeField] private RectTransform spawnPoint; // Canvas의 중앙 지점

    [SerializeField] private int spawnInterval = 1;
    [SerializeField] private int nextSpawn = 0;

    private float lastSpawnTime = 0f;
    private List<Note> activeNotes = new List<Note>();


    private float nextSpawnTime;
    private RhythmManager rhythmManager;


    //for beat tracking
    [SerializeField] private bool waitforString = false;
    [SerializeField] private string stringToWaitFor = "MusicBody";

    private void Awake()
    {
        nextSpawn = spawnInterval;
        rhythmManager = RhythmManager.Instance;

        //RhythmManager Subscribe.
        RhythmManager.markerUpdated += WaitForMarker;
        RhythmManager.beatUpdated += SpawnNote;
    }

    private void OnDestroy()
    {

        //RhythmManager unSubscribe.
        RhythmManager.markerUpdated -= WaitForMarker;
        RhythmManager.beatUpdated -= SpawnNote;
    }

    private void Start()
    {
    }

    private void Update()
    {
        // BPM에 맞춰 노트 생성
        //if (rhythmManager.SongPosition >= nextSpawnTime)
        //{
        //    SpawnNote();
        //    nextSpawnTime += rhythmManager.SecPerBeat;
        //}
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
                // Canvas의 중앙에 노트 생성
                GameObject note = Instantiate(notePrefab, spawnPoint);
                note.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                nextSpawn = spawnInterval - 1;
            }
        }
      
    }

    private void WaitForMarker()
    {
        if (RhythmManager.Instance.timelineInfo.lastMarker == stringToWaitFor)
        {
            waitforString = false;
        }
    }

    //private void UpdateNotes(float currentTime)
    //{
    //    foreach (Note note in activeNotes.ToList())
    //    {
    //        if (note == null) continue;

    //        // 노트 크기 업데이트
    //        note.UpdateScale(currentTime);

    //        // 미스 체크 (너무 작아졌을 때)
    //        if (note.transform.localScale.x < 0.05f && !note.isHit)
    //        {
    //            Debug.Log("Miss!");
    //            activeNotes.Remove(note);
    //            Destroy(note.gameObject);
    //        }
    //    }
    //}

    private void MoveNotes()
    {
        foreach (Note note in activeNotes.ToList())
        {
            if (note == null) continue;
           // note.transform.position += Vector3.down * scrollSpeed * Time.deltaTime;
        }
    }

    public Note GetClosestNote()
    {
        return activeNotes.Count > 0 ? activeNotes[0] : null;
    }

    public void RemoveNote(Note note)
    {
        activeNotes.Remove(note);
    }
}