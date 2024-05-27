using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMotor : MonoBehaviourPunCallbacks, IPunObservable {
    
    [SerializeField]
    float speed = 3f;

    public float lookSensitivity = 8f;

    //Jumping variables
    float jumpForce = 2.0f;
    public bool isGrounded;
    public Vector3 jump;

    //Camera rotation
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

    void Start() {
        body = GetComponent<Rigidbody>();
        jump = new Vector3(0.0f, 1.5f, 0.0f);
        pv = GetComponent<PhotonView>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (photonView.IsMine) {
            foreach (GameObject gameObject in fpsHandsGameObject) {
                gameObject.SetActive(true);
            }
            foreach (GameObject gameObject in soldierGameObject) {
                gameObject.SetActive(false);
            }
        }
        else {

            foreach (GameObject gameObject in fpsHandsGameObject) {
                gameObject.SetActive(false);
            }
            foreach (GameObject gameObject in soldierGameObject) {
                gameObject.SetActive(true);
            }
        }
    }

    void Update() {
        // Only process input for the local player
        if (!pv.IsMine) {
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 horizontalMovement = transform.right * x;
        Vector3 verticalMovement = transform.forward * z;

        //Setting walk animations
        anim.SetFloat("Horizontal", x);
		anim.SetFloat("Vertical", z);

        //Sprinting
        anim.SetBool("isSprinting", Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Sprint"));
        if (anim.GetBool("isSprinting")/*  && !anim.GetBool("isAiming") */) {
            speed = 5f;            
        }
        else {
            speed = 3f;
        }

        //Final movement velocity vector
        Vector3 movementVelocity = (horizontalMovement + verticalMovement).normalized * speed;

        //Apply movement
        Move(movementVelocity);

        //Calculate rotation as a 3D Vector for turning around
        float yRot = Input.GetAxis("Mouse X");
        Vector3 rotVector = new Vector3(0, yRot, 0) * lookSensitivity;

        //Apply rotation
        Rotate(rotVector);

        //Calculate look up and down camera rotation
        float _camUpDownRotation = Input.GetAxis("Mouse Y") * lookSensitivity;

        //Apply rotation
        RotateCamera(_camUpDownRotation);

        //Jumping
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded || Input.GetButtonDown("Jump") && isGrounded){
            body.AddForce(jump * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        //Aiming
        if (Input.GetButtonDown("Fire2") && photonView.IsMine) {
            anim.SetTrigger("aimSight");
            lookSensitivity = 2f;
            speed = 1f;
        }
        else if (Input.GetButtonUp("Fire2")  && photonView.IsMine) {
            anim.SetTrigger("unaimSight");
            lookSensitivity = 10f;
            speed = 3f;
        }
    }

    //Runs per physics interation
    void FixedUpdate() {
        // Only process movement and rotation for the local player
        if (!pv.IsMine) {
            return;
        }

        if (velocity != Vector3.zero) {
            body.MovePosition(body.position + velocity * Time.fixedDeltaTime);
        }

        body.MoveRotation(body.rotation * Quaternion.Euler(rotation));

        if (fpsCamera != null) {
            currentCamRotation -= cameraUpAndDownRotation;
            currentCamRotation = Mathf.Clamp(currentCamRotation, camYUpwardRotation, camYDownwardRotation);
            fpsCamera.transform.localEulerAngles = new Vector3(currentCamRotation, 0, 0);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (body == null) {
            Debug.LogError("Rigidbody component is not assigned.");
            return;
        }

        if (stream.IsWriting) {
            Vector3 pos = body.position;
            Quaternion rot = body.rotation;
            Vector3 vel = body.velocity;
            Vector3 rotVel = body.angularVelocity;

            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
            stream.Serialize(ref vel);
            stream.Serialize(ref rotVel);
        } else {
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

    void Move(Vector3 movementVelocity) {
        velocity = movementVelocity;
    }

    void Rotate(Vector3 rotationVector) {
        rotation = rotationVector;
    }

    void RotateCamera(float camUpAndDownRotation) {
        cameraUpAndDownRotation = camUpAndDownRotation;
    }

    //Checking if were on the ground
    void OnCollisionStay(){
        isGrounded = true;
    }
}
