using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;

public class PriestBehavior : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    private EnemyPawn pawn;

    [Header("Settings")]
    public int radius = 2;
    public int turnsToActivate = 3;

    private int currentTurnCount = 0;

    private void Awake()
    {
        pawn = GetComponent<EnemyPawn>();
    }

    public void ProcessPriestTurn()
    {
        currentTurnCount++;

        if (currentTurnCount >= turnsToActivate)
        {
            ActivateAbility();
            currentTurnCount = 0;
        }
    }

    private void ActivateAbility()
    {
        List<EnemyPawn> pawnsInRadius = GetPawnsInRadius();

        foreach (EnemyPawn target in pawnsInRadius)
        {
            if (!pawn.isPossessed)
            {
                if (target.isPossessed)
                {
                    if (TurnManager.instance != null)
                    {
                        TurnManager.instance.CheckPawnDeathCondition(target);
                    }

                    target.Unpossess();
                }
            }
            else
            {
                if (!target.isPossessed)
                {
                    target.Possess();
                }
            }
        }
    }

    private List<EnemyPawn> GetPawnsInRadius()
    {
        List<EnemyPawn> affectedPawns = new List<EnemyPawn>();
        Vector2Int center = pawn.coordinate;

        for (int x = -radius;  x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (Mathf.Abs(x) +  Mathf.Abs(y) <= radius)
                {
                    Vector2Int checkCoord = center + new Vector2Int(x, y);

                    if (gridManager.Grid.ContainsKey(checkCoord))
                    {
                        GridNode node = gridManager.Grid[checkCoord];

                        if (node.IsOccupied && node.Occupant != null)
                        {
                            EnemyPawn targetPawn = node.Occupant.GetComponent<EnemyPawn>();

                            if (targetPawn != null && targetPawn != this.pawn)
                            {
                                affectedPawns.Add(targetPawn);
                            }
                        }
                    }
                }
            }
        }
        return affectedPawns;
    }
}
