using Unity.VisualScripting;
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
        if (TurnManager.instance == null) return;

        Vector2 screenPos = Pointer.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            EnemyPawn clickedPawn = hit.collider.GetComponent<EnemyPawn>();

            if (TurnManager.instance.currentState == TurnState.FirstBlood)
            {
                if (!clickedPawn.isPossessed)
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

    void HandleTouchEnd(InputAction.CallbackContext context)
    {
        isDragging = false;
        activeSwipePawn = null;
    }

    void Update()
    {
        if (!isDragging || activeSwipePawn == null || TurnManager.instance.currentState != TurnState.PlayerTurn) return;

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

        if (actionSuccessful)
        {
            TurnManager.instance.EndPlayerTurn();
        }
    }
}
