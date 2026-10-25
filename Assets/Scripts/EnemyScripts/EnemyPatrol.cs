using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(EnemyPawn))]
public class EnemyPatrol : MonoBehaviour
{
    private EnemyPawn pawn;

    [Header("Patrol Route")]
    public List<Vector2Int> moveSequence = new List<Vector2Int>();

    private int currentStep = 0;

    void Awake()
    {
        pawn = GetComponent<EnemyPawn>();
    }

    public void TakePatrolStep()
    {
        if (moveSequence.Count == 0 || pawn.isPossessed) return;
        
        Vector2Int direction = moveSequence[currentStep];
        PawnSightline sightline = GetComponent<PawnSightline>();
        
        if (sightline != null)
        {
            sightline.Rotate(direction);
        }

        bool moveSuccessful = pawn.ExecuteAIMove(direction);

        if (moveSuccessful)
        {
            currentStep = (currentStep + 1) % moveSequence.Count;
        }

        if (sightline != null)
        {
            sightline.CheckLineOfSight();
        }
    }
}