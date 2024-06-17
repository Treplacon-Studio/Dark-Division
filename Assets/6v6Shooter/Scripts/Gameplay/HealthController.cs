using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using _6v6Shooter.Scripts.Gameplay.Player;

public class HealthController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Image healthBar;

    public float health;
    public float startHealth = 100;

    public bool targetDummy;

    // Add references to the player components and ragdoll bones
    private Animator animator;
    private MovementController movementController;
    private  BoneRotator boneRotator;
    private List<Rigidbody> ragdollBodies = new List<Rigidbody>();

    void Start() {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;

        // Initialize references
        animator = GetComponentInChildren<Animator>();
        movementController = GetComponent<MovementController>();
        boneRotator = GetComponentInChildren<BoneRotator>();

        // Find all rigidbody components in the character's children (ragdoll bones)
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>()) {
            if (rb.gameObject != this.gameObject) { // Exclude the main rigidbody if any
                ragdollBodies.Add(rb);
                rb.isKinematic = true; // Disable physics at start
            }
        }
    }

    [PunRPC]
    public void TakeDamage(float damage) {
        health -= damage;
        healthBar.fillAmount = health / startHealth;    

        if (health <= 0f)
            Die();
    }

    void Die() {
        if (photonView.IsMine) {
            if (targetDummy) {
                photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
            } else {
                EnableRagdoll();
                StartCoroutine(Respawn());
            }
        }
    }

    void EnableRagdoll() {
        if (animator != null) animator.enabled = false;
        if (movementController != null) movementController.enabled = false;
        if (boneRotator != null) boneRotator.enabled = false;

        // Disable any other movement scripts or input handlers
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts) {
            if (script != this 
            && script != photonView 
            && script != movementController 
            && script != boneRotator)
            {
                script.enabled = false;
            }
        }

        // Enable physics on all ragdoll bones
        foreach (Rigidbody rb in ragdollBodies) {
            rb.isKinematic = false;
        }
    }

    void DisableRagdoll() {
        if (animator != null) animator.enabled = true;
        if (movementController != null) movementController.enabled = true;
        if (boneRotator != null) boneRotator.enabled = true;

        // Enable any other movement scripts or input handlers
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts) {
            if (script != this 
            && script != photonView 
            && script != movementController 
            && script != boneRotator) {
                script.enabled = true;
            }
        }

        // Disable physics on all ragdoll bones
        foreach (Rigidbody rb in ragdollBodies) {
            rb.isKinematic = true;
        }
    }

    IEnumerator Respawn() {
        GameObject respawnText = GameObject.Find("RespawnText");

        float respawnTime = 8.0f;
        while (respawnTime > 0.0f) {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            respawnText.GetComponent<Text>().text = "You are killed. Respawning at: " + respawnTime.ToString(".00");
        }

        respawnText.GetComponent<Text>().text = "";

        int randomPoint = Random.Range(-20, 20);
        transform.position = new Vector3(randomPoint, 0, randomPoint);
        
        DisableRagdoll();

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RegainHealth() {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
    }
}
