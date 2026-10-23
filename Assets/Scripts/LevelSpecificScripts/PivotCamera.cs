using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PivotCamera : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;

    [Header("Settings")]
    public float rotationSpeed = 0.2f;
    public float zoomSpeed = 0.05f;
    public float minPivotHeight = 2f;
    public float maxPivotHeight = 10f;

    private bool wasTwoFingersTouching = false;
    private float prevPinchDistance = 0f;

    private void Start()
    {
        float startY = transform.position.y;
        if (startY < minPivotHeight) minPivotHeight = startY - 2f;
        if (startY > maxPivotHeight) maxPivotHeight = startY + 5f;
    }

    private void Update()
    {
        var touch = Touchscreen.current;
        if (touch == null) return;

        bool touch0active = touch.touches[0].press.isPressed;
        bool touch1active = touch.touches[1].press.isPressed;

        if (touch0active && touch1active)
        {
            HandleTwoFingerInput(touch);
        }
        else
        {
            wasTwoFingersTouching = false;
        }
    }

    private void HandleTwoFingerInput(Touchscreen touch)
    {
        Vector2 pos0 = touch.touches[0].position.ReadValue();
        Vector2 pos1 = touch.touches[1].position.ReadValue();
        float currentPinchDistance = Vector2.Distance(pos0, pos1);

        if (!wasTwoFingersTouching)
        {
            wasTwoFingersTouching = true;
            prevPinchDistance = currentPinchDistance;
            return;
        }

        float deltaX0 = touch.touches[0].delta.ReadValue().x;
        float deltaX1 = touch.touches[1].delta.ReadValue().x;
        float averageDeltaX = (deltaX0 - deltaX1) * 0.5f;

        transform.Rotate(Vector3.up, averageDeltaX * rotationSpeed, Space.World);

        float zoomDelta = currentPinchDistance - prevPinchDistance;
        prevPinchDistance = currentPinchDistance;

        Vector3 pivotPos = transform.position;

        pivotPos.y -= zoomDelta * zoomSpeed;
        pivotPos.y = Mathf.Clamp(pivotPos.y, minPivotHeight, maxPivotHeight);

        transform.position = pivotPos;
    }
}
