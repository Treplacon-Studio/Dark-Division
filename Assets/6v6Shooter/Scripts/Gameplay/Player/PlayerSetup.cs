using System.Collections;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using UnityEngine.Serialization;
using System.Collections.Generic;

public class PlayerSetup : MonoBehaviourPun
{
    [SerializeField] private PlayerNetworkController pnc;
    
    private CinemachineVirtualCamera playerCamera;
    
    public GameObject[] fpsHandsGameObject;
    public GameObject[] soldierGameObject;
    public GameObject playerHUD;
    public GameObject[] MiniMapObjects;

    private Animator animator;
    private MovementController movementController;
    private BoneRotator boneRotator;
    private List<Rigidbody> ragdollBodies = new List<Rigidbody>();

    //Add references to the Cinemachine cameras
    public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera ragdollCamera;
    
    void Start()
    {
        if (photonView.IsMine)
            photonView.RPC("RPC_AutoLoadAllWeapons", RpcTarget.All);
        
        playerCamera = GetComponentInChildren<CinemachineVirtualCamera>(true);

        if (photonView.IsMine)
        {
            if (playerCamera != null)
                playerCamera.enabled = true;
            else
                Debug.LogError("CinemachineVirtualCamera not found in player prefab.");

            PlayerBodySetup(true);

            transform.GetComponent<MovementController>().enabled = true;
            transform.GetComponentInChildren<PlayerAnimationController>().enabled = true;

            foreach (var item in MiniMapObjects)
            {
                item.SetActive(true);
            }
            
            EnableHUD();
        }
        else
        {
            if (playerCamera != null)
                playerCamera.enabled = false;

            PlayerBodySetup(false);

            transform.GetComponent<MovementController>().enabled = false;
            transform.GetComponentInChildren<PlayerAnimationController>().enabled = false;

            foreach (var item in MiniMapObjects)
            {
                item.SetActive(false);
            }

            DisableHUD();
        }

        animator = GetComponentInChildren<Animator>();
        movementController = GetComponent<MovementController>();
        boneRotator = GetComponentInChildren<BoneRotator>();

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if (rb.gameObject != this.gameObject)
            {
                ragdollBodies.Add(rb);
                rb.isKinematic = true;
            }
        }

        // Ensure the ragdoll camera is disabled initially
        if (ragdollCamera != null) ragdollCamera.enabled = false;
    }

    public void PlayerBodySetup(bool firstPerson)
    {
        if (firstPerson)
        {
            //Activate FPS hands, Deactivate Soldier
            foreach (GameObject gameObject in fpsHandsGameObject)
                gameObject.SetActive(true);

            foreach (GameObject gameObject in soldierGameObject)
                gameObject.SetActive(false);
        }
        else
        {
            foreach (GameObject gameObject in fpsHandsGameObject)
                gameObject.SetActive(false);

            foreach (GameObject gameObject in soldierGameObject)
                gameObject.SetActive(true);
        }
    }
    
    // ---------------
    //  FOR DEBUGGING
    // ---------------
    public string[] allGuns;
    
    //Method for debugging, automatically loads all weapons to equipment
    //Here can easily test attachments

    [PunRPC]
    private void RPC_AutoLoadAllWeapons()
    {
        int[,] attachments =
        {
            {0, -1, -1, -1, -1, 0},
            {0, -1, -1, -1, -1, 0},
            {0, -1, -1, -1, -1, 0},
        };
        
        ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.SetNewEquipment(allGuns, attachments);
    }

    [PunRPC]
    void EnableRagdollRPC()
    {
        EnableRagdoll();
    }

    [PunRPC]
    void DisableRagdollRPC()
    {
        DisableRagdoll();
    }

    void EnableRagdoll()
    {
        if (photonView.IsMine)
            PlayerBodySetup(true);


        Debug.Log("Ragdoll Enabled");
        if (animator != null) animator.enabled = false;
        if (movementController != null) movementController.enabled = false;
        if (boneRotator != null) boneRotator.enabled = false;

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != this && script != photonView && script != movementController && script != boneRotator)
            {
                script.enabled = false;
            }
        }

        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
        }

        Rigidbody mainRb = GetComponent<Rigidbody>();
        if (mainRb != null)
        {
            mainRb.isKinematic = true;
        }
    }

    void DisableRagdoll()
    {
        if (photonView.IsMine)
            PlayerBodySetup(true);


        if (animator != null) animator.enabled = true;
        if (movementController != null) movementController.enabled = true;
        if (boneRotator != null) boneRotator.enabled = true;

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != this && script != photonView && script != movementController && script != boneRotator)
                script.enabled = true;
        }

        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = true;
        }

        Rigidbody mainRb = GetComponent<Rigidbody>();
        if (mainRb != null)
        {
            mainRb.isKinematic = false;
        }

        // Reset the animator
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
    }

    public void SwitchToRagdollCamera()
    {
        if (ragdollCamera != null && mainCamera != null)
        {
            ragdollCamera.enabled = true;
            mainCamera.enabled = false;
        }
    }

    public void SwitchToMainCamera()
    {
        if (ragdollCamera != null && mainCamera != null)
        {
            ragdollCamera.enabled = false;
            mainCamera.enabled = true;
        }
    }

    public void DisableHUD()
    {
        if (playerHUD != null)
            playerHUD.SetActive(false);
    }

    public void EnableHUD()
    {
        if (playerHUD != null)
            playerHUD.SetActive(true);
    }
}