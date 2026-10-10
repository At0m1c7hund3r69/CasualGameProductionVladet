using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    //Progress - Development
    public GameObject pauseMenu;
    public GameObject gameplayUI;

    public MonoBehaviour cameraLookScript;
    public GameObject firstSelectedButton;

    public bool isPaused;

    private EventSystem eventSystem;

    void Start() //Sets the Pause Panel to inactive on start
    {
        pauseMenu.SetActive(false);

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(true);
        }

        Time.timeScale = 1f;
        isPaused = false;

        eventSystem = EventSystem.current;

        if (eventSystem == null)
        {
            Debug.LogError("EventSystem not found in the scene.");
        }
    }

    public void PauseGame() //Sets game time to zero, close gameplay UI, opens up pause panel
    {
        if (isPaused) return;

        pauseMenu.SetActive(true);

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(false);
        }

        Time.timeScale = 0f;
        isPaused = true;

        if (cameraLookScript != null)
        {
            cameraLookScript.enabled = false;
        }

        if (eventSystem != null && firstSelectedButton != null)
        {
            eventSystem.SetSelectedGameObject(firstSelectedButton);
        }
    }

    public void ResumeGame() //Sets gametime back to normal, closes pause panel, reopens gameplayUI
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(true);
        }

        isPaused = false;

        if (cameraLookScript != null)
        {
            cameraLookScript.enabled = true;
        }

        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(null);
        }
    }
}
