using UnityEngine;
using TMPro; 
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class TeamDeathmatchManager : MonoBehaviourPunCallbacks
{
    public static TeamDeathmatchManager instance;

    [Header("TIME")]
    public float TimeRemaining = 600.0f;
    public float BeginMatchCountdown = 10f;
    private float _syncCountdown;

    [Header("SCORE")]
    public int TeamBlueScore = 0;
    public int TeamRedScore = 0;

    [Header("GAME ENTITIES/VARIABLES")]
    public GameObject BeginMatchCountdownScreen;
    public bool GameInPlay;

    [Header("UI")]
    public TextMeshProUGUI countdownText;


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
            return;
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
        if (TimeRemaining < 1 || TeamBlueScore == 75 || TeamRedScore == 75)
            return true;

        return false;
    }

    void UpdateCountdownUI()
    {
        countdownText.text = BeginMatchCountdown > 0 ? Mathf.Ceil(BeginMatchCountdown).ToString() : "GO!";
    }

    void DisablePlayerMovementForAll()
    {
        players.Clear();
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(player);
            MovementController movement = player.GetComponent<MovementController>();
            if (movement != null)
                movement.enabled = false;
        }
    }

    void EnablePlayerMovementForAll()
    {
        foreach (GameObject player in players)
        {
            MovementController movement = player.GetComponent<MovementController>();
            if (movement != null)
                movement.enabled = true;
        }
    }

    [PunRPC]
    public void SyncMatchBeginCountdown(float countdown)
    {
        _syncCountdown = countdown;
    }

    [PunRPC]
    public void AddPointForTeam(Team team)
    {
        if (team == Team.Blue)
            TeamRedScore++;
        else if (team == Team.Red)
            TeamBlueScore++;
        else
            Debug.Log("Error validating team for this player so point will not count.");
    }
}
