using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [Header("Interaction")]
    public UnityEvent onInteract;

    [Header("Audio and Special Effect")]
    public AudioClip unlockSfx;
    public GameObject unlockVfx;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    public void TriggerInteraction()
    {
        if (unlockVfx  != null)
        {
            GameObject vfx = Instantiate(unlockVfx, transform.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }
        if (unlockSfx != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(unlockSfx, Camera.main.transform.position, sfxVolume);
        }

        onInteract.Invoke();
    }
}
