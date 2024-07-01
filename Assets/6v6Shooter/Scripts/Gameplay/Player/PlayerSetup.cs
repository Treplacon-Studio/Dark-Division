using UnityEngine;
using Photon.Pun;
using Cinemachine;
using UnityEngine.Serialization;

public class PlayerSetup : MonoBehaviourPun
{
    [SerializeField] private PlayerNetworkController pnc;
    
    private CinemachineVirtualCamera playerCamera;
    
    public GameObject[] fpsHandsGameObject;
    public GameObject[] soldierGameObject;

    
    void Start()
    {
        AutoLoadAllWeapons();
        
        playerCamera = GetComponentInChildren<CinemachineVirtualCamera>(true);

        if (photonView.IsMine)
        {
            if (playerCamera != null)
                playerCamera.enabled = true;
            else
                Debug.LogError("CinemachineVirtualCamera not found in player prefab.");

            //Activate FPS hands, Deactivate Soldier
            foreach (GameObject gameObject in fpsHandsGameObject)
                gameObject.SetActive(true);

            foreach (GameObject gameObject in soldierGameObject)
                gameObject.SetActive(false);

            transform.GetComponent<MovementController>().enabled = true;
            transform.GetComponentInChildren<PlayerAnimationController>().enabled = true;
        }
        else
        {
            if (playerCamera != null)
                playerCamera.enabled = false;

            foreach (GameObject gameObject in fpsHandsGameObject)
                gameObject.SetActive(false);

            foreach (GameObject gameObject in soldierGameObject)
                gameObject.SetActive(true);

            transform.GetComponent<MovementController>().enabled = false;
            transform.GetComponentInChildren<PlayerAnimationController>().enabled = false;
        }
    }
    
    //Sets weapons to the player equipment
    //Before calling this method for every weapon's Weapon component set its attachments with 
    //ApplyAttachments[WeaponCategory] method from Weapon class
    private void SetupWeapons(GameObject[] weapons, int[,] attachments)
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.SetNewEquipment(weapons, attachments);
    }

    
    // ---------------
    //  FOR DEBUGGING
    // ---------------
    public GameObject[] allGuns;
    
    //Method for debugging, automatically loads all weapons to equipment
    //Here can easily test attachments
    private void AutoLoadAllWeapons()
    {
        int[,] attachments =
        {
            {0, -1, -1, -1, -1},
            {0, -1, -1, -1, -1},
            {0, -1, -1, -1, -1},
        };
        
        SetupWeapons(allGuns, attachments);
    }
}