using Unity.VisualScripting;
using UnityEngine;

public class FreeFlyCamera : MonoBehaviour
{

    [Header("Movement Settings")]
    public float moveSpeed = 10f;

    [Header("Look Settings")]
    public float lookSpeed = 2f;
    public bool invertY = false;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 angles = transform.eulerAngles;
        rotationX = angles.x;
        rotationY = angles.y;
    }

    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX += invertY ? mouseY : -mouseY;
        rotationY += Input.GetAxis("Mouse X") * lookSpeed;

        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float verticalFly = 0f;

        if (Input.GetKey(KeyCode.E)) verticalFly = 1f;
        if (Input.GetKey(KeyCode.Q)) verticalFly = -1f;

        Vector3 moveDirection = (transform.forward * vertical) + (transform.right * horizontal) + (Vector3.up * verticalFly);
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } 
    }
}
