using UnityEngine;

public class RagdollCameraController : MonoBehaviour
{
    public float sensitivity = 5.0f;
    public float maxYAngle = 80f;

    private Vector2 currentRotation;

    void Start()
    {
        currentRotation = new Vector2(transform.eulerAngles.y, transform.eulerAngles.x);
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor for free movement
    }

    void Update()
    {
        // Get mouse input
        currentRotation.x += Input.GetAxis("Mouse X") * sensitivity;
        currentRotation.y -= Input.GetAxis("Mouse Y") * sensitivity;
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);

        // Apply rotation to the camera
        transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
    }
}
