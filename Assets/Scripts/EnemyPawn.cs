using UnityEngine;

public class EnemyPawn : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public Material possessedMaterial;
    private MeshRenderer meshRenderer;

    [Header("Settings")]
    public Vector2Int coordinate;
    public bool isPossessed = false;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if(gridManager != null && gridManager.Grid.ContainsKey(coordinate))
        {
            transform.position = gridManager.Grid[coordinate].WorldPosition;

            gridManager.Grid[coordinate].IsOccupied = true;
            gridManager.Grid[coordinate].Occupant = this.gameObject;
        }

        if (TurnManager.instance != null) TurnManager.instance.RegisterEnemy(this);
    }

    public void Possess()
    {
        if (isPossessed) return;
        isPossessed = true;

        if (meshRenderer  != null && possessedMaterial != null)
        {
            meshRenderer.material = possessedMaterial;
        } 
    }
}
