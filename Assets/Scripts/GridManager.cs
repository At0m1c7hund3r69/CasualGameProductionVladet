using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 8;
    public int height = 8;
    public float cellSize = 1f;

    public Dictionary<Vector2Int, GridNode> Grid { get; private set; }

    void Awake()
    {
        GenerateGrid();
    }

    private void OnValidate()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        Grid = new Dictionary<Vector2Int, GridNode>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int coord = new Vector2Int(x, y);
                Vector3 worldPos = transform.position + new Vector3(x * cellSize, 0, y * cellSize);
                Grid.Add(coord, new GridNode(coord, worldPos));
            }
        }
    }

    void OnDrawGizmos()
    {
        if (Grid == null) return;

        Gizmos.color = Color.yellow;
        foreach (var node in Grid.Values)
        {
            Gizmos.DrawWireCube(node.WorldPosition, new Vector3(cellSize, 0.1f, cellSize));
        }
    }
}