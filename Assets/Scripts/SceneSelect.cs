using UnityEngine;

public class SceneSelect : MonoBehaviour
{
    //Progress - Development
    public void SampleScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene"); //Changes scene to Sample Scene
    }

    public void LevelOne()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelOne"); //Changes scene to Level One
    }

    public void WinScreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("WinScreen"); //Changes scene to the Win Screen
    }

    public void LoseScreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoseScreen"); //Changes scene to the Lose Screen
    }

    public void Endgame()
    {
        Application.Quit(); //used to quit game with a button press
    }
}
