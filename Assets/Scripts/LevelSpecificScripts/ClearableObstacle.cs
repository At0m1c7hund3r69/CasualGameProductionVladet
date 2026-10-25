using UnityEngine;

public class ClearableObstacle: MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;

    [Header("Settings")]
    public Vector2Int coordinate;

    private void Start()
    {
        if (gridManager != null && gridManager.Grid.ContainsKey(coordinate))
        {
            transform.position = gridManager.Grid[coordinate].WorldPosition;
            gridManager.Grid[coordinate].IsOccupied = true;
            gridManager.Grid[coordinate].Occupant = this.gameObject;
        }
        else
        {
            Debug.LogWarning($"Obstacle {name} placed at invalid coordinate: {coordinate}");
        }
    }

    private void OnDisable()
    {
        if (gridManager != null && gridManager.Grid.ContainsKey(coordinate))
        {
            gridManager.Grid[coordinate].IsOccupied = false;

            if (gridManager.Grid[coordinate].Occupant == this.gameObject)
            {
                gridManager.Grid[coordinate].Occupant = null;
            }
        }
    }
}
