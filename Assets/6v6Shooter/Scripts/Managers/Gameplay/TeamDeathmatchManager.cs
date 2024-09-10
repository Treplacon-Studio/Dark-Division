using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;

public class TeamDeathmatchManager : MonoBehaviourPunCallbacks
{
    public static TeamDeathmatchManager instance;

    [Header("TIME")]
    public float TimeRemaining = 600.0f;
    public float BeginMatchCountdown = 10f;
    public float EndMatchCountdown = 10f;
    private float _syncCountdown;

    [Header("SCORE")]
    public int TeamBlueScore = 0;
    public int TeamRedScore = 0;
    public TextMeshProUGUI scoreTxtRed;
    public TextMeshProUGUI scoreTxtBlue;

    [Header("UI")]
    public GameObject endGameCanvas;
    public GameObject BeginMatchCountdownScreen;
    public GameObject redWinsText;
    public GameObject blueWinsText;
    public GameObject drawText;
    public TextMeshProUGUI endGameCountdownTxt;
    public TextMeshProUGUI startGameCountdownTxt;

    public float countdownDuration = 10.0f;
    public bool GameInPlay;
    private List<GameObject> players = new List<GameObject>();

    [Header("KillFeed")]
    public GameObject killFeedElementPrefab;
    public Sprite M4A1Icon;
    public Sprite scarIcon;
    public Sprite tac45Icon;
    public Sprite vectorIcon;
    public Sprite velIcon;
    public Sprite stoegerIcon;
    public Sprite gauge320Icon;
    public Sprite fnFiveIcon;
    public Sprite dsr50Icon;
    public Sprite balistaIcon;

    void Awake() {
        if(instance == null) 
            instance = this;

        DisablePlayerMovementForAll();
        GameInPlay = false;
        BeginMatchCountdownScreen.SetActive(true);
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            _syncCountdown = BeginMatchCountdown;
    }

    void Update()
    {
        if (CheckIfGameShouldEnd())
            HandleEndGame();

        else if (GameInPlay)
            HandleInGame();

        else
            HandleGameStartCountdown();

        UpdateCountdownUI();
    }

   [PunRPC]
    public void ShareKillFeed(string victimName, string killerName, string weaponName)
    {
        Dictionary<string, Sprite> weaponIcons = new Dictionary<string, Sprite>
        {
            { "M4A1Sentinel254", M4A1Icon },
            { "ScarEnforcer557", scarIcon },
            { "Tac45", tac45Icon },
            { "VectorGhost500", vectorIcon },
            { "VelIronclad308", velIcon },
            { "Stoeger22", stoegerIcon },
            { "Gauge320", gauge320Icon },
            { "FnFive8", fnFiveIcon },
            { "Dsr50", dsr50Icon },
            { "BalistaVortex", balistaIcon },
        };

        foreach (GameObject player in players)
        {
            Transform playerHUDTransform = player.transform.Find("PF_Player_HUD");
            if (playerHUDTransform == null)
            {
                Debug.LogWarning("PlayerHUD not found for " + player.name);
                continue;
            }

            Transform hudCanvasTransform = playerHUDTransform.Find("PF_HUD_Canvas");
            if (hudCanvasTransform == null)
            {
                Debug.LogWarning("HUDCanvas not found for " + player.name);
                continue;
            }

            Transform killFeedTransform = hudCanvasTransform.Find("PF_HUD_KillFeed");
            if (killFeedTransform == null)
            {
                Debug.LogWarning("KillFeed container not found for " + player.name);
                continue;
            }

            GameObject killFeedElement = Instantiate(killFeedElementPrefab, killFeedTransform);

            // find/set prefab
            TextMeshProUGUI victimNameText = killFeedElement.transform.Find("VictimNameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI killerNameText = killFeedElement.transform.Find("KillerNameText").GetComponent<TextMeshProUGUI>();
            Transform gunIconTransform = killFeedElement.transform.Find("GunIcon");

            if (victimNameText != null) victimNameText.text = victimName;
            if (killerNameText != null) killerNameText.text = killerName;

            if (gunIconTransform != null)
            {
                Image gunIconImage = gunIconTransform.GetComponent<Image>();
                if (gunIconImage != null)
                {
                    if (weaponIcons.TryGetValue(weaponName, out Sprite weaponIcon))
                    {
                        gunIconImage.sprite = weaponIcon;
                        gunIconImage.enabled = true;
                    }
                }
                else
                {
                    Debug.LogWarning("Image component not found on GunIcon.");
                }
            }
            else
            {
                Debug.LogWarning("GunIcon not found in the kill feed element.");
            }

            killFeedElement.SetActive(true);
            StartCoroutine(RemoveFeed(killFeedElement));
        }
    }

     private IEnumerator RemoveFeed(GameObject killFeedElement)
    {
        CanvasGroup canvasGroup = killFeedElement.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("No CanvasGroup component found.");
            yield break;
        }

        yield return new WaitForSeconds(6f);

        float fadeDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        // Deactivate or destroy the element after it fades out
        canvasGroup.alpha = 0f;
        killFeedElement.SetActive(false);

        Destroy(killFeedElement);
    }


