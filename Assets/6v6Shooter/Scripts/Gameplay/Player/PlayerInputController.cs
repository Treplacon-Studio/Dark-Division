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
    public InputAction inspectAction;
    public InputAction reloadAction;

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
        inspectAction.Enable();
        reloadAction.Enable();
    }

    private void RegisterInputActionCallbacks()
    {
        aimAction.performed += ctx => OnAim();
        aimAction.canceled += ctx => OnStopAim();
        shootAction.performed += ctx => OnShoot();
        jumpAction.performed += ctx => OnJump();
        inspectAction.performed += ctx => OnInspect();
        reloadAction.performed += ctx => OnReload();
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

    private void OnJump()
    {
        playerMotor.animationController.PlayJumpAnimation();
    }

    private void OnInspect()
    {
        playerMotor.animationController.PlayInspectAnimation();
    }

    private void OnReload()
    {
        playerMotor.animationController.PlayReloadAnimation();
    }
}
