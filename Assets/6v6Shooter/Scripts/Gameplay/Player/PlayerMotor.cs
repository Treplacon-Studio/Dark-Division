using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMotor : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private float speed = 3f;


    public float lookSensitivity = 10f;

    // Jumping variables
    private float jumpForce = 2.0f;
    public bool isGrounded;
    public Vector3 jump;


    public GameObject[] fpsHandsGameObject;
    public GameObject[] soldierGameObject;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;

    private float cameraUpAndDownRotation = 0f;
    private float currentSpineRotationX = 22.9f;

    public PhotonView pv;
    private Rigidbody body;
    public Animator anim;

    // ref to the spine bone for following 'lookt'
    public Transform spineBone;

    private PlayerCameraController cameraController;

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

        if (anim != null)
        {
            anim.enabled = false;
        }

        cameraController = GetComponent<PlayerCameraController>();
    }

    void Update()
    {
        if (!pv.IsMine)
        {
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 horizontalMovement = transform.right * x;
        Vector3 verticalMovement = transform.forward * z;

        anim.SetFloat("Horizontal", x);
        anim.SetFloat("Vertical", z);

        anim.SetBool("isWalking", Input.GetKey(KeyCode.W));

        anim.SetBool("isSprinting", Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Sprint"));
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Sprint"))
        {
            speed = 5f;
        }
        else
        {
            speed = 3f;
        }

        Vector3 movementVelocity = (horizontalMovement + verticalMovement).normalized * speed;

        Move(movementVelocity);

        float yRot = Input.GetAxis("Mouse X");
        Vector3 rotVector = new Vector3(0, yRot, 0) * lookSensitivity;
        Rotate(rotVector);

        float camUpDownRotation = Input.GetAxis("Mouse Y") * lookSensitivity;
        cameraController.RotateCamera(camUpDownRotation);

        AdjustSpineRotation(camUpDownRotation);

        if ((Input.GetKeyDown(KeyCode.Space) && isGrounded) || (Input.GetButtonDown("Jump") && isGrounded))
        {
            body.AddForce(jump * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        if (Input.GetButtonDown("Fire2") && photonView.IsMine)
        {
            anim.SetTrigger("aimSight");
            lookSensitivity = 2f;
            speed = 1f;
        }
        else if (Input.GetButtonUp("Fire2") && photonView.IsMine)
        {
            anim.SetTrigger("unaimSight");
            lookSensitivity = 10f;
            speed = 3f;
        }
    }

    void FixedUpdate()
    {
        if (!pv.IsMine)
        {
            return;
        }

        if (velocity != Vector3.zero)
        {
            body.MovePosition(body.position + velocity * Time.fixedDeltaTime);
        }

        body.MoveRotation(body.rotation * Quaternion.Euler(rotation));
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

            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
            stream.Serialize(ref vel);
        }
        else
        {
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            Vector3 vel = Vector3.zero;

            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
            stream.Serialize(ref vel);

            body.position = pos;
            body.rotation = rot;
            body.velocity = vel;
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

    void AdjustSpineRotation(float camUpDownRotation)
    {
        if (spineBone != null)
        {
            currentSpineRotationX -= camUpDownRotation;
            currentSpineRotationX = Mathf.Clamp(currentSpineRotationX, -30f, 75f);
            spineBone.localRotation = Quaternion.Euler(currentSpineRotationX, 0, 0);
        }
    }

    void OnCollisionStay()
    {
        isGrounded = true;
    }
}
