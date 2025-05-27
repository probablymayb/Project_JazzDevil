using UnityEngine;
public class NoteJudge : MonoBehaviour
{
    [SerializeField] private NoteSpawner noteSpawner;
    [SerializeField] private JudgeNoteTextUI JudgeNoteTextUI;

    [Header("Judgement Settings")]
    [SerializeField] private float perfectWindow = 0.05f;
    [SerializeField] private float goodWindow = 0.1f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckNoteHit();
        }
    }

    private void CheckNoteHit()
    {
        Debug.Log("CheckNoteHit 호출됨");
        Note closestNote = noteSpawner.GetClosestNote();
        if (closestNote == null)
        {
            Debug.Log("null");
            return;
        }

        float currentScale = closestNote.transform.localScale.x;
        float timeOffset = 1f;//Mathf.Abs(RhythmManager.Instance.SongPosition - closestNote.hitTime);

        // 크기 기반 판정
        if (currentScale > 0.1f && currentScale < 0.3f)
        {
            Debug.Log("Perfect!");
            ProcessHit(closestNote, "Perfect");
        }
        else if (currentScale > 0.05f && currentScale < 0.5f)
        {
            Debug.Log("Good!");
            ProcessHit(closestNote, "Good");
        }
        else
        {
            Debug.Log("Miss!");
        }
    }

    private void ProcessHit(Note note, string judgement)
    {
        //note.Hit();
        noteSpawner.RemoveNote(note);
        Destroy(note.gameObject);

        //note 판정 text
        if (JudgeNoteTextUI != null)
        {
            JudgeNoteTextUI.ShowJudge(judgement);
        }

        // 추가적인 판정 처리 (점수, 콤보 등)
    }
}