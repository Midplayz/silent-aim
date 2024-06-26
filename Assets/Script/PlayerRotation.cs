using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public float touchSensitivity = 0.1f;

    public float normalFOV = 60f;
    public float scopedFOV = 20f;
    public float zoomSpeed = 10f;
    public GameObject sniper; 
    public Vector3 scopedPosition = new Vector3(0, -0.2f, 0.5f); 

    private Vector3 originalSniperPosition;
    public GameObject scopeImage;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private Camera playerCamera;

    private bool isScoping = false;
    private bool hasSniperMoved = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera = Camera.main;
        originalSniperPosition = sniper.transform.localPosition;
        scopeImage.SetActive(false);
    }

    private void Update()
    {
        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("Is PC");
            RotateWithMouse();
            HandleScoping();
        }
        else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Debug.Log("Is mobile");
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

    private void HandleScoping()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isScoping = true;
            hasSniperMoved = false;
            scopeImage.SetActive(false);
        }
        if (Input.GetMouseButtonUp(1))
        {
            isScoping = false;
            hasSniperMoved = false;
            sniper.SetActive(true);
            scopeImage.SetActive(false);
        }

        if (isScoping)
        {
            MoveSniperToScopedPosition();
            if (hasSniperMoved)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, scopedFOV, zoomSpeed * Time.deltaTime);
                sniper.SetActive(false);
                scopeImage.SetActive(true);
            }
        }
        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, normalFOV, zoomSpeed * Time.deltaTime);
            sniper.transform.localPosition = Vector3.Lerp(sniper.transform.localPosition, originalSniperPosition, zoomSpeed * Time.deltaTime);
        }
    }

    private void MoveSniperToScopedPosition()
    {
        sniper.transform.localPosition = Vector3.Lerp(sniper.transform.localPosition, scopedPosition, zoomSpeed * Time.deltaTime);
        if (Vector3.Distance(sniper.transform.localPosition, scopedPosition) < 0.01f)
        {
            hasSniperMoved = true;
        }
    }
}
