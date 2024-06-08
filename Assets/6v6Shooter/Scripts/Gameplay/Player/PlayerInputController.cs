using UnityEngine;
using UnityEngine.InputSystem;
using static SharedPlayerSettings;


public class PlayerInputController : MonoBehaviour
{
    /* private PlayerMotor playerMotor;
    private PlayerControls playerControls;

    public Vector2 inputMovement;
    public Vector2 inputView;

    private Vector2 newCameraRotation;

    [Header("References")]
    public Transform cameraHolder;

    [Header("Settings")]
    public PlayerSettingsModel playerSettings;
    public float viewClampYMin = -70;
    public float viewClampYMax = 80;

    private void Awake()
    {
        playerMotor = GetComponent<PlayerMotor>();
        
        playerControls = new PlayerControls();
        playerControls.PlayerBase.Movement.performed += e => inputMovement = e.ReadValue<Vector2>();
        playerControls.PlayerBase.Look.performed += e => inputView = e.ReadValue<Vector2>();

        playerControls.Enable();

        newCameraRotation = cameraHolder.localRotation.eulerAngles;
    }

    private void Update()
    {
        CalculateView();
        CalculateMovement();
    }

    private void CalculateView()
    {
        newCameraRotation.x += playerSettings.ViewYSensitivity * inputView.y * Time.deltaTime;
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, viewClampYMin, viewClampYMax);

        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);
    }

    private void CalculateMovement()
    {

    } */
}
