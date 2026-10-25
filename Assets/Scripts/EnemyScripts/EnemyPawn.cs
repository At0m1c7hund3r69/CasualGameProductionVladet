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

    [Header("VFX")]
    public GameObject possessVfx;
    public Vector3 vfxOffset = new Vector3(0, 0.5f, 0);

    [Header("SFX")]
    public AudioClip possessSfx;
    public AudioClip moveSfx;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private Material originalMaterial;
    private Renderer pawnRenderer;

    private void Awake()
    {
        pawnRenderer = GetComponentInChildren<Renderer>();
        if (pawnRenderer != null )
        {
            originalMaterial = pawnRenderer.material;
        }
    }
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

        if (VillegersLeft.Instance != null) //This was added by chris. Calls the VillegersLeft Script to lower it once an enemy is possessd
        {
            VillegersLeft.Instance.Villagers(-1);
        }

        if (possessVfx != null)
        {
            GameObject vfx = Instantiate(possessVfx, transform.position + vfxOffset, Quaternion.identity);
            Destroy(vfx, 1.5f);
        }

        if (possessSfx != null)
        {
            AudioSource.PlayClipAtPoint(possessSfx, Camera.main.transform.position, sfxVolume);
        }

        if (meshRenderer  != null && possessedMaterial != null)
        {
            meshRenderer.material = possessedMaterial;
        }
        
        if (TurnManager.instance == null)
        {
            TurnManager.instance.CheckWinLoseState();
        }
    }

    public void Unpossess()
    {
        isPossessed = false;

        if (pawnRenderer !=null && originalMaterial != null)
        {
            pawnRenderer.material = originalMaterial;
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

                InteractableObject interactable = targetNode.Occupant.GetComponent<InteractableObject>();

                if (interactable != null)
                {
                    interactable.TriggerInteraction();
                    targetNode.Occupant = null;
                    return true;
                }
            }
            return false;
        }

        PawnSightline sightline = GetComponent<PawnSightline>();
        if (sightline != null)
        {
            sightline.Rotate(direction);

            if (sightline.CheckLineOfSight())
            {
                return true;
            }
        }

        StartCoroutine(SlideCoroutine(targetNode, targetCoord));
        return true;
    }

    public bool ExecuteAIMove(Vector2Int direction)
    {
        if (gridManager == null) return false;

        Vector2Int targetCoord = coordinate + direction;

        if (!gridManager.Grid.ContainsKey (targetCoord)) return false;

        GridNode targetNode = gridManager.Grid[targetCoord];

        if (targetNode.IsOccupied)
        {
            return false;
        }

        GetComponent<PawnSightline>().Rotate(direction);
        StartCoroutine(SlideCoroutine(targetNode, targetCoord));
        return true;
    }

    public void ExecuteKillMove(EnemyPawn targetEnemy, Vector2Int targetCoord)
    {
        if (TurnManager.instance != null && targetEnemy  != null)
        {
            TurnManager.instance.CheckPawnDeathCondition(targetEnemy);
        }

            GridNode targetNode = gridManager.Grid[targetCoord];
            StartCoroutine(KillAndMoveCoroutine(targetEnemy, targetNode, targetCoord));
    }

    private System.Collections.IEnumerator KillAndMoveCoroutine(EnemyPawn targetEnemy, GridNode targetNode, Vector2Int targetCoord)
    {
        if (TurnManager.instance != null)
        {
            TurnManager.instance.CheckPawnDeathCondition(this);
        }

        if (moveSfx != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(moveSfx, Camera.main.transform.position, sfxVolume);
        }

        if (TurnManager.instance)TurnManager.instance.UnRegisterEnemy(targetEnemy);
        Destroy(targetEnemy.gameObject);

        gridManager.Grid[coordinate].IsOccupied = false;
        gridManager.Grid[coordinate].Occupant = null;

        coordinate = targetCoord;
        targetNode.IsOccupied = true;
        targetNode.Occupant = this.gameObject;

        Vector3 startPos = transform.position;
        Vector3 endPos = targetNode.WorldPosition;
        float elapsedTime = 0f;
        float moveDuration = 0.2f;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;

        if (TurnManager.instance)
        {
            TurnManager.instance.CheckWinLoseState();
        }
    }

    private System.Collections.IEnumerator SlideCoroutine(GridNode targetNode, Vector2Int targetCoord)
    {
        if (moveSfx != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(moveSfx, Camera.main.transform.position, sfxVolume);
        }

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
        if (moveSfx != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(moveSfx, Camera.main.transform.position, sfxVolume);
        }

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
