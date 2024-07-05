using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [field: Header("Aiming Input")]
    [field: SerializeField] private float mouseSensitivity = 100f;
    [field: SerializeField] private float touchSensitivity = 0.1f;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private Camera playerCamera;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera = Camera.main;
    }

    private void Update()
    {
        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            RotateWithMouse();
        }
        else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            RotateWithTouch();
        }
    }

    private void RotateWithMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation += mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    private void RotateWithTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float touchX = touch.deltaPosition.x * touchSensitivity;
                float touchY = touch.deltaPosition.y * touchSensitivity;

                xRotation += touchY;
                yRotation += touchX;

                xRotation = Mathf.Clamp(xRotation, -90f, 90f);
                yRotation = Mathf.Clamp(yRotation, -90f, 90f);

                transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
            }
        }
    }
}
