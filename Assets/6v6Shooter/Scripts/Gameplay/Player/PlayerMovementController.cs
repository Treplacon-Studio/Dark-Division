using UnityEngine;


public class PlayerMovementController : MonoBehaviour
{
    private PlayerMotor playerMotor;

    public float currentSpeed;
    public float jumpForce;
    public float lookSensitivity;
    public bool isGrounded;
    public Vector3 jump;

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
        currentSpeed = 3f;
        lookSensitivity = playerMotor.inputController.Gamepad != null ? 10f : 100f;
    }

    private void Update()
    {
        if (!playerMotor.photonView.IsMine)
            return;

        ControllerMovement();
    }

    void FixedUpdate()
    {
        if (!playerMotor.photonView.IsMine)
            return;

        if (velocity != Vector3.zero)
            playerBody.MovePosition(playerBody.position + velocity * Time.fixedDeltaTime);

        playerBody.MoveRotation(playerBody.rotation * Quaternion.Euler(rotation));
    }

    private void ControllerMovement()
    {
        Vector2 movementInput = new Vector2(playerMotor.inputController.right.ReadValue<float>() 
                                            - playerMotor.inputController.left.ReadValue<float>()
                                            , playerMotor.inputController.forward.ReadValue<float>() 
                                            - playerMotor.inputController.backward.ReadValue<float>());
                                            
        Vector3 horizontalMovement = transform.right * movementInput.x;
        Vector3 verticalMovement = transform.forward * movementInput.y;

        playerMotor.animationController.PlayWalkingAnimation(movementInput);

        Vector3 movementVelocity = (horizontalMovement + verticalMovement).normalized * currentSpeed;

        SetVelocity(movementVelocity);
        SprintMovmement();
        JumpMovement();
    }

    private void SprintMovmement()
    {
        bool isSprinting = playerMotor.inputController.sprintAction.IsPressed();
        playerMotor.animationController.PlaySprintAnimation(isSprinting);
        currentSpeed = isSprinting ? 5f : 3f;
    }

    private void SetJumpForce()
    {
        jumpForce = 2f;
        jump = new Vector3(0.0f, 1.5f, 0.0f);
    }

    private void JumpMovement()
    {
        if (playerMotor.inputController.jumpAction.triggered && isGrounded)
        {
            playerBody.AddForce(jump * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void SetVelocity(Vector3 movementVelocity)
    {
        velocity = movementVelocity;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}
