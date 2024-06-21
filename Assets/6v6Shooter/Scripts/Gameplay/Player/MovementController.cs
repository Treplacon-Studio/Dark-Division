using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class MovementController : MonoBehaviourPunCallbacks
{
    [Header("Movement properties")]
    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("Slows moving back speed as a fracture of normal speed.")]
    private float moveBackwardFactor = 0.8f;

    [SerializeField] [Range(0f, 1f)] [Tooltip("Slows moving left and right speed as a fracture of normal speed.")]
    private float moveSideFactor = 0.8f;

    [SerializeField] [Tooltip("Reduces shaking when stepping on non flat terrain.")]
    private float antiBumpFactor = 0.75f;

    [SerializeField] [Tooltip("Lets the character to change movement direction while in air.")]
    private bool airControl = true;

    [SerializeField] [Tooltip("Defines how fast character will slide down from slope.")]
    private float slopeSpeed;

    private CharacterController _characterController;
    private bool _isGrounded = true;
    private float _speed;
    private Vector2 _input = Vector2.zero;
    private Vector3 _moveDirection = Vector3.zero;
    private Vector3 _moveVelocity = Vector3.zero;
    private CollisionFlags _collisionFlags;
    private RaycastHit _slopeHit;


    //Animations
    [Header("Animations")] [SerializeField] [Tooltip("Controller for animations.")]
    private PlayerAnimationController pac;

    [SerializeField] [Tooltip("Central spine bone.")]
    private Transform centralSpineBone;


    //Controls
    [Header("Controls")] [Space(10)] [SerializeField] [Tooltip("Input action for looking around with mouse.")]
    private InputAction lookAround;


    //Walking
    [Header("Walking properties")] [SerializeField] [Tooltip("Character speed while walking.")]
    private float walkingSpeed = 5f;


    //Running
    [Header("Running properties")] [SerializeField] [Tooltip("Character speed while running.")]
    private float runningSpeed = 8f;


    //Jumping
    [Header("Jumping properties")] [SerializeField] [Tooltip("Horizontal jump speed.")]
    private float xJumpingSpeed = 2f;

    [SerializeField] [Tooltip("Vertical jump speed.")]
    private float yJumpingSpeed = 7f;

    [SerializeField] [Tooltip("Delay that needs to pass until next jump can be done.")]
    private float frameDelayBetweenJumps = 1f;

    private float _frameDelayCounter;
    private bool _isLanding;


    //Falling
    private bool _isFalling;


    //Crouching
    [Header("Crouching")] [SerializeField] [Tooltip("Offset between standard and crouch position.")]
    private float crouchHeight = 1f;

    [SerializeField] [Tooltip("Speed of moving between standing/crouching.")]
    private float crouchSpeed = 3f;

    [SerializeField] [Tooltip("If player if crouching.")]
    private bool isCrouching;


    //Aiming
    [Header("Aiming")] [SerializeField] [Tooltip("Aim mode.")]
    private Aiming.AimMode aimMode;


    //Camera
    [FormerlySerializedAs("mouseLook")]
    [Header("Camera")]
    [SerializeField]
    [Tooltip("Component that let character to freely look around.")]
    private MouseLookAround mouseLookAround;


    //Physics
    [Header("Physics")] [SerializeField] [Tooltip("Gravity that is applied on character.")]
    private float gravity = 20f;


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        if (photonView.IsMine is false)
            return;
        
        mouseLookAround.Init(transform);

        _speed = walkingSpeed;
        _frameDelayCounter = frameDelayBetweenJumps;
    }

    private void Update()
    {
        if (photonView.IsMine is false)
            return;

        //Handle freely look around
        mouseLookAround.LookRotation();

        //Handle some movement actions
        HandleActions();

        //Handle player movement
        MovePosition();

        //Check landing
        CheckLanding();

        //Handles animations
        HandleAnimations();
    }

    private void CheckLanding()
    {
        if (_isGrounded)
            return;

        // Array of directions to cast rays (down, slightly angled)
        Vector3[] directions =
        {
            Vector3.down
        };

        _isLanding = false;

        foreach (var direction in directions)
        {
            if (Physics.Raycast(transform.position, direction, out var hit))
            {
                if (hit.distance <= 1.0f)
                {
                    _isLanding = true;
                    break;
                }
            }
        }
    }

    private void HandleActions()
    {
        if (_isGrounded)
        {
            if (_isFalling) _isFalling = false;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (!isCrouching)
                    _speed = runningSpeed;
            }
            else if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
            {
                isCrouching = !isCrouching;
            }

            if (!Input.GetKey(KeyCode.LeftShift))
                _speed = isCrouching ? crouchSpeed : walkingSpeed;
        }
        else
        {
            if (!_isFalling) _isFalling = true;
            if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
                isCrouching = !isCrouching;
        }
    }

    private void MovePosition()
    {
        _input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (_input.y < 0) _input.y *= moveBackwardFactor;
        if (_input.x != 0) _input.x *= moveSideFactor;
        if (_input.sqrMagnitude > 1) _input.Normalize();

        if (_isGrounded)
        {
            _moveDirection = transform.forward * _input.y + transform.right * _input.x;

            Physics.SphereCast(transform.position + _characterController.center, _characterController.radius,
                Vector3.down, out var rh,
                _characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);

            _moveDirection = Vector3.ProjectOnPlane(_moveDirection, rh.normal).normalized;
            _moveDirection.y = -antiBumpFactor;

            if (OnSteepSlope())
                SteepSlopeMovement();

            _moveVelocity = _moveDirection * _speed;

            if (ActionsManager.Instance.Jumping.JumpTriggered() && ActionsManager.Instance.Jumping.CanJump())
            {
                _moveVelocity.y = yJumpingSpeed;
                _moveVelocity += _moveDirection * xJumpingSpeed;
                _frameDelayCounter = 0;

                if (isCrouching)
                    isCrouching = true;
            }
        }
        else
        {
            //Control direction while jumping
            if (airControl)
            {
                _moveVelocity.x = _input.x * _speed;
                _moveVelocity.z = _input.y * _speed;
                _moveVelocity = transform.TransformDirection(_moveVelocity);
            }
        }

        _moveVelocity.y -= gravity * Time.deltaTime;

        //Slowly decreasing velocity if stopping
        if (_input.sqrMagnitude <= 0.4f && _isGrounded)
        {
            _moveVelocity.x *= 0.4f;
            _moveVelocity.z *= 0.4f;
            if (_moveVelocity.sqrMagnitude < 0.01f)
            {
                _moveVelocity.x = 0;
                _moveVelocity.z = 0;
            }
        }

        //Move character
        _collisionFlags = _characterController.Move(_moveVelocity * Time.deltaTime);
        _isGrounded = (_collisionFlags & CollisionFlags.Below) != 0;
    }

    private void HandleAnimations()
    {
        if (ActionsManager.Instance?.Sprinting is not null)
            ActionsManager.Instance.Sprinting.Run();
        
        if (ActionsManager.Instance?.Jumping is not null)
            ActionsManager.Instance.Jumping.Run(_isLanding, _isGrounded);
        
        if (ActionsManager.Instance?.Shooting is not null)
            ActionsManager.Instance.Shooting.Run();
        
        if (ActionsManager.Instance?.Aiming is not null)
            ActionsManager.Instance.Aiming.Run(aimMode);
        
        if (ActionsManager.Instance?.Walking is not null)
            ActionsManager.Instance.Walking.Run(_input, _isGrounded);
        
        if (ActionsManager.Instance?.Reloading is not null)
            ActionsManager.Instance.Reloading.Run();
        
        if (ActionsManager.Instance?.Inspecting is not null)
            ActionsManager.Instance.Inspecting.Run();
        
        if (ActionsManager.Instance?.Switching is not null)
            ActionsManager.Instance.Switching.Run();
    }

    private bool OnSteepSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit,
                _characterController.height / 2f + 1f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            if (_slopeHit.collider.CompareTag("Stairs"))
                return false;

            var slopeAngle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            if (slopeAngle > _characterController.slopeLimit)
                return true;
        }

        return false;
    }

    private void SteepSlopeMovement()
    {
        var slopeAngle = Vector3.Angle(Vector3.up, _slopeHit.normal);
        var slopeDirection = Vector3.ProjectOnPlane(Vector3.down, _slopeHit.normal).normalized;

        var slopeAngleFactor =
            Mathf.Clamp01((slopeAngle - _characterController.slopeLimit) / (90f - _characterController.slopeLimit));
        var slideSpeed = _speed * slopeSpeed * slopeAngleFactor;

        _moveDirection = slopeDirection * slideSpeed;
        _moveDirection.y = -antiBumpFactor;
    }
}