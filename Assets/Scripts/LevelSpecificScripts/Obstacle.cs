using Unity.VisualScripting;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;

    [Header("Settings")]
    public Vector2Int coordinate;

    void Start()
    {
        if(gridManager != null && gridManager.Grid.ContainsKey(coordinate))
        {
            transform.position = gridManager.Grid[coordinate].WorldPosition;
            gridManager.Grid[coordinate].IsOccupied = true;
            gridManager.Grid[coordinate].Occupant = this.gameObject;
        }
        else
        {
            Debug.LogWarning($"Obstace {name} placed at invalid coordinate: {coordinate}");
        }
    }
}
