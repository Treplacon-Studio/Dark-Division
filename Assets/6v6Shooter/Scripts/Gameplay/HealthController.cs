using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class HealthController : MonoBehaviourPunCallbacks
{
    [SerializeField] private Image healthBar;
    public float health;
    public float startHealth = 100;
    public bool targetDummy;

    [SerializeField] PlayerSetup playerSetup;

    public GameObject fpsHandsGameObject;
    public GameObject fpsHandsOutfit;
    public GameObject soldierGameObject;
    public GameObject soldierOutfit;

    public PublicMatchSpawnManager spawnManager;
    public DeathUIManager deathUIManager;

    void Start()
    {
        playerSetup = GetComponent<PlayerSetup>();
        spawnManager = FindObjectOfType<PublicMatchSpawnManager>();
        deathUIManager = FindObjectOfType<DeathUIManager>();
        if (spawnManager == null)
        {
            Debug.LogError("PublicMatchSpawnManager not found in the scene.");
            return;
        }

        health = startHealth;
        healthBar.fillAmount = health / startHealth;
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.fillAmount = health / startHealth;

        if (health <= 0f)
            Die();
    }

    void Die()
    {
        if (photonView.IsMine)
        {
            if (targetDummy)
            {
                photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
            }
            else
            {
                Team? team = TeamManager.GetTeam(PhotonNetwork.LocalPlayer);
                TeamDeathmatchManager.instance.GetComponent<PhotonView>().RPC("AddPointForTeam", RpcTarget.AllBuffered, team);
                playerSetup.GetComponent<PhotonView>().RPC("EnableRagdollRPC", RpcTarget.All);
                playerSetup.SwitchToRagdollCamera();
                deathUIManager.ShowDeathUI(4f); // Show death UI with 4-second countdown
                StartCoroutine(Respawn());
            }
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(4f);

        string team = GetTeam();
        Transform spawnPoint = spawnManager.GetRandomSpawnPoint(team);

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }
        else
        {
            Debug.LogError("No valid spawn point found for the team.");
        }

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
        playerSetup.GetComponent<PhotonView>().RPC("DisableRagdollRPC", RpcTarget.All);
        playerSetup.SwitchToMainCamera();
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
    }

    private string GetTeam()
    {
        Team? team = TeamManager.GetTeam(PhotonNetwork.LocalPlayer);
        return team.ToString();
    }
}
