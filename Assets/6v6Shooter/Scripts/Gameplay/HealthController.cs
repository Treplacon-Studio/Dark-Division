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
        public GameObject soldierGameObject;

        // Reference to the PublicMatchSpawnManager
        public PublicMatchSpawnManager spawnManager;

        void Start()
        {
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

        void Die() {
            if (photonView.IsMine) {
                if (targetDummy is true)
                    photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
                else
                {
                    Team? team = TeamManager.GetTeam(PhotonNetwork.LocalPlayer);
                    Debug.Log("AddPointForTeam HIT");
                    Debug.Log(PhotonNetwork.LocalPlayer);
                    Debug.Log(team);
                    TeamDeathmatchManager.instance.GetComponent<PhotonView>().RPC("AddPointForTeam", RpcTarget.AllBuffered, team);
                    StartCoroutine(Respawn());
                }
            }
        }

        IEnumerator Respawn() {
            photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
            yield return new WaitForSeconds(1.0f);
        }

        [PunRPC]
        public void RegainHealth()
        {
            health = startHealth;
            healthBar.fillAmount = health / startHealth;
        }

        // Placeholder method to get the team, replace with your actual implementation
        private string GetTeam()
        {
            return "Red"; // or "Blue"
        }
    }
}
