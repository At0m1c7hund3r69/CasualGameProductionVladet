using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    //Progress - Devolopment
    
    public GameObject pauseMenu;
    public MonoBehaviour cameraLookScript;
    public GameObject firstSelectedButton;

    public bool isPaused;

    private EventSystem eventSystem;

    void Start()
    {
        pauseMenu.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        // Initialize the EventSystem
        eventSystem = EventSystem.current;

        if (eventSystem == null)
        {
            Debug.LogError("EventSystem not found in the scene.");
        }
    }

    // Called by the mobile UI Pause button
    public void PauseGame()
    {
        if (isPaused) return;

        pauseMenu.SetActive(true);
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

    // Called by the Resume button
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        isPaused = false;

        if (cameraLookScript != null)
        {
            cameraLookScript.enabled = true;
        }

        // Clear any selected UI object
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(null);
        }
    }
}