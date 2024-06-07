using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputController : MonoBehaviour
{
    private PlayerMotor playerMotor;

    public InputAction forward;
    public InputAction backward;
    public InputAction left;
    public InputAction right;
    public InputAction jumpAction;
    public InputAction sprintAction;
    public InputAction aimAction;
    public InputAction lookAction;
    public InputAction shootAction;

    public Gamepad Gamepad;

    private void Awake()
    {
        playerMotor = GetComponent<PlayerMotor>();
        
        EnableInputActions();
        RegisterInputActionCallbacks();
    }

    private void EnableInputActions()
    {
        forward.Enable();
        backward.Enable();
        left.Enable();
        right.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
        aimAction.Enable();
        lookAction.Enable();
        shootAction.Enable();
    }

    private void RegisterInputActionCallbacks()
    {
        aimAction.performed += ctx => OnAim();
        aimAction.canceled += ctx => OnStopAim();
        shootAction.performed += ctx => OnShoot();
    }

    private void OnAim()
    {
        playerMotor.animationController.PlayAimDownSightAnimation();
        playerMotor.movementController.lookSensitivity = 2f;
        playerMotor.movementController.currentSpeed = 1f;
    }

    private void OnStopAim()
    {
        playerMotor.animationController.PlayStopAimDownSightAnimation();
        playerMotor.movementController.lookSensitivity = Gamepad.current != null ? 10f : 100f;
        playerMotor.movementController.currentSpeed = 3f;
    }

    private void OnShoot()
    {
        playerMotor.animationController.PlayShootAnimation();
    }
}
