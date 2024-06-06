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
        //lookSensitivity = 2f;
        //speed = 1f;
    }

    private void OnStopAim()
    {
        playerMotor.animationController.PlayStopAimDownSightAnimation();
        //lookSensitivity = Gamepad.current != null ? 10f : 100f;
        //speed = 3f;
    }

    private void OnShoot()
    {
        playerMotor.animationController.PlayShootAnimation();
    }
}
