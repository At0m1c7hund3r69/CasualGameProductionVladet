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
    public float minZoom = 2f;
    public float maxZoom = 10f;

    private void Update()
    {
        var touch = Touchscreen.current;
        if (touch == null) return;

        bool touch0active = touch.touches[0].press.isPressed;
        bool touch1active = touch.touches[1].press.isPressed;

        if (touch0active && touch1active)
        {
            HandleRotation(touch);
        }
    }

    private void HandleRotation(Touchscreen touch)
    {
        var touch0 = touch.touches[0];
        var touch1 = touch.touches[1];

        if (touch0.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began ||
            touch1.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began ||
            touch0.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Ended ||
            touch1.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Ended)
        {
            return;
        }

        float deltaX0 = touch0.delta.ReadValue().x;
        float deltaX1 = touch1.delta.ReadValue().x;
        float averageDeltaX = (deltaX0 + deltaX1) / 2f;

        transform.Rotate(Vector3.up, averageDeltaX * rotationSpeed, Space.World);
    }
}
