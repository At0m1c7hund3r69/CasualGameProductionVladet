using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class InteractableObject : MonoBehaviour
{
    [Header("Interaction")]
    public UnityEvent onInteract;

    [Header("Audio and Special Effect")]
    public AudioClip unlockSfx;
    public GameObject unlockVfx;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    public TurnState currentState;

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

        StartCoroutine(ExecuteAfterDelay(2.0f));
    }

    IEnumerator ExecuteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (TurnManager.instance != null)
        {
            TurnManager.instance.currentState = TurnState.LevelComplete;
            SceneManager.LoadScene("WinScreen");
        }
    }
}
