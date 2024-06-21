using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class PlayerSetup : MonoBehaviourPun
{
    private CinemachineVirtualCamera playerCamera;

    void Start()
    {
        playerCamera = GetComponentInChildren<CinemachineVirtualCamera>(true);

        if (photonView.IsMine)
        {
            // Enable the camera for the local player
            if (playerCamera != null)
            {
                playerCamera.enabled = true;
            }
            else
            {
                Debug.LogError("CinemachineVirtualCamera not found in player prefab.");
            }
        }
        else
        {
            // Disable the camera for other players
            if (playerCamera != null)
            {
                playerCamera.enabled = false;
            }
        }
    }
}
