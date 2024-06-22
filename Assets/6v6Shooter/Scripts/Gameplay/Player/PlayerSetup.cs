using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class PlayerSetup : MonoBehaviourPun
{
    private CinemachineVirtualCamera playerCamera;

    public GameObject[] fpsHandsGameObject;
    public GameObject[] soldierGameObject;

    void Start()
    {
        playerCamera = GetComponentInChildren<CinemachineVirtualCamera>(true);

        if (photonView.IsMine)
        {
            if (playerCamera != null)
                playerCamera.enabled = true;
            else
                Debug.LogError("CinemachineVirtualCamera not found in player prefab.");

            //Activate FPS hands, Deactivate Soldier
            foreach (GameObject gameObject in fpsHandsGameObject) {
                gameObject.SetActive(true);
            }
            foreach (GameObject gameObject in soldierGameObject) {
                gameObject.SetActive(false);
            }

            transform.GetComponent<MovementController>().enabled = true;
            transform.GetComponentInChildren<PlayerAnimationController>().enabled = true;
        }
        else
        {
            if (playerCamera != null)
                playerCamera.enabled = false;

            foreach (GameObject gameObject in fpsHandsGameObject) {
                gameObject.SetActive(false);
            }
            foreach (GameObject gameObject in soldierGameObject) {
                gameObject.SetActive(true);
            }

            transform.GetComponent<MovementController>().enabled = false;
            transform.GetComponentInChildren<PlayerAnimationController>().enabled = false;
        }
    }
}
