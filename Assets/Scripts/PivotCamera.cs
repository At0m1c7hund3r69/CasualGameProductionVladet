using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PivotCamera : MonoBehaviour
{
    [Header("Reference")]
    public Camera mainCamera;
    public InputActionAsset inputAsset;

    [Header("Settings")]
    public float rotationSpeed = 0.2f;
    public float zoomSpeed = 0.05f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    private InputAction deltaAction;
    private InputAction zoomAction;
    private InputAction pressAction;

    private void OnEnable()
    {
        deltaAction = inputAsset.FindAction("Game/Camera");
        zoomAction = inputAsset.FindAction("Game/Zoom");
        pressAction = inputAsset.FindAction("Game/Primary");

        deltaAction.Enable();
        zoomAction.Enable();
        pressAction.Enable();
    }

    private void OnDisable()
    {
        deltaAction.Disable();
        zoomAction.Disable();
        pressAction.Disable();
    }

    private void HandleRotation()
    {
        if (pressAction.ReadValue<float>() > 0)
        {
            Vector2 delta = deltaAction.ReadValue<Vector2>();
            transform.Rotate(Vector3.up, delta.x * rotationSpeed, Space.World);
        }
    }

    private void Update()
    {
        HandleRotation();
        HandleZoom();
    }

    private void HandleZoom()
    {
        var touch = UnityEngine.InputSystem.Touchscreen.current;
        if (touch != null && touch.touches[0].press.isPressed && touch.touches[1].press.isPressed)
        {
            Vector2 touch0Pos = touch.touches[0].position.ReadValue();
            Vector2 touch1Pos = touch.touches[1].position.ReadValue();

            Vector2 touch0PrevPos = touch0Pos - touch.touches[0].delta.ReadValue();
            Vector2 touch1PrevPos = touch1Pos - touch.touches[1].delta.ReadValue();

            float prevDistance = Vector2.Distance(touch0PrevPos, touch1PrevPos);
            float currentDistance = Vector2.Distance(touch0Pos, touch1Pos);
            float zoomDelta = currentDistance - prevDistance;

            Vector3 localPos = mainCamera.transform.localPosition;
            localPos.z += zoomDelta * zoomSpeed;

            localPos.z = Mathf.Clamp(localPos.z, -maxZoom, -minZoom);
            mainCamera.transform.localPosition = localPos;
        }
    }
}
