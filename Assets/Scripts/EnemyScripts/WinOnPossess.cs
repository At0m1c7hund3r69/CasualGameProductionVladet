using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinOnPossess : MonoBehaviour
{
    public float winDelay = 1.5f;
    private EnemyPawn pawn;
    private bool winTriggered = false;

    void Start()
    {
        pawn = GetComponent<EnemyPawn>();    
    }

    void Update()
    {
        if (pawn != null && pawn.isPossessed && !winTriggered)
        {
            winTriggered = true;
            StartCoroutine(WinSequence());
        }
    }

    private IEnumerator WinSequence()
    {
        yield return new WaitForSeconds(winDelay);

        if (TurnManager.instance != null)
        {
            TurnManager.instance.currentState = TurnState.LevelComplete;
            SceneManager.LoadScene("WinScreen");
        }
    }
}
