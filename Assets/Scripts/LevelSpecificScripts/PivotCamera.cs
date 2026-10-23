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
    public float minFOV = 25f;
    public float maxFOV = 70f;

    public float pinchDeadzone = 2.5f;

    private bool wasTwoFingersTouching = false;
    private Vector2 prevPos0;
    private Vector2 prevPos1;

    private void Update()
    {
        var touch = Touchscreen.current;
        if (touch == null || mainCamera == null) return;

        bool touch0Active = touch.touches[0].press.isPressed;
        bool touch1Active = touch.touches[1].press.isPressed;

        if (touch0Active && touch1Active)
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

        if (!wasTwoFingersTouching)
        {
            wasTwoFingersTouching = true;
            prevPos0 = pos0;
            prevPos1 = pos1;
            return;
        }

        Vector2 delta0 = pos0 - prevPos0;
        Vector2 delta1 = pos1 - prevPos1;

        float prevDistance = Vector2.Distance(prevPos0, prevPos1);
        float currentDistance = Vector2.Distance(pos0, pos1);

        prevPos0 = pos0;
        prevPos1 = pos1;

        float averageDeltaX = (delta0.x + delta1.x) * 0.5f;
        float zoomDelta = currentDistance - prevDistance;

        if (Mathf.Abs(averageDeltaX) > Mathf.Abs(zoomDelta) || Mathf.Abs(zoomDelta) < pinchDeadzone)
        {
            transform.Rotate(Vector3.up, averageDeltaX * rotationSpeed, Space.World);
        }
        else
        {
            if (mainCamera.orthographic)
            {
                mainCamera.orthographicSize -= zoomDelta * (zoomDelta * 0.1f);
                mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, 2f, 12f);
            }
            else
            {
                mainCamera.fieldOfView -= zoomDelta * zoomSpeed;
                mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, minFOV, maxFOV);
            }
        }
    }
}
