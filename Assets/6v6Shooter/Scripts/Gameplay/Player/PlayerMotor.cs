using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerMotor : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    float speed = 3f;

    // Create input actions for basic movements
    [SerializeField] private InputAction forward;
    [SerializeField] private InputAction backward;
    [SerializeField] private InputAction left;
    [SerializeField] private InputAction right;
    [SerializeField] private InputAction jumpAction;
    [SerializeField] private InputAction sprintAction;
    [SerializeField] private InputAction aimAction;
    [SerializeField] private InputAction lookAction;
    [SerializeField] private InputAction shootAction;

    [SerializeField]
    private Transform spineBone;
    private float currentSpineRotationX = 23f;

    public float lookSensitivity = Gamepad.current != null ? 10f : 100f;
    [SerializeField]

    // Jumping variables
    float jumpForce = 2.0f;
    public bool isGrounded;
    public Vector3 jump;

    // Camera rotation
    public float camYUpwardRotation;
    public float camYDownwardRotation;

    public GameObject[] fpsHandsGameObject;
    public GameObject[] soldierGameObject;

    [SerializeField]
    GameObject fpsCamera;

    Vector3 velocity = Vector3.zero;
    Vector3 rotation = Vector3.zero;
    float cameraUpAndDownRotation = 0f;
    float currentCamRotation = 0;

    public PhotonView pv;

    Rigidbody body;
    public Animator anim;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        jump = new Vector3(0.0f, 1.5f, 0.0f);
        pv = GetComponent<PhotonView>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (photonView.IsMine)
        {
            foreach (GameObject gameObject in fpsHandsGameObject)
            {
                gameObject.SetActive(true);
            }
            foreach (GameObject gameObject in soldierGameObject)
            {
                gameObject.SetActive(false);
            }

            // Enable input actions
            forward.Enable();
            backward.Enable();
            left.Enable();
            right.Enable();
            jumpAction.Enable();
            sprintAction.Enable();
            aimAction.Enable();
            lookAction.Enable();
            shootAction.Enable();

            // Register callbacks for aim action
            aimAction.performed += ctx => OnAim();
            aimAction.canceled += ctx => OnStopAim();
            shootAction.performed += ctx => OnShoot();
            //shootAction.canceled += ctx => OnShoot();
        }
        else
        {
            foreach (GameObject gameObject in fpsHandsGameObject)
            {
                gameObject.SetActive(false);
            }
            foreach (GameObject gameObject in soldierGameObject)
            {
                gameObject.SetActive(true);
            }
        }
    }

    void Update()
    {
        // Only process input for the local player
        if (!pv.IsMine)
        {
            return;
        }

        Vector2 movementInput = new Vector2(right.ReadValue<float>() - left.ReadValue<float>(), forward.ReadValue<float>() - backward.ReadValue<float>());
        Vector3 horizontalMovement = transform.right * movementInput.x;
        Vector3 verticalMovement = transform.forward * movementInput.y;

        // Setting walk animations
        anim.SetFloat("Horizontal", movementInput.x);
        anim.SetFloat("Vertical", movementInput.y);

        // Sprinting
        bool isSprinting = sprintAction.IsPressed();
        anim.SetBool("isSprinting", isSprinting);
        speed = isSprinting ? 5f : 3f;

        // Final movement velocity vector
        Vector3 movementVelocity = (horizontalMovement + verticalMovement).normalized * speed;

        // Apply movement
        Move(movementVelocity);

        // Calculate rotation as a 3D Vector for turning around
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        float yRot = lookInput.x * lookSensitivity * Time.deltaTime;
        Vector3 rotVector = new Vector3(0, yRot, 0);

        // Apply rotation
        Rotate(rotVector);

        // Calculate look up and down camera rotation
        float camUpAndDownRotation = lookInput.y * lookSensitivity * Time.deltaTime;

        // Apply rotation
        RotateCamera(camUpAndDownRotation);

        // Rotate the head bone based on mouse x input
        RotateHeadBone(yRot);

        // Jumping
        if (jumpAction.triggered && isGrounded)
        {
            body.AddForce(jump * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnAim()
    {
        anim.SetTrigger("aimSight");
        lookSensitivity = 2f;
        speed = 1f;
    }

    void OnStopAim()
    {
        anim.SetTrigger("unaimSight");
        lookSensitivity = Gamepad.current != null ? 10f : 100f;
        speed = 3f;
    }

    void OnShoot()
    {
        Debug.Log("PEW PEW");
    }

    // Method to rotate the head bone
    void RotateHeadBone(float yRot)
    {
        if (spineBone != null)
        {
            spineBone.Rotate(Vector3.right * yRot * lookSensitivity);
        }
    }

    // Runs per physics interaction
    void FixedUpdate()
    {
        // Only process movement and rotation for the local player
        if (!pv.IsMine)
        {
            return;
        }

        if (velocity != Vector3.zero)
        {
            body.MovePosition(body.position + velocity * Time.fixedDeltaTime);
        }

        body.MoveRotation(body.rotation * Quaternion.Euler(rotation));

        if (fpsCamera != null)
        {
            currentCamRotation -= cameraUpAndDownRotation;
            currentCamRotation = Mathf.Clamp(currentCamRotation, camYUpwardRotation, camYDownwardRotation);
            fpsCamera.transform.localEulerAngles = new Vector3(currentCamRotation, 0, 0);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (body == null)
        {
            Debug.LogError("Rigidbody component is not assigned.");
            return;
        }

        if (stream.IsWriting)
        {
            Vector3 pos = body.position;
            Quaternion rot = body.rotation;
            Vector3 vel = body.velocity;
            Vector3 rotVel = body.angularVelocity;

            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
            stream.Serialize(ref vel);
            stream.Serialize(ref rotVel);
        }
        else
        {
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            Vector3 vel = Vector3.zero;
            Vector3 rotVel = Vector3.zero;

            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
            stream.Serialize(ref vel);
            stream.Serialize(ref rotVel);

            body.position = pos;
            body.rotation = rot;
            body.velocity = vel;
            body.angularVelocity = rotVel;
        }
    }

    void Move(Vector3 movementVelocity)
    {
        velocity = movementVelocity;
    }

    void Rotate(Vector3 rotationVector)
    {
        rotation = rotationVector;
    }

    void RotateCamera(float camUpAndDownRotation)
    {
        cameraUpAndDownRotation = camUpAndDownRotation;
    }

    // Checking if we're on the ground
    void OnCollisionStay()
    {
        isGrounded = true;
    }
}