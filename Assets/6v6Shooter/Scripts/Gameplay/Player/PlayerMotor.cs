using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MovementController : MonoBehaviourPunCallbacks {
    
    [SerializeField]
    float speed = 3f;

    public float lookSensitivity = 10f;

    //Jumping variables
    float jumpForce = 2.0f;
    public bool isGrounded;
    public Vector3 jump;

    [SerializeField]
    GameObject fpsCamera;

    Vector3 velocity = Vector3.zero;
    Vector3 rotation = Vector3.zero;
    float cameraUpAndDownRotation = 0f;
    float currentCamRotation = 0;

    Rigidbody body;
    //Animator anim;

    void Start() {
        body = GetComponent<Rigidbody>();
        jump = new Vector3(0.0f, 1.5f, 0.0f);
        //anim = GetComponent<Animator>();
    }

    void Update() {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 horizontalMovement = transform.right * x;
        Vector3 verticalMovement = transform.forward * z;

        //Setting walk animations
        /*anim.SetFloat("Horizontal", x);
		anim.SetFloat("Vertical", z);

        //Sprinting
        anim.SetBool("isRunning", Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Sprint"));
        if (anim.GetBool("isRunning") && !anim.GetBool("isAiming")) {
            speed = 4f;            
        }
        else {
            speed = 2f;
        }*/

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

        /* if (Input.GetButtonDown("Fire2") && photonView.IsMine) {
            anim.SetTrigger("aimSight");
            lookSensitivity = 1.5f;
            speed = 1f;

            openCrosshairUI.gameObject.SetActive(false);
            closedCrosshairUI.gameObject.SetActive(true);
        }
        else if (Input.GetButtonUp("Fire2")  && photonView.IsMine) {
            anim.SetTrigger("dontAimSight");
            lookSensitivity = 10f;
            speed = 3f;

            openCrosshairUI.gameObject.SetActive(true);
            closedCrosshairUI.gameObject.SetActive(false);
        } */
    }

    //Runs per physics interation
    void FixedUpdate() {
        if (velocity != Vector3.zero) {
            body.MovePosition(body.position + velocity * Time.fixedDeltaTime);
        }

        body.MoveRotation(body.rotation * Quaternion.Euler(rotation));

        if (fpsCamera != null) {
            currentCamRotation -= cameraUpAndDownRotation;
            currentCamRotation = Mathf.Clamp(currentCamRotation, -85, 85);
            fpsCamera.transform.localEulerAngles = new Vector3(currentCamRotation, 0, 0);
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.IsWriting) {
			Vector3 pos = body.position;
			Quaternion rot = body.rotation;
			Vector3 vel = body.velocity;
			Vector3 rotVel = body.angularVelocity;

			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			//stream.Serialize(ref input);
			stream.Serialize(ref vel);
			stream.Serialize(ref rotVel);
		}
		else {
			Vector3 pos = Vector3.zero;
			Quaternion rot = Quaternion.identity;
			Vector3 vel = Vector3.zero;
			Vector3 rotVel = Vector3.zero;

			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			//stream.Serialize(ref input);
			stream.Serialize(ref vel);
			stream.Serialize(ref rotVel);

			body.position = pos;
			body.rotation = rot;
			body.velocity = vel;
			body.angularVelocity = rotVel;
		}
	}
}