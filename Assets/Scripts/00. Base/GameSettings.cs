using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
[System.Serializable]
public class GameSettings : ScriptableObject
{
    public float masterVolume = 1f;
    public float sfxVolume = 1f;
}
