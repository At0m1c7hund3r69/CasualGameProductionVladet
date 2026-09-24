using UnityEngine;

public class GridNode
{
    public Vector2Int Coordinate;
    public Vector3 WorldPosition;
    public bool IsOccupied;
    public GameObject Occupant;

    public GridNode(Vector2Int coord, Vector3 worldPos)
    {
        Coordinate = coord;
        WorldPosition = worldPos;
        IsOccupied = false;
        Occupant = null;
    }
}
