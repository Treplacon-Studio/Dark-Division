using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

namespace _6v6Shooter.Scripts.Gameplay
{
    public class HealthController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Image healthBar;
        public float health;
        public float startHealth = 100;
        public bool targetDummy;

        private Animator animator;
        private MovementController movementController;
        private BoneRotator boneRotator;
        private List<Rigidbody> ragdollBodies = new List<Rigidbody>();

        // Add references to the Cinemachine cameras
        public CinemachineVirtualCamera mainCamera;
        public CinemachineVirtualCamera ragdollCamera;

        public GameObject fpsHandsGameObject;
        public GameObject fpsHandsOutfit;
        public GameObject soldierGameObject;
        public GameObject soldierOutfit;

        // Reference to the PublicMatchSpawnManager
        public PublicMatchSpawnManager spawnManager;

        void Start()
        {
            fpsHandsGameObject.SetActive(true);
            fpsHandsOutfit.SetActive(true);
            soldierGameObject.SetActive(false);
            soldierOutfit.SetActive(false);

            spawnManager = FindObjectOfType<PublicMatchSpawnManager>();
            if (spawnManager == null)
            {
                Debug.LogError("PublicMatchSpawnManager not found in the scene.");
                return;
            }

            health = startHealth;
            healthBar.fillAmount = health / startHealth;

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

        [PunRPC]
        public void TakeDamage(float damage)
        {
            health -= damage;
            healthBar.fillAmount = health / startHealth;

            if (health <= 0f)
            {
                Die();
            }
        }

        void Die()
        {
            Debug.Log("Die function triggered");

            if (photonView.IsMine)
            {
                if (targetDummy)
                {
                    photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
                }
                else
                {
                    Team? team = TeamManager.GetTeam(PhotonNetwork.LocalPlayer);
                    Debug.Log("AddPointForTeam HIT");
                    Debug.Log(PhotonNetwork.LocalPlayer);
                    Debug.Log(team);
                    TeamDeathmatchManager.instance.GetComponent<PhotonView>().RPC("AddPointForTeam", RpcTarget.AllBuffered, team);
                    Debug.Log("Enabling Ragdoll");
                    photonView.RPC("EnableRagdollRPC", RpcTarget.All);
                    SwitchToRagdollCamera();
                    StartCoroutine(Respawn());
                }
            }
        }

        IEnumerator Respawn()
        {
            yield return new WaitForSeconds(4f);

            // Get the team and find a random spawn point
            string team = GetTeam();
            Transform spawnPoint = spawnManager.GetRandomSpawnPoint(team);

            if (spawnPoint != null)
            {
                Debug.Log($"Respawning player at {spawnPoint.position} for team {team}");
                transform.position = spawnPoint.position;
                transform.rotation = spawnPoint.rotation;
            }
            else
            {
                Debug.LogError("No valid spawn point found for the team.");
                // Optionally handle this case, e.g., fallback spawn or error handling
            }

            // Reset health
            photonView.RPC("RegainHealth", RpcTarget.AllBuffered);

            photonView.RPC("DisableRagdollRPC", RpcTarget.All);
            SwitchToMainCamera();
        }

        [PunRPC]
        public void RegainHealth()
        {
            health = startHealth;
            healthBar.fillAmount = health / startHealth;
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
            if (photonView.IsMine){
                fpsHandsGameObject.SetActive(false);
                fpsHandsOutfit.SetActive(false);
                soldierGameObject.SetActive(true);
                soldierOutfit.SetActive(true);
            }
            

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
            if (photonView.IsMine) {
                fpsHandsGameObject.SetActive(true);
                fpsHandsOutfit.SetActive(true);
                soldierGameObject.SetActive(false);
                soldierOutfit.SetActive(false);
            }
            

            if (animator != null) animator.enabled = true;
            if (movementController != null) movementController.enabled = true;
            if (boneRotator != null) boneRotator.enabled = true;

            MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script != this && script != photonView && script != movementController && script != boneRotator)
                {
                    script.enabled = true;
                }
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

        void SwitchToRagdollCamera()
        {
            if (ragdollCamera != null && mainCamera != null)
            {
                ragdollCamera.enabled = true;
                mainCamera.enabled = false;
            }
        }

        void SwitchToMainCamera()
        {
            if (ragdollCamera != null && mainCamera != null)
            {
                ragdollCamera.enabled = false;
                mainCamera.enabled = true;
            }
        }

        private string GetTeam()
        {
            Team? team = TeamManager.GetTeam(PhotonNetwork.LocalPlayer);
            return team.ToString();
        }
    }
}
