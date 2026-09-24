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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if(gridManager != null && gridManager.Grid.ContainsKey(coordinate))
        {
            transform.position = gridManager.Grid[coordinate].WorldPosition;

            gridManager.Grid[coordinate].IsOccupied = true;
            gridManager.Grid[coordinate].Occupant = this.gameObject;
        }
        
    }

    public void Possess()
    {
        if (isPossessed) return;
        isPossessed = true;
        Debug.Log(name + "has been possessed!");

        if (meshRenderer  != null && possessedMaterial != null)
        {
            meshRenderer.material = possessedMaterial;
        } 
    }
}
