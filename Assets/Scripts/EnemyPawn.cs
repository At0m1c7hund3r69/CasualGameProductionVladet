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

    public bool AttemptAction(Vector2Int direction)
    {
        if (gridManager == null) return false;
        
        Vector2Int targetCoord = coordinate + direction;

        if (!gridManager.Grid.ContainsKey(targetCoord)) return false;

        GridNode targetNode = gridManager.Grid[targetCoord];

        if (targetNode.IsOccupied)
        {
            if (targetNode.Occupant != null)
            {
                EnemyPawn targetEnemy = targetNode.Occupant.GetComponent<EnemyPawn>();

                if (targetEnemy != null && !targetEnemy.isPossessed)
                {
                    StartCoroutine(BumpCoroutine(targetNode.WorldPosition, targetEnemy));
                    return true;
                }
            }
            return false;
        }
        StartCoroutine(SlideCoroutine(targetNode, targetCoord));
        return true;
    }

    private System.Collections.IEnumerator SlideCoroutine(GridNode targetNode, Vector2Int targetCoord)
    {
        gridManager.Grid[coordinate].IsOccupied = false;
        gridManager.Grid[coordinate].Occupant = null;

        coordinate = targetCoord;
        targetNode.IsOccupied = true;
        targetNode.Occupant = this.gameObject;

        Vector3 startPos = transform.position;
        Vector3 endPos = targetNode.WorldPosition;
        float elapsedTime = 0f;
        float moveDuration = 0.2f;

        while (elapsedTime  < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
    }

    private System.Collections.IEnumerator BumpCoroutine(Vector3 targetWorldPos, EnemyPawn targetEnemy)
    {
        Vector3 startPos = transform.position;
        Vector3 bumpPos = Vector3.Lerp(startPos, targetWorldPos, 0.5f);

        float elapsedTime = 0f;
        float moveDuration = 0.1f;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, bumpPos, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetEnemy.Possess();

        elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(bumpPos, startPos, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;
    }
}
