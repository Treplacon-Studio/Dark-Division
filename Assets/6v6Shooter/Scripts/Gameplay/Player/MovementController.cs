using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls all physical variables of movement and operations on it.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class MovementController : MonoBehaviourPunCallbacks
{
    #region Properties
    
    [SerializeField] 
    private PlayerNetworkController pnc;
    
    [SerializeField] [Range(0f, 1f)] [Tooltip("Slows moving back speed as a fracture of normal speed.")]
    private float fMoveBackwardFactor = 0.8f;

    [SerializeField] [Range(0f, 1f)] [Tooltip("Slows moving left and right speed as a fracture of normal speed.")]
    private float fMoveSideFactor = 0.8f;
    
    [SerializeField] [Tooltip("Anti bump for sliding.")]
    private float fAntiBumpFactorSliding = 0.75f;

    [SerializeField] [Tooltip("Lets the character to change movement direction while in air.")]
    private bool bAirControl = true;

    [SerializeField] [Tooltip("Defines how fast character will slide down from slope.")]
    private float fSlopeSpeed;

    private CharacterController _characterController;
    private bool _bIsGrounded = true;
    private float _fSpeed;
    private Vector2 _v2Input = Vector2.zero;
    private Vector3 _v2MoveDirection = Vector3.zero;
    private Vector3 _v2MoveVelocity = Vector3.zero;
    private CollisionFlags _collisionFlags;
    private RaycastHit _slopeHit;
    
    //Animations
    [Header("Animations")] 
    
    [SerializeField] [Tooltip("Controller for animations.")]
    private PlayerAnimationController pac;

    [SerializeField] [Tooltip("Central spine bone.")]
    private Transform tCentralSpineBone;
    
    //Controls
    [Header("Controls")] [Space(10)] 
    
    [SerializeField] [Tooltip("Input action for looking around with mouse.")]
    private InputAction lookAround;
    
    //Walking
    [Header("Walking properties")] [SerializeField] 
    
    [Tooltip("Character speed while walking.")]
    private float fWalkingSpeed = 5f;
    
    //Running
    [Header("Running properties")] 
    
    [SerializeField] [Tooltip("Character speed while running.")]
    private float fRunningSpeed = 8f;
    
    //Sliding
    private Vector2 _v2InputDump;
    
    //Jumping
    [Header("Jumping properties")] 
    
    [SerializeField] [Tooltip("Horizontal jump speed.")]
    private float fXJumpingSpeed = 2f;

    [SerializeField] [Tooltip("Vertical jump speed.")]
    private float fYJumpingSpeed = 7f;

    [SerializeField] [Tooltip("Delay that needs to pass until next jump can be done.")]
    private float fFrameDelayBetweenJumps = 1f;
    
    private float _fFrameDelayCounter;
    private bool _bIsLanding;
    
    //Falling
    private bool _bIsFalling;

    //Camera
    [Header("Camera")]
    
    [SerializeField] [Tooltip("Component that let character to freely look around.")]
    private MouseLookAround mouseLookAround;
    
    //Gravity
    [Header("Physics")] 
    
    [SerializeField] [Tooltip("Gravity that is applied on character.")]
    private float fGravity = 20f;

    #endregion Properties
    
    #region Basic Methods
    
    private void Awake()
    {
        if (photonView.IsMine)
        {
            _characterController = GetComponent<CharacterController>();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            mouseLookAround.Init(transform);

            _fSpeed = fWalkingSpeed;
            _fFrameDelayCounter = fFrameDelayBetweenJumps;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            //Handle freely look around
            mouseLookAround.LookRotation();

            //Handle some movement actions
            HandleOtherOperations();

            //Handle player movement
            MovePosition();

            //Check landing
            CheckLanding();

            //Handles animations
            HandleActions();
        }        
    }
    
    #endregion Basic Methods

    #region Other Methods
    
    /// <summary>
    /// Updates player-in-air variables.
    /// </summary>
    private void CheckLanding()
    {
        if (_bIsGrounded)
            return;

        // Array of directions to cast rays (down, slightly angled)
        Vector3[] v3Directions =
        {
            Vector3.down
        };

        _bIsLanding = false;

        foreach (var v3Direction in v3Directions)
        {
            if (Physics.Raycast(transform.position, v3Direction, out var hit))
            {
                if (hit.distance <= 2.0f)
                {
                    _bIsLanding = true;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Handles operations that are not movement but need physical operations.
    /// </summary>
    private void HandleOtherOperations()
    {
        var fMultiplier = ActionsManager.GetInstance(pnc.GetInstanceID()).Crouching.GetSlideSpeedMultiplier();
        if (_bIsGrounded)
        {
            if (_bIsFalling) 
                _bIsFalling = false;
            
            _fSpeed = Input.GetKey(KeyCode.LeftShift) ? fRunningSpeed : fWalkingSpeed;
            if (ActionsManager.GetInstance(pnc.GetInstanceID()).Crouching.IsSliding())
                _fSpeed = fRunningSpeed * fMultiplier;
        }
        else
        {
            if (!_bIsFalling) 
                _bIsFalling = true;
        }
    }

    /// <summary>
    /// Handles player movement and input operations.
    /// </summary>
    private void MovePosition()
    {
        
        _v2Input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        //If sliding, input should be blocked
        if (ActionsManager.GetInstance(pnc.GetInstanceID()).Crouching.IsSliding())
            _v2Input = _v2InputDump;
        else _v2InputDump = _v2Input;
            
        if (_v2Input.y < 0) _v2Input.y *= fMoveBackwardFactor;
        if (_v2Input.x != 0) _v2Input.x *= fMoveSideFactor;
        if (_v2Input.sqrMagnitude > 1) _v2Input.Normalize();
        
        if (_bIsGrounded)
        {
            _v2MoveDirection = transform.forward * _v2Input.y + transform.right * _v2Input.x;

            Physics.SphereCast(transform.position + _characterController.center, _characterController.radius,
                Vector3.down, out var rh,
                _characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);

            if (OnSteepSlope())
                SteepSlopeMovement();

            _v2MoveVelocity = _v2MoveDirection * _fSpeed;

            if (ActionsManager.GetInstance(pnc.GetInstanceID()).Jumping.JumpTriggered() && ActionsManager.GetInstance(pnc.GetInstanceID()).Jumping.CanJump())
            {
                //Cannot jump when sliding
                if (!ActionsManager.GetInstance(pnc.GetInstanceID()).Crouching.IsSliding())
                {
                    _v2MoveVelocity.y = fYJumpingSpeed;
                    _v2MoveVelocity += _v2MoveDirection * fXJumpingSpeed;
                    _fFrameDelayCounter = 0;
                }
            }
        }
        else
        {
            //Control direction while jumping
            if (bAirControl)
            {
                _v2MoveVelocity.x = _v2Input.x * _fSpeed;
                _v2MoveVelocity.z = _v2Input.y * _fSpeed;
                _v2MoveVelocity = transform.TransformDirection(_v2MoveVelocity);
            }
        }

        _v2MoveVelocity.y -= fGravity * Time.deltaTime;

        //Slowly decreasing velocity if stopping
        if (_v2Input.sqrMagnitude <= 0.4f && _bIsGrounded)
        {
            _v2MoveVelocity.x *= 0.4f;
            _v2MoveVelocity.z *= 0.4f;
            if (_v2MoveVelocity.sqrMagnitude < 0.01f)
            {
                _v2MoveVelocity.x = 0;
                _v2MoveVelocity.z = 0;
            }
        }

        //Move character
        _collisionFlags = _characterController.Move(_v2MoveVelocity * Time.deltaTime);
        _bIsGrounded = (_collisionFlags & CollisionFlags.Below) != 0;
    }

    /// <summary>
    /// Runs all actions' handle methods at once.
    /// </summary>
    private void HandleActions()
    {
        if (photonView.IsMine)
        {
            if (ActionsManager.GetInstance(pnc.GetInstanceID())?.Sprinting is not null)
                ActionsManager.GetInstance(pnc.GetInstanceID()).Sprinting.Run();
            
            if (ActionsManager.GetInstance(pnc.GetInstanceID())?.Jumping is not null)
                ActionsManager.GetInstance(pnc.GetInstanceID()).Jumping.Run();
            
            if (ActionsManager.GetInstance(pnc.GetInstanceID())?.Shooting is not null)
                ActionsManager.GetInstance(pnc.GetInstanceID()).Shooting.Run();
            
            if (ActionsManager.GetInstance(pnc.GetInstanceID())?.Aiming is not null)
                ActionsManager.GetInstance(pnc.GetInstanceID()).Aiming.Run();
            
            if (ActionsManager.GetInstance(pnc.GetInstanceID())?.Walking is not null)
                ActionsManager.GetInstance(pnc.GetInstanceID()).Walking.Run();
            
            if (ActionsManager.GetInstance(pnc.GetInstanceID())?.Reloading is not null)
                ActionsManager.GetInstance(pnc.GetInstanceID()).Reloading.Run();
            
            if (ActionsManager.GetInstance(pnc.GetInstanceID())?.Inspecting is not null)
                ActionsManager.GetInstance(pnc.GetInstanceID()).Inspecting.Run();
            
            if (ActionsManager.GetInstance(pnc.GetInstanceID())?.Switching is not null)
                ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.Run();
            
            if (ActionsManager.GetInstance(pnc.GetInstanceID())?.Crouching is not null)
                ActionsManager.GetInstance(pnc.GetInstanceID()).Crouching.Run();
        }
    }
    
    /// <summary>
    /// Checks if player is standing on slope.
    /// </summary>
    /// <returns>Information if player is standing on slope.</returns>
    private bool OnSteepSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit,
                _characterController.height / 2f + 1f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            if (_slopeHit.collider.CompareTag("Stairs"))
                return false;

            var fSlopeAngle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            if (fSlopeAngle > _characterController.slopeLimit)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Defines slope movement.
    /// </summary>
    private void SteepSlopeMovement()
    {
        var fSlopeAngle = Vector3.Angle(Vector3.up, _slopeHit.normal);
        var fSlopeDirection = Vector3.ProjectOnPlane(Vector3.down, _slopeHit.normal).normalized;

        var fSlopeAngleFactor =
            Mathf.Clamp01((fSlopeAngle - _characterController.slopeLimit) / (90f - _characterController.slopeLimit));
        var fSlideSpeed = _fSpeed * fSlopeSpeed * fSlopeAngleFactor;

        _v2MoveDirection = fSlopeDirection * fSlideSpeed;
    }
    
    #endregion Other Methods
    
    #region Accessors
    
    /// <summary>
    /// Getter method.
    /// </summary>
    /// <returns>Current input value.</returns>
    public Vector2 GetInput()
    {
        return _v2Input;
    }

    /// <summary>
    /// Getter method.
    /// </summary>
    /// <returns>Information if player is now landing.</returns>
    public bool IsLanding()
    {
        return _bIsLanding;
    }
    
    /// <summary>
    /// Getter method.
    /// </summary>
    /// <returns>Information if player now stands on the ground.</returns>
    public bool IsGrounded()
    {
        return _bIsGrounded;
    }
    
    #endregion Accessors
}