using UnityEngine;
using FMODUnity;

public class Ambience : MonoBehaviour
{
    [Header("FMOD Events")]
    [SerializeField]
    private EventReference ambienceBGM;

    private void Awake()
    {
        AudioManager.Instance.PlayLooping(ambienceBGM, "ambience");
    }
}
