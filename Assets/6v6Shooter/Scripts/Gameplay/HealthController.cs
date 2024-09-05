using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthController : MonoBehaviourPunCallbacks
{
    [SerializeField] private Image healthBar;
    public float health;
    public float startHealth = 100;
    public bool targetDummy;

    [SerializeField] PlayerSetup playerSetup;
    [SerializeField] private Material hitEffect;
    [SerializeField] private float maskAmount = 1;
    public GameObject resCanvas;
    public TextMeshProUGUI resCount;

    public PublicMatchSpawnManager spawnManager;

    void Start()
    {
        playerSetup = GetComponent<PlayerSetup>();
        spawnManager = FindObjectOfType<PublicMatchSpawnManager>();
        if (spawnManager == null)
        {
            Debug.LogError("PublicMatchSpawnManager not found in the scene.");
            return;
        }

        health = startHealth;
        healthBar.fillAmount = health / startHealth;
    }

    private void Update()
    {
        // if(Input.GetKeyUp(KeyCode.Keypad4))
        // {
        //     TakeDamage(10);
        // }
    }

    [PunRPC]
    public void TakeDamage(float damage, int shooterViewID)
    {
        health -= damage;
        healthBar.fillAmount = health / startHealth;

        hitEffect.SetFloat("_MaskAmount", health / (100 / 1.4f));

        if (health <= 0f)
        {
            Die(shooterViewID);
        }
    }

    void Die(int shooterViewID)
    {
        if (photonView != null && photonView.IsMine)
        {
            if (targetDummy)
            {
                photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
            }
            else
            {
                string playerName = PhotonNetwork.LocalPlayer.NickName;
                Team? team = TeamManager.GetTeam(PhotonNetwork.LocalPlayer);
                TeamDeathmatchManager.instance.GetComponent<PhotonView>()
                    .RPC("AddPointForTeam", RpcTarget.AllBuffered, team);
                TeamDeathmatchManager.instance.GetComponent<PhotonView>()
                    .RPC("ShareKillFeed", RpcTarget.AllBuffered, playerName, PhotonView.Find(shooterViewID).Owner.NickName);

                // Disable player HUD, activate respawn canvas, and handle ragdoll
                playerSetup.DisableHUD();
                resCanvas.SetActive(true);
                playerSetup.GetComponent<PhotonView>().RPC("EnableRagdollRPC", RpcTarget.All);
                playerSetup.SwitchToRagdollCamera();

                // Handle respawn
                StartCoroutine(Respawn());
                StartCoroutine(CountdownTimer(4));
            }
        }
        else
        {
            if (targetDummy)
            {
                RegainHealth();
            }
        }

        // **Trigger Kill Notification for the Shooter**
        PhotonView shooterPhotonView = PhotonView.Find(shooterViewID);
        if (shooterPhotonView != null && shooterPhotonView.IsMine)
        {
            shooterPhotonView.GetComponent<PlayerKillNotification>().TriggerKillNotification();
        }
    }




    IEnumerator Respawn()
    {
        hitEffect.SetFloat("_MaskAmount", 1f);
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

        resCanvas.SetActive(false);
        playerSetup.EnableHUD();
        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
        playerSetup.GetComponent<PhotonView>().RPC("DisableRagdollRPC", RpcTarget.All);
        playerSetup.SwitchToMainCamera();
    }

    IEnumerator CountdownTimer(int seconds)
    {
        int remainingTime = seconds;
        while (remainingTime > 0)
        {
            resCount.text = remainingTime.ToString();
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }
        resCount.text = "0";
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;

        // Adjusting the mask amount to be between 0 and 1.4
        hitEffect.SetFloat("_MaskAmount", health / (100 / 1.4f));
    }


    private string GetTeam()
    {
        Team? team = TeamManager.GetTeam(PhotonNetwork.LocalPlayer);
        return team.ToString();
    }
}
