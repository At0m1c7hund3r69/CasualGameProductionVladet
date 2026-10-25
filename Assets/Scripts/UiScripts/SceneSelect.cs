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

    public void Puzzle_Level1()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Puzzle_LevelOne"); //Changes scene to the lv1
    }

    public void Puzzle_Level2()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Puzzle_LevelTwo"); //Changes scene to the lv2
    }

    public void Puzzle_Level3()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Puzzle_LevelThree"); //Changes scene to the lv3
    }

    public void Puzzle_Level4()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Puzzle_LevelFour"); //Changes scene to the lv4
    }


    public void LevelSelect()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelect"); //Changes scene to the sceneSelect
    }


    public void Endgame()
    {
        Application.Quit(); //used to quit game with a button press
    }
}
