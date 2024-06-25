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
        [SerializeField]
        Image healthBar;

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

        void Start()
        {
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
                    Debug.Log("Enabling Ragdoll");
                    EnableRagdoll();
                    SwitchToRagdollCamera();
                    StartCoroutine(Respawn());
                }
            }
        }

        void EnableRagdoll()
        {
            Debug.Log("Ragdoll Enabled");
            if (animator != null) animator.enabled = false;
            if (movementController != null) movementController.enabled = false;
            if (boneRotator != null) boneRotator.enabled = false;

            MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script != this
                && script != photonView
                && script != movementController
                && script != boneRotator)
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
            if (animator != null) animator.enabled = true;
            if (movementController != null) movementController.enabled = true;
            if (boneRotator != null) boneRotator.enabled = true;

            MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script != this
                && script != photonView
                && script != movementController
                && script != boneRotator)
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

        IEnumerator Respawn()
        {
            GameObject respawnText = GameObject.Find("RespawnText");

            float respawnTime = 8.0f;
            while (respawnTime > 0.0f)
            {
                yield return new WaitForSeconds(1.0f);
                respawnTime -= 1.0f;
                transform.GetComponent<PlayerMotor>().enabled = false;
                respawnText.GetComponent<Text>().text = "You are killed. Respawning at: " + respawnTime.ToString("0.00");
            }

            respawnText.GetComponent<Text>().text = "";

            int randomPoint = Random.Range(-20, 20);
            transform.position = new Vector3(randomPoint, 0, randomPoint);
            transform.GetComponent<PlayerMotor>().enabled = true;

            photonView.RPC("RegainHealth", RpcTarget.AllBuffered);

            // Switch back to the main camera after respawn
            SwitchToMainCamera();
        }

        [PunRPC]
        public void RegainHealth()
        {
            health = startHealth;
            healthBar.fillAmount = health / startHealth;
        }
    }
}
