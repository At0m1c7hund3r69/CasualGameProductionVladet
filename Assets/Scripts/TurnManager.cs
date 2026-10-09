using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public enum TurnState
{
    FirstBlood,
    PlayerTurn,
    EnemyTurn,
    GameOver,
    LevelComplete
}


public class TurnManager : MonoBehaviour
{
    public static TurnManager instance {  get; private set; }

    [Header("Turn")]
    public TurnState currentState = TurnState.FirstBlood;

    [Header("Board Entities")]
    public List<EnemyPawn> activeEnemies = new List<EnemyPawn>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterEnemy(EnemyPawn enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    public void UnRegisterEnemy(EnemyPawn enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    public void EndPlayerTurn()
    {
        if (currentState != TurnState.PlayerTurn) return;

        CheckWinLoseState();
        if (currentState == TurnState.LevelComplete || currentState == TurnState.GameOver) return; 

        currentState = TurnState.EnemyTurn;
        StartCoroutine(EnemyPhaseRoutine());
    }

    public void CheckWinLoseState()
    {
        activeEnemies.RemoveAll(item => item == null);

        int possessedCount = 0;
        foreach (EnemyPawn pawn in new System.Collections.Generic.List<EnemyPawn>(activeEnemies))
        { 
            if (pawn == null) continue;
            if (pawn.isPossessed) possessedCount++;
        }

        if (possessedCount == 0)
        {
            currentState = TurnState.GameOver;
            SceneManager.LoadScene("LoseScreen");
        }
        else if (possessedCount == activeEnemies.Count && activeEnemies.Count > 0)
        {
            currentState = TurnState.LevelComplete;
            SceneManager.LoadScene("WinScreen");
        }

    }

    private IEnumerator EnemyPhaseRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        foreach (EnemyPawn pawn in new System.Collections.Generic.List<EnemyPawn>(activeEnemies))
        {
            if (pawn == null) continue;

            PawnSightline sightline = pawn.GetComponent<PawnSightline>();
            if (sightline != null)
            {
                sightline.CheckLineOfSight();
            }

            if (currentState != TurnState.GameOver)
            {
                EnemyPatrol patrol = pawn.GetComponent<EnemyPatrol>();
                if (patrol != null)
                {
                    patrol.TakePatrolStep();
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

        if (currentState != TurnState.GameOver)
        {
            currentState = TurnState.PlayerTurn;
            Debug.Log("Player Turn");
        }
    }

    public void TriggerGameOver()
    {
        currentState = TurnState.GameOver;
        Debug.Log("GG");
    }
}
