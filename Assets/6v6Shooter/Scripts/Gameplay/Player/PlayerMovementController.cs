using UnityEngine;


public class PlayerMovementController : MonoBehaviour
{
    private PlayerMotor playerMotor;

    public float currentSpeed;
    public Vector3 jumpForce;
    public float lookSensitivity;

    private Rigidbody playerBody;
    private Vector3 velocity;
    private Vector3 rotation;

    private void Awake()
    {
        playerMotor = GetComponent<PlayerMotor>();
        playerBody = GetComponent<Rigidbody>();
        SetJumpForce();
    }

    void Start()
    {
        velocity = Vector3.zero;
        rotation = Vector3.zero;
        //lookSensitivity = playerMotor.inputController.Gamepad.current != null ? 10f : 100f;
    }

    private void Update()
    {
        if (!playerMotor.photonView.IsMine)
            return;

        ControllerMovement();
    }

    private void ControllerMovement()
    {
        SprintMovmement();

        Vector2 movementInput = new Vector2(playerMotor.inputController.right.ReadValue<float>() 
                                            - playerMotor.inputController.left.ReadValue<float>()
                                            , playerMotor.inputController.forward.ReadValue<float>() 
                                            - playerMotor.inputController.backward.ReadValue<float>());
                                            
        Vector3 horizontalMovement = transform.right * movementInput.x;
        Vector3 verticalMovement = transform.forward * movementInput.y;

        playerMotor.animationController.PlayWalkingAnimation(movementInput);

        Vector3 movementVelocity = (horizontalMovement + verticalMovement).normalized * currentSpeed;

        SetVelocity(movementVelocity);
    }

    private void SprintMovmement()
    {
        bool isSprinting = playerMotor.inputController.sprintAction.IsPressed();
        playerMotor.animationController.PlaySprintAnimation(isSprinting);
        currentSpeed = isSprinting ? 5f : 3f;
    }

    private void SetJumpForce()
    {
        jumpForce = new Vector3(0.0f, 1.5f, 0.0f);
    }

    private void SetVelocity(Vector3 movementVelocity)
    {
        velocity = movementVelocity;
    }
}
