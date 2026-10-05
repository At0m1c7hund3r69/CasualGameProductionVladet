using UnityEngine;
using UnityEngine.InputSystem;

public class PawnSightline : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public EnemyPawn baseUnit;

    [Header("Settings")]
    public Vector2Int lookDirection = new Vector2Int(0, 1);
    public int sightRange = 3;

    public void CheckLineOfSight()
    {
        if (gridManager == null || baseUnit == null) return;

        if (baseUnit.isPossessed) return;

        Vector2Int currentCheckCoord = baseUnit.coordinate;
        
        for (int i = 1;  i <= sightRange; i++)
        {
            currentCheckCoord += lookDirection;

            if (!gridManager.Grid.ContainsKey(currentCheckCoord))
            {
                break;
            }

            GridNode node = gridManager.Grid[currentCheckCoord];

            if (node.IsOccupied && node.Occupant != null)
            {
                EnemyPawn targetEnemy = node.Occupant.GetComponent<EnemyPawn>();

                if(targetEnemy != null && targetEnemy.isPossessed)
                {
                    baseUnit.ExecuteKillMove(targetEnemy, currentCheckCoord);
                    return;
                }
                else
                {
                    break;
                }
            }
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log($"{gameObject.name} is scanning...");
            CheckLineOfSight();
        }
    }

    public void Rotate(Vector2Int newDirection)
    {
        lookDirection = newDirection;

        Vector3 direction3D = new Vector3(newDirection.x, 0, newDirection.y);
        if (direction3D != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction3D);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (baseUnit == null || gridManager == null) return;

        Gizmos.color = Color.red;
        Vector3 startPos = transform.position + Vector3.up * 0.5f;
        Vector3 direction3D = new Vector3(lookDirection.x, 0, lookDirection.y);
        Gizmos.DrawRay(startPos, direction3D * sightRange);
    }
}
