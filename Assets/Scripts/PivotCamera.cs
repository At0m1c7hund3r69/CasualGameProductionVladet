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

    private bool isInteractingWithBoard = false;
    private float currentCamDistance;

    private void Start()
    {
        if(mainCamera != null)
        {
            currentCamDistance = Vector3.Distance(transform.position, mainCamera.transform.position);
        }
    }

    private void Update()
    {
        var touch = Touchscreen.current;
        if (touch == null) return;

        bool touch0Active = touch.touches[0].press.isPressed;
        bool touch1Active = touch.touches[1].press.isPressed;

        if (touch0Active && !touch1Active)
        {
            HandleRotation(touch);
        }
        //else if (touch0Active && touch1Active)
        //{
        //    HandleZoom(touch);
        //    isInteractingWithBoard = false;
        //}
        else
        {
            isInteractingWithBoard = false;
        }
    }
    private void HandleRotation(Touchscreen touch)
    {
        var primaryTouch = touch.touches[0];

        if (primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
        {
            Ray ray = mainCamera.ScreenPointToRay(primaryTouch.position.ReadValue());

            if (Physics.Raycast(ray))
            {
                isInteractingWithBoard = true;
            }
        }

        if (!isInteractingWithBoard)
        {
            Vector2 delta = primaryTouch.delta.ReadValue();
            transform.Rotate(Vector3.up, delta.x * rotationSpeed, Space.World);
        }
    }

    private void HandleZoom(Touchscreen touch)
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

        Vector2 touch0Pos = touch0.position.ReadValue();
        Vector2 touch1Pos = touch1.position.ReadValue();

        Vector2 touch0PrevPos = touch0Pos - touch0.delta.ReadValue();
        Vector2 touch1PrevPos = touch1Pos - touch1.delta.ReadValue();

        float prevDistance = Vector2.Distance(touch0PrevPos, touch1PrevPos);
        float currentDistance = Vector2.Distance(touch0Pos, touch1Pos);

        float zoomDelta = currentDistance - prevDistance;

        Vector3 localPos = mainCamera.transform.localPosition;

        localPos.z += zoomDelta * zoomSpeed;

        localPos.z = Mathf.Clamp(localPos.z, -maxZoom, -minZoom);

        mainCamera.transform.localPosition = localPos;
    }
}
