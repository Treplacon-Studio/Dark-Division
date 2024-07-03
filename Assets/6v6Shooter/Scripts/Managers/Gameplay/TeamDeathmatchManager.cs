using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

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


    void Awake() {
        if(instance == null) 
            instance = this;

        DisablePlayerMovementForAll();
        GameInPlay = false;
        BeginMatchCountdownScreen.SetActive(true);
    }

    void Start()
    {
        //When the master client joins in the game, we begin the countdown process to start the match
        if (PhotonNetwork.IsMasterClient)
            _syncCountdown = BeginMatchCountdown;
    }

    void Update()
    {
        if (CheckIfGameShouldEnd())
        {
            TimeRemaining = 0.0f;
            ShowEndGameCanvas();
            if (EndMatchCountdown >= 0)
                EndMatchCountdown -= Time.deltaTime;
                photonView.RPC("SyncMatchEndCountdown", RpcTarget.AllBuffered, BeginMatchCountdown);

            if (EndMatchCountdown <= 0)
                photonView.RPC("BackToLobby", RpcTarget.All);
        }

        if (GameInPlay is true)
        {
            if (TimeRemaining > 0)
                TimeRemaining -= Time.deltaTime;
        }
        else
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
                {
                    GameInPlay = true;
                    BeginMatchCountdownScreen.SetActive(false);
                    EnablePlayerMovementForAll();
                }
            }
            else
            {
                if (BeginMatchCountdown >= 0)
                {
                    BeginMatchCountdown = _syncCountdown;
                }
                else
                {
                    GameInPlay = true;
                    BeginMatchCountdownScreen.SetActive(false);
                    EnablePlayerMovementForAll();
                }
            }

            UpdateCountdownUI();
        }

    }

    public bool CheckIfGameShouldEnd()
    {
        if (TimeRemaining < 1 || TeamBlueScore >= 75 || TeamRedScore >= 75)
            return true;

        return false;
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
            Transform playerHUDTransform = player.transform.Find("PlayerHUD");

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
        foreach (GameObject player in players)
        {
            MovementController movement = player.GetComponent<MovementController>();
            Transform playerHUDTransform = player.transform.Find("PlayerHUD");
            if (movement != null)
                movement.enabled = true;

            if (playerHUDTransform != null)
                playerHUDTransform.gameObject.SetActive(true);

            Transform boneTransform = player.transform.Find("Character 01/rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003");

            if (boneTransform != null)
            {
                BoneRotator boneRotator = boneTransform.GetComponent<BoneRotator>();
                if (boneRotator != null)
                {
                    boneRotator.enabled = true;
                }
                else
                {
                    Debug.LogWarning("BoneRotator component not found on " + boneTransform.name);
                }
            }
            else
                Debug.LogWarning("Bone transform path not found for " + player.name);
        }
    }

    [PunRPC]
    public void SyncMatchBeginCountdown(float countdown)
    {
        _syncCountdown = countdown;
    }

    [PunRPC]
    public void SyncMatchEndCountdown(float countdown)
    {
        _syncCountdown = countdown;
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
        {
            GameInPlay = false;
        }
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

        // StartCoroutine(EndGameCountdown());
    }

    // private IEnumerator EndGameCountdown()
    // {
    //     int countdown = Mathf.CeilToInt(countdownDuration);
    //     while (countdown > 0)
    //     {
    //         endGameCountdownTxt.text = countdown.ToString();
    //         yield return new WaitForSeconds(1.0f);
    //         countdown--;
    //     }

    //     photonView.RPC("BackToLobby", RpcTarget.All);
    // }

    [PunRPC]
    private void BackToLobby()
    {
        GameManager.instance.StartLoadingBar("S02_Lobby", true);
    }

    private void SetScoreboard() 
    {
        scoreTxtBlue.text = TeamBlueScore.ToString();
        scoreTxtRed.text = TeamRedScore.ToString();
    }
}
