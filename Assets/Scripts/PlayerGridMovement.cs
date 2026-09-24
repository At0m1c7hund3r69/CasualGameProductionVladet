using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class PlayerGridMovement : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;

    [Header("Settings")]
    public Vector2Int startingCoordinate = new Vector2Int(0,0);
    public float moveSpeed = 5f;

    public Vector2Int CurrentCoordinate { get; private set;  }
    private bool isMoving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentCoordinate = startingCoordinate;

        if (gridManager != null && gridManager.Grid.ContainsKey(CurrentCoordinate))
        {
            transform.position = gridManager.Grid[CurrentCoordinate].WorldPosition;

            gridManager.Grid[CurrentCoordinate].IsOccupied = true;
            gridManager.Grid[CurrentCoordinate].Occupant = this.gameObject;
        }
    }

    public void AttemptMove(Vector3 direction)
    {
        if (isMoving) return;

        Vector2Int gridDirection = new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.z));
        Vector2Int targetCoordinate = CurrentCoordinate + gridDirection;

        if (gridManager.Grid.ContainsKey(targetCoordinate))
        {
            GridNode targetNode = gridManager.Grid[targetCoordinate];

            if (!targetNode.IsOccupied)
            {
                StartCoroutine(SlideToTile(targetNode));
            }
            else
            {
                GameObject obstacle = targetNode.Occupant;
                EnemyPawn enemy = obstacle.GetComponent<EnemyPawn>();

                if (enemy != null && !enemy.isPossessed)
                {
                    enemy.Possess();
                }
                else
                {
                    Debug.Log("Path Blocked by: " + targetNode.Occupant.name);
                }
            }
        }
    }

    private IEnumerator SlideToTile(GridNode targetNode)
    {
        isMoving = true;

        gridManager.Grid[CurrentCoordinate].IsOccupied = false;
        gridManager.Grid[CurrentCoordinate].Occupant = null;

        Vector3 targetPosition = targetNode.WorldPosition;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
            
        transform.position = targetPosition;
        CurrentCoordinate = targetNode.Coordinate;

        targetNode.IsOccupied = true;
        targetNode.Occupant = this.gameObject;

        isMoving = false;
    }
}
