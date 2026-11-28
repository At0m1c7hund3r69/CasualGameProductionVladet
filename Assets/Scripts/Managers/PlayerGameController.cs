using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGameController : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference touchContactAction;
    public InputActionReference touchDeltaAction;

    [Header("Settings")]
    public float swipeThreshold = 50f;

    private Vector2 accumulatedDelta;
    private bool isDragging = false;
    private EnemyPawn activeSwipePawn;

    void OnEnable()
    {
        touchContactAction.action.started += HandleTouchStart;
        touchContactAction.action.canceled += HandleTouchEnd;
        touchContactAction.action.Enable();
        touchDeltaAction.action.Enable();
    }

    void OnDisable()
    {
        touchContactAction.action.started -= HandleTouchStart;
        touchContactAction.action.canceled -= HandleTouchEnd;
        touchContactAction.action.Disable();
        touchDeltaAction.action.Disable();
    }

    void HandleTouchStart(InputAction.CallbackContext context)
    {
        bool isRivals = RivalsModeManager.instance != null;
        if (!isRivals && TurnManager.instance == null) return;
        if (Pointer.current == null || Camera.main == null) return;

        Vector2 screenPos = Pointer.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            EnemyPawn clickedPawn = hit.collider.GetComponent<EnemyPawn>();
            if (clickedPawn == null) return;

            if (isRivals)
            {
                if (RivalsModeManager.instance.isGameOver || RivalsModeManager.instance.isEnemyPhaseActive) return;

                var activePlayer = RivalsModeManager.instance.activePlayer;
                var phase = RivalsModeManager.instance.currentPhase;

                if (clickedPawn.isPossessed && clickedPawn.owner == activePlayer)
                {
                    activeSwipePawn = clickedPawn;
                    isDragging = true;
                    accumulatedDelta = Vector2.zero;
                    return;
                }

                if (!clickedPawn.isPossessed &&
                    (phase == RivalsModeManager.RivalsPhase.FirstBloodP1 ||
                     phase == RivalsModeManager.RivalsPhase.FirstBloodP2) &&
                    !PlayerOwnsAnyPawn(activePlayer))
                {
                    clickedPawn.Possess();
                    RivalsModeManager.instance.RegisterActionCompleted(true);
                }
                return;
            }

            if (TurnManager.instance.currentState == TurnState.FirstBlood)
            {
                bool isPatrolUnit = clickedPawn.GetComponent<EnemyPatrol>() != null;
                bool isPriestUnit = clickedPawn.GetComponent<PriestBehavior>() != null;

                if (!clickedPawn.isPossessed && !isPatrolUnit && !isPriestUnit)
                {
                    clickedPawn.Possess();
                    TurnManager.instance.currentState = TurnState.PlayerTurn;
                }
                return;
            }

            if (TurnManager.instance.currentState == TurnState.PlayerTurn)
            {
                if (clickedPawn.isPossessed)
                {
                    activeSwipePawn = clickedPawn;
                    isDragging = true;
                    accumulatedDelta = Vector2.zero;
                }
            }
        }
    }

    private bool PlayerOwnsAnyPawn(RivalsModeManager.RivalsPlayer player)
    {
        if (RivalsModeManager.instance == null) return false;

        foreach (EnemyPawn pawn in RivalsModeManager.instance.allPawns)
        {
            if (pawn != null && pawn.gameObject.activeInHierarchy && pawn.owner == player)
            {
                return true;
            }
        }
        return false;
    }

    void HandleTouchEnd(InputAction.CallbackContext context)
    {
        isDragging = false;
        activeSwipePawn = null;
    }

    void Update()
    {
        if (!isDragging || activeSwipePawn == null) return;

        bool canAct = (RivalsModeManager.instance != null && !RivalsModeManager.instance.isGameOver && !RivalsModeManager.instance.isEnemyPhaseActive)
            || (TurnManager.instance != null && TurnManager.instance.currentState == TurnState.PlayerTurn);

        if (!canAct) return;

        Vector2 delta = touchDeltaAction.action.ReadValue<Vector2>();
        accumulatedDelta += delta;

        if (accumulatedDelta.magnitude > swipeThreshold)
        {
            ExecuteSwipeCommand(accumulatedDelta);

            isDragging = false;
            activeSwipePawn = null;
            accumulatedDelta = Vector2.zero;
        }
    }

    void ExecuteSwipeCommand(Vector2 rawSwipe)
    {
        Vector2 screenDirection = Vector2Int.zero;

        if (Mathf.Abs(rawSwipe.x) > Mathf.Abs(rawSwipe.y))
        {
            screenDirection = rawSwipe.x > 0 ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
        }
        else
        {
            screenDirection = rawSwipe.y > 0 ? new Vector2Int(0, 1) : new Vector2Int(0, -1);
        }

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 relative3D = (camRight * screenDirection.x) + (camForward * screenDirection.y);

        Vector2Int moveDirection = Vector2Int.zero;
        if (Mathf.Abs(relative3D.x) > Mathf.Abs(relative3D.z))
        {
            moveDirection = new Vector2Int((int)Mathf.Sign(relative3D.x), 0);
        }
        else
        {
            moveDirection = new Vector2Int(0, (int)Mathf.Sign(relative3D.z));
        }

        bool actionSuccessful = activeSwipePawn.AttemptAction(moveDirection);

        if (actionSuccessful && RivalsModeManager.instance == null && TurnManager.instance != null)
        {
            TurnManager.instance.EndPlayerTurn();
        }
    }
}
