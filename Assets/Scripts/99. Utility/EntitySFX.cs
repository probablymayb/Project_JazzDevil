using UnityEngine;
using FMODUnity;

public class EntitySFX : MonoBehaviour
{
    [Header("FMOD Events")]
    [SerializeField]
    private EventReference footstepSound;

    private void PlayFootstepSound()
    {
        if (footstepSound.IsNull)
        {
            Debug.LogWarning("footstep 사운드 이벤트를 찾을 수 없음.");
        }
        else
        {
            AudioManager.Instance.PlayOneShot(footstepSound, transform.position);
        }
    }
}