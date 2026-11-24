using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using Unity.VisualScripting;

public class RivalsModeManager : MonoBehaviour
{
    public static RivalsModeManager instance;

    public enum RivalsPlayer { None, Player1, Player2  }
    public enum RivalsPhase { FirstBloodP1, FirstBloodP2, NormalTurn }

    [Header("Current Rivals State")]
    public RivalsPlayer activePlayer = RivalsPlayer.Player1;
    public RivalsPhase currentPhase = RivalsPhase.FirstBloodP1;
    public bool isGameOver = false;

    [Header("Tracked Pawns")]
    public List<EnemyPawn> allPawns = new List<EnemyPawn>();

    [Header("Event States")]
    public UnityEvent<RivalsPlayer> onTurnChanged;
    public UnityEvent<RivalsPlayer> onPlayerWon;

    private bool p1HasMadeFirstPossession = false;
    private bool p2HasMadeFirstPossession = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (allPawns.Count == 0)
        {
            allPawns.AddRange(FindObjectsByType<EnemyPawn>());
        }

        activePlayer = RivalsPlayer.Player1;
        currentPhase = RivalsPhase.FirstBloodP1;
        onTurnChanged?.Invoke(activePlayer);
    }

    public void RegisterActionCompleted(bool possessedThisTurn)
    {
        if (isGameOver) return;

        EvaluateWinLoss();
        if (isGameOver) return;

        if (currentPhase == RivalsPhase.FirstBloodP1)
        {
            if (possessedThisTurn)
            {
                p1HasMadeFirstPossession = true;
            }

            if (p1HasMadeFirstPossession && possessedThisTurn)
            {
                activePlayer = RivalsPlayer.Player2;
                currentPhase = RivalsPhase.FirstBloodP2;
                onTurnChanged?.Invoke(activePlayer);
            }
            return;
        }

        if (currentPhase == RivalsPhase.FirstBloodP2)
        {
            if (possessedThisTurn)
            {
                p2HasMadeFirstPossession = true;
            }

            if (p2HasMadeFirstPossession && !possessedThisTurn)
            {
                currentPhase = RivalsPhase.NormalTurn;
                SwitchToOtherPlayer();
            }
            return;
        }

        if (!possessedThisTurn)
        {
            SwitchToOtherPlayer();
        }
    }

    private void SwitchToOtherPlayer()
    {
        activePlayer = (activePlayer == RivalsPlayer.Player1)
            ? RivalsPlayer.Player2
            : RivalsPlayer.Player1;

        onTurnChanged?.Invoke(activePlayer);
    }

    public void EvaluateWinLoss()
    {
        if (isGameOver) return;

        int p1Count = 0;
        int p2Count = 0;

        foreach (EnemyPawn pawn in allPawns)
        {
            if (pawn == null || !pawn.gameObject.activeInHierarchy) continue;

            if (pawn.owner == RivalsPlayer.Player1) p1Count++;
            else if (pawn.owner == RivalsPlayer.Player2) p2Count++;
        }

        if (p1Count == 0 && p2Count > 0)
        {
            TriggerPlayerWin(RivalsPlayer.Player2);
        }
        else if (p2Count == 0 && p1Count > 0)
        {
            TriggerPlayerWin(RivalsPlayer.Player1);
        }
    }

    public void TriggerPlayerWin(RivalsPlayer winner)
    {
        if (!isGameOver) return;
        isGameOver = true;

        if (TurnManager.instance != null)
        {
            TurnManager.instance.currentState = TurnState.LevelComplete;
        }

        onPlayerWon?.Invoke(winner);
    }

    public void Forfeit(RivalsPlayer forfeitingPlayer)
    {
        RivalsPlayer winner = (forfeitingPlayer == RivalsPlayer.Player1)
            ? RivalsPlayer.Player2
            : RivalsPlayer.Player1;

        TriggerPlayerWin(winner);
    }
}
