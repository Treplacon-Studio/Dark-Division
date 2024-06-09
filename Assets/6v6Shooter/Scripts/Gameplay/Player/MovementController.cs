using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player
{
    public enum MovementState { Idle, Walking, Running, Jumping, Falling, Crouching};

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class MovementController : MonoBehaviour {
   
        //General
        [Header("General properties")]
    
        [SerializeField] [Tooltip("Player movement state on start.")] 
        private MovementState movementState = MovementState.Idle;
    
        [Header("Movement properties")]
        [SerializeField] [Range(0f, 1f)] [Tooltip("Slows moving back speed as a fracture of normal speed.")] 
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
    
    
        //Walking
        [Header("Walking properties")]
    
        [SerializeField] [Tooltip("Character speed while walking.")] 
        private float walkingSpeed = 5f;
    
        
        //Running
        [Header("Running properties")] 
        [SerializeField] [Tooltip("Character speed while running.")] 
        private float runningSpeed = 8f;
    
    
        //Jumping
        [Header("Jumping properties")]
        [SerializeField] [Tooltip("Horizontal jump speed.")] 
        private float xJumpingSpeed = 2f;
    
        [SerializeField] [Tooltip("Vertical jump speed.")] 
        private float yJumpingSpeed = 7f;
    
        [SerializeField] [Tooltip("Delay that needs to pass until next jump can be done.")] 
        private float frameDelayBetweenJumps = 1f;
    
        private float _frameDelayCounter;
    
    
        //Falling
        private bool _isFalling;
    
    
        //Crouching
        [Header("Crouching")]
    
        [SerializeField] [Tooltip("Offset between standard and crouch position.")] 
        private float crouchHeight = 1f;
    
        [SerializeField] [Tooltip("Speed of moving between standing/crouching.")] 
        private float crouchSpeed = 3f;
    
        [SerializeField] [Tooltip("If player if crouching.")] 
        private bool isCrouching;
    
        private enum CrouchState { Down, Up, Jump };
    
        private float _originalCameraLocalHeight;
        private float _originalCharacterControllerHeight;
        private float _originalCharacterControllerCenterY;

    
        //Camera
        [Header("Camera")]
    
        [SerializeField] [Tooltip("Component that let character to freely look around.")] 
        private MouseLook mouseLook;
    
        [SerializeField] private Camera _fpsCamera;
    
    
        //Physics
        [Header("Physics")]
        [SerializeField] [Tooltip("Gravity that is applied on character.")] 
        private float gravity = 20f;

    
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Start ()
        {
            mouseLook.Init(transform, _fpsCamera.transform);
        
            _speed = walkingSpeed;
            _frameDelayCounter = frameDelayBetweenJumps;
        
            //Some starting values for crouching revert
            _originalCameraLocalHeight = _fpsCamera.transform.localPosition.y;
            _originalCharacterControllerHeight = _characterController.height;
            _originalCharacterControllerCenterY = _characterController.center.y;
        }
	
        private void Update ()
        {
            //Handle freely look around
            mouseLook.LookRotation();
            
            //Handle some movement actions
            HandleActions();
        
            //Handle player movement
            MovePosition();
        
            //Indicates crouch
            UpdateCrouch();
        
            //Updates character movement state
            UpdateMoveState();
        }
    
        private void HandleActions()
        {
            if (_isGrounded)
            {
                if (_isFalling) _isFalling = false;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if(!isCrouching)
                        _speed = runningSpeed;
                }
                else if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
                {
                    isCrouching = !isCrouching;
                }
                if(!Input.GetKey(KeyCode.LeftShift))
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

                Physics.SphereCast(transform.position + _characterController.center, _characterController.radius, Vector3.down, out var rh,
                    _characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            
                _moveDirection = Vector3.ProjectOnPlane(_moveDirection, rh.normal).normalized;
                _moveDirection.y = -antiBumpFactor;
            
                if(OnSteepSlope())
                    SteepSlopeMovement();
            
                _moveVelocity = _moveDirection * _speed;
            
                if (!Input.GetButton("Jump"))
                    _frameDelayCounter++;
                else if (_frameDelayCounter >= frameDelayBetweenJumps)
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
     
        private void UpdateCrouch()
        {
            switch(isCrouching ? !_isGrounded ? CrouchState.Jump : CrouchState.Down : CrouchState.Up)
            {
                case (CrouchState.Down):
                    CrouchDown();
                    break;
                case (CrouchState.Up):
                    CrouchUp();
                    break;
                case (CrouchState.Jump):
                    CrouchJump();
                    break;
            }
        }
    
        private void UpdateMoveState()
        {
            if (!_isGrounded)
            {
                movementState = _characterController.velocity.y < 0 ? MovementState.Falling : MovementState.Jumping;
                return;
            }

            if (isCrouching)
            {
                movementState = MovementState.Crouching;
                return;
            }

            var moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (moveInput.sqrMagnitude == 0)
            {
                movementState = MovementState.Idle;
                return;
            }

            if (_speed == runningSpeed)
                movementState = MovementState.Running;
            else if (_speed == walkingSpeed)
                movementState = MovementState.Walking;
        }
    
    
        // ---------- UTILS ----------

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
            
            var slopeAngleFactor = Mathf.Clamp01((slopeAngle - _characterController.slopeLimit) / (90f - _characterController.slopeLimit));
            var slideSpeed = _speed * slopeSpeed * slopeAngleFactor;

            _moveDirection = slopeDirection * slideSpeed;
            _moveDirection.y = -antiBumpFactor;
        }
    
        private void CrouchUp()
        {
            if (_characterController.height != _originalCharacterControllerHeight)
                _characterController.height = Mathf.MoveTowards(_characterController.height, _originalCharacterControllerHeight, Time.deltaTime * crouchSpeed);
        
            if (_characterController.center.y != _originalCharacterControllerCenterY)
                _characterController.center = new Vector3(_characterController.center.x,
                    _originalCharacterControllerCenterY - (_originalCharacterControllerHeight - _characterController.height) * .5f,
                    _characterController.center.z);
        
            if (_fpsCamera.transform.localPosition.y != _originalCameraLocalHeight)
                _fpsCamera.transform.localPosition = new Vector3(_fpsCamera.transform.localPosition.x,
                    Mathf.MoveTowards(_fpsCamera.transform.localPosition.y, _originalCameraLocalHeight, Time.deltaTime * crouchSpeed),
                    _fpsCamera.transform.localPosition.z);
        }

        private void CrouchDown()
        {
            if (_characterController.height != crouchHeight)
                _characterController.height = Mathf.MoveTowards(_characterController.height, crouchHeight, Time.deltaTime * crouchSpeed);
        
            if (_characterController.center.y != _originalCharacterControllerCenterY - crouchHeight * 0.5f)
                _characterController.center = new Vector3(_characterController.center.x,
                    _originalCharacterControllerCenterY - (_originalCharacterControllerHeight - _characterController.height) * .5f,
                    _characterController.center.z);
        
            if (_fpsCamera.transform.localPosition.y != crouchHeight)
                _fpsCamera.transform.localPosition = new Vector3(_fpsCamera.transform.localPosition.x,
                    Mathf.MoveTowards(_fpsCamera.transform.localPosition.y, crouchHeight, Time.deltaTime * crouchSpeed),
                    _fpsCamera.transform.localPosition.z);
        }

        private void CrouchJump()
        {
            if (_characterController.height != crouchHeight)
            {
                var newHeight = Mathf.MoveTowards(_characterController.height, crouchHeight, Time.deltaTime * crouchSpeed);
                var heightDelta = newHeight - _characterController.height;
                _characterController.height = newHeight;
            
                _characterController.transform.position = new Vector3(_characterController.transform.position.x,
                    _characterController.transform.position.y - heightDelta,
                    _characterController.transform.position.z);
            }
        
            var characterControllerHeightDelta = _originalCharacterControllerHeight - _characterController.height;
            if (_characterController.center.y != _originalCharacterControllerCenterY - crouchHeight * .5f)
            {
                _characterController.center = new Vector3(_characterController.center.x,
                    _originalCharacterControllerCenterY - characterControllerHeightDelta * .5f,
                    _characterController.center.z);
            }
        
            if (_fpsCamera.transform.localPosition.y != crouchHeight)
                _fpsCamera.transform.localPosition = new Vector3(_fpsCamera.transform.localPosition.x,
                    Mathf.MoveTowards(_fpsCamera.transform.localPosition.y, crouchHeight, Time.deltaTime * crouchSpeed),
                    _fpsCamera.transform.localPosition.z);
        }
    }
}