using UnityEditor;
using UnityEngine;

public class ResetPlayerPrefs : MonoBehaviour
{
    [MenuItem("File/PlayerPrefs 초기화")]
    private static void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs 초기화됨.");
    }
}