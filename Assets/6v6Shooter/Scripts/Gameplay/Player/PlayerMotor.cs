using _6v6Shooter.Scripts.Gameplay;
using Photon.Pun;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    //Scripts that are on the root object
    public PlayerInputController inputController;
    public PlayerMovementController movementController;
    public HealthController healthController;
    public PlayerAnimationController animationController;
    public PlayerNetworkController networkController;
    public PhotonView photonView;

    //Scripts that are in a child object

    private void Awake() {
        inputController = GetComponent<PlayerInputController>();
        movementController = GetComponent<PlayerMovementController>();
        healthController = GetComponent<HealthController>();
        animationController = GetComponent<PlayerAnimationController>();
        networkController = GetComponent<PlayerNetworkController>();
        photonView = GetComponent<PhotonView>();
    }
}