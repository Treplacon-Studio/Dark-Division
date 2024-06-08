using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputController : MonoBehaviour
{
    private PlayerMotor playerMotor;

    public Gamepad Gamepad;

    private PlayerControls playerControls;

    private void Awake()
    {
        playerMotor = GetComponent<PlayerMotor>();
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return playerControls.PlayerBase.Movement.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        return playerControls.PlayerBase.Look.ReadValue<Vector2>();
    }

    /* public void OnAim()
    {
        playerMotor.animationController.PlayAimDownSightAnimation();
        playerMotor.movementController.lookSensitivity = 2f;
        playerMotor.movementController.currentSpeed = 1f;
    }

    public void OnStopAim()
    {
        playerMotor.animationController.PlayStopAimDownSightAnimation();
        playerMotor.movementController.lookSensitivity = Gamepad.current != null ? 10f : 100f;
        playerMotor.movementController.currentSpeed = 3f;
    } */

    public bool OnSprint()
    {
        return playerControls.PlayerBase.Sprint.IsPressed();
    }

    public bool OnJump()
    {
        return playerControls.PlayerBase.Jump.triggered;
    }
}