    void HandleEndGame()
    {
        TimeRemaining = 0.0f;
        DisablePlayerMovementForAll();
        ShowEndGameCanvas();

        if (EndMatchCountdown >= 0)
        {
            EndMatchCountdown -= Time.deltaTime;
            photonView.RPC("SyncMatchEndCountdown", RpcTarget.AllBuffered, EndMatchCountdown);
        }

        if (EndMatchCountdown <= 0)
            photonView.RPC("BackToLobby", RpcTarget.All);
    }

    void HandleInGame()
    {
        if (TimeRemaining > 0)
            TimeRemaining -= Time.deltaTime;
    }

    void HandleGameStartCountdown()
    {
        DisablePlayerMovementForAll();

        if (PhotonNetwork.IsMasterClient)
        {
            if (BeginMatchCountdown >= 0)
            {
                BeginMatchCountdown -= Time.deltaTime;
                photonView.RPC("SyncMatchBeginCountdown", RpcTarget.AllBuffered, BeginMatchCountdown);
            }
            else
                StartGame();
        }
        else
        {
            if (BeginMatchCountdown >= 0)
            {
                BeginMatchCountdown = _syncCountdown;
            }
            else
                StartGame();
        }
    }

    void StartGame()
    {
        GameInPlay = true;
        BeginMatchCountdownScreen.SetActive(false);
        EnablePlayerMovementForAll();
    }

    void UpdateCountdownUI()
    {
        startGameCountdownTxt.text = BeginMatchCountdown > 0 
            ? Mathf.Ceil(BeginMatchCountdown).ToString() 
            : "GO!";

        endGameCountdownTxt.text = EndMatchCountdown > 0 
            ? Mathf.Ceil(EndMatchCountdown).ToString() 
            : "0";
    }

    void DisablePlayerMovementForAll()
    {
        players.Clear();
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(player);
            MovementController movement = player.GetComponent<MovementController>();
            Transform playerHUDTransform = player.transform.Find("PF_Player_HUD");

            if (movement != null)
                movement.enabled = false;

            if (playerHUDTransform != null)
                playerHUDTransform.gameObject.SetActive(false);

            Transform boneTransform = player.transform.Find("Character 01/rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003");

            if (boneTransform != null)
            {
                BoneRotator boneRotator = boneTransform.GetComponent<BoneRotator>();
                if (boneRotator != null)
                    boneRotator.enabled = false;
                else 
                    Debug.LogWarning("BoneRotator component not found on " + boneTransform.name);
            }
            else
                Debug.LogWarning("Bone transform path not found for " + player.name);
        }
    }

    void EnablePlayerMovementForAll()
    {
        Debug.Log("Enabling");
        foreach (GameObject player in players)
        {
            MovementController movement = player.GetComponent<MovementController>();
            Transform playerHUDTransform = player.transform.Find("PF_Player_HUD");

            if (movement != null)
                movement.enabled = true;

            if (playerHUDTransform != null)
                playerHUDTransform.gameObject.SetActive(true);

            Transform boneTransform = player.transform.Find("Character 01/rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003");

            if (boneTransform != null)
            {
                BoneRotator boneRotator = boneTransform.GetComponent<BoneRotator>();
                if (boneRotator != null)
                    boneRotator.enabled = true;
                else
                    Debug.LogWarning("BoneRotator component not found on " + boneTransform.name);
            }
            else
                Debug.LogWarning("Bone transform path not found for " + player.name);
        }
    }

    public bool CheckIfGameShouldEnd()
    {
        if (TimeRemaining < 1 || TeamBlueScore >= 75 || TeamRedScore >= 75)
            return true;

        return false;
    }

    [PunRPC]
    public void SyncMatchBeginCountdown(float countdown)
    {
        _syncCountdown = countdown;
    }

    [PunRPC]
    public void SyncMatchEndCountdown(float countdown)
    {
        EndMatchCountdown = countdown;
    }

    [PunRPC]
    public void AddPointForTeam(Team team)
    {
        if (team == Team.Blue)
            TeamBlueScore++;
        else if (team == Team.Red)
            TeamRedScore++;
        else
            Debug.Log("Error validating team for this player so point will not count.");

        if (CheckIfGameShouldEnd())
            GameInPlay = false;
    }

    private void ShowEndGameCanvas()
    {
        endGameCanvas.SetActive(true);
        SetScoreboard();

        if (TeamBlueScore > TeamRedScore)
        {
            blueWinsText.SetActive(true);
            redWinsText.SetActive(false);
            drawText.SetActive(false);
        }
        else if (TeamRedScore > TeamBlueScore)
        {
            redWinsText.SetActive(true);
            blueWinsText.SetActive(false);
            drawText.SetActive(false);
        }
        else if (TeamRedScore == TeamBlueScore)
        {
            drawText.SetActive(true);
            redWinsText.SetActive(false);
            blueWinsText.SetActive(false);
        }

        GameInPlay = false;
    }

    [PunRPC]
    private void BackToLobby() => GameManager.instance.StartLoadingBar("S02_Lobby", true);

    private void SetScoreboard() 
    {
        scoreTxtBlue.text = TeamBlueScore.ToString();
        scoreTxtRed.text = TeamRedScore.ToString();
    }
}
