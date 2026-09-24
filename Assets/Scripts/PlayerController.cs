using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference touchContactAction;
    public InputActionReference touchDeltaAction;

    [Header("Movement Settings")]
    public float swipeThreshhold = 50f;

    private Vector2 accumulatedDelta;
    private bool isDragging = false;
    public PlayerGridMovement movementComponent;

    void OnEnable()
    {
        touchContactAction.action.started += StartDrag;
        touchContactAction.action.canceled += EndDrag;

        touchContactAction.action.Enable();
        touchDeltaAction.action.Enable();
    }

    void OnDisable()
    {
        touchContactAction.action.started -= StartDrag;
        touchContactAction.action.canceled -= EndDrag;

        touchContactAction.action.Disable();
        touchDeltaAction.action.Disable();
    }

    void StartDrag(InputAction.CallbackContext context)
    {
        isDragging = true;
        accumulatedDelta = Vector2.zero;
    }

    void EndDrag(InputAction.CallbackContext context)
    {
        isDragging = false;
    }
    void Update()
    {
        if (!isDragging) return;

        Vector2 delta = touchDeltaAction.action.ReadValue < Vector2>();
        accumulatedDelta += delta;

        if (accumulatedDelta.magnitude > swipeThreshhold)
        {
            DetermineDirection(accumulatedDelta);
            accumulatedDelta = Vector2.zero;
        }
    }

    void DetermineDirection(Vector2 rawSwipe)
    {
        Vector3 moveDirection = Vector3.zero;

        if (Mathf.Abs(rawSwipe.x) > Mathf.Abs(rawSwipe.y))
        {
            moveDirection = rawSwipe.x >0? Vector3.right : Vector3.left;
            movementComponent.AttemptMove(moveDirection);
        }
        else
        {
            moveDirection = rawSwipe.y > 0 ? Vector3.forward : Vector3.back;
            movementComponent.AttemptMove(moveDirection);
        }
    }
}
