using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private float noteDuration = 2f;  // 노트가 생성되고 판정점까지 걸리는 시간
    [SerializeField] private RectTransform spawnPoint; // Canvas의 중앙 지점

    private float lastSpawnTime = 0f;
    private List<Note> activeNotes = new List<Note>();


    private float nextSpawnTime;
    private RhythmManager rhythmManager;

    private void Start()
    {
        rhythmManager = RhythmManager.Instance;
    }

    private void Update()
    {
        // BPM에 맞춰 노트 생성
        if (rhythmManager.SongPosition >= nextSpawnTime)
        {
            SpawnNote();
            nextSpawnTime += rhythmManager.SecPerBeat;
        }
    }

    private void SpawnNote()
    {
        // Canvas의 중앙에 노트 생성
        GameObject note = Instantiate(notePrefab, spawnPoint);
        note.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
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