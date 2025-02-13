using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private float noteDuration = 2f;  // ��Ʈ�� �����ǰ� ���������� �ɸ��� �ð�
    [SerializeField] private RectTransform spawnPoint; // Canvas�� �߾� ����

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
        // BPM�� ���� ��Ʈ ����
        if (rhythmManager.SongPosition >= nextSpawnTime)
        {
            SpawnNote();
            nextSpawnTime += rhythmManager.SecPerBeat;
        }
    }

    private void SpawnNote()
    {
        // Canvas�� �߾ӿ� ��Ʈ ����
        GameObject note = Instantiate(notePrefab, spawnPoint);
        note.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    //private void UpdateNotes(float currentTime)
    //{
    //    foreach (Note note in activeNotes.ToList())
    //    {
    //        if (note == null) continue;

    //        // ��Ʈ ũ�� ������Ʈ
    //        note.UpdateScale(currentTime);

    //        // �̽� üũ (�ʹ� �۾����� ��)
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