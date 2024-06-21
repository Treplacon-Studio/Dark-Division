using UnityEngine;
using Photon.Pun;
using static SharedPlayerSettings;

public class PlayerMovementController : MonoBehaviourPun, IPunObservable
{
    private PlayerMotor playerMotor;
    private PlayerControls playerControls;
    private CharacterController characterController;

    public Vector2 inputMovement;
    public Vector2 inputView;

    private Vector2 newCameraRotation;
    private Vector3 newCharacterRotation;

    [Header("References")]
    public Transform cameraHolder;

    [Header("Settings")]
    public PlayerSettingsModel playerSettings;
    public float viewClampYMin = -70;
    public float viewClampYMax = 80;

    private void Awake()
    {
        playerMotor = GetComponent<PlayerMotor>();
        characterController = GetComponent<CharacterController>();

        if (photonView.IsMine)
        {
            playerControls = new PlayerControls();
            playerControls.PlayerBase.Movement.performed += e => inputMovement = e.ReadValue<Vector2>();
            playerControls.PlayerBase.Look.performed += e => inputView = e.ReadValue<Vector2>();

            playerControls.Enable();

            newCameraRotation = cameraHolder.localRotation.eulerAngles;
            newCharacterRotation = transform.localRotation.eulerAngles;
        }
        else
        {
            // Disable components not needed for remote players
            characterController.enabled = false;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            CalculateView();
            CalculateMovement();
        }
        else
        {
            // Optionally: Smoothly update remote player position and rotation
            SmoothSyncMovement();
        }
    }

    private void CalculateView()
    {
        newCharacterRotation.y += playerSettings.ViewXSensitivity * (playerSettings.ViewXInverted ? -inputView.x : inputView.x) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(newCharacterRotation); // Note: Changed to localRotation

        newCameraRotation.x += playerSettings.ViewYSensitivity * (playerSettings.ViewYInverted ? inputView.y : -inputView.y) * Time.deltaTime;
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, viewClampYMin, viewClampYMax);

        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);
    }

    private void CalculateMovement()
    {
        var verticalSpeed = playerSettings.WalkingForwardSpeed * inputMovement.y * Time.deltaTime;
        var horizontalSpeed = playerSettings.WalkingStrafeSpeed * inputMovement.x * Time.deltaTime;

        var newMovementSpeed = new Vector3(horizontalSpeed, 0, verticalSpeed);
        newMovementSpeed = transform.TransformDirection(newMovementSpeed);

        characterController.Move(newMovementSpeed);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(cameraHolder.localRotation);
        }
        else
        {
            // Network player, receive data
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            cameraHolder.localRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    private void SmoothSyncMovement()
    {
        // Optionally: Add smoothing for remote players' movement and rotation
    }
}
