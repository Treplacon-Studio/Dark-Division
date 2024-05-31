using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private GameObject fpsCamera;
    [SerializeField] private float lookSensitivity = 8f;
    [SerializeField] private float camYUpwardRotation;
    [SerializeField] private float camYDownwardRotation;

    private float cameraUpAndDownRotation = 0f;
    private float currentCamRotation = 0;

    public void RotateCamera(float camUpAndDownRotation)
    {
        cameraUpAndDownRotation = camUpAndDownRotation;
    }

    private void FixedUpdate()
    {
        if (fpsCamera != null)
        {
            currentCamRotation -= cameraUpAndDownRotation;
            currentCamRotation = Mathf.Clamp(currentCamRotation, camYUpwardRotation, camYDownwardRotation);
            fpsCamera.transform.localEulerAngles = new Vector3(currentCamRotation, 0, 0);
        }
    }
}
