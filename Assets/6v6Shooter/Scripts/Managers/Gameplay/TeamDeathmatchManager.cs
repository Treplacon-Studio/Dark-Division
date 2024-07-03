using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Collections;

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

    public bool IsGameActive { get; private set; }

    private bool hasGameEnded = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        BeginMatchCountdownScreen.SetActive(true);
    }

    void Start()
    {
        IsGameActive = false;
        //When the master client joins in the game, we begin the countdown process to start the match
        if (PhotonNetwork.IsMasterClient)
            _syncCountdown = BeginMatchCountdown;
    }

    void Update()
    {
     if (!hasGameEnded && CheckIfGameShouldEnd())
        {
            TimeRemaining = 0.0f;
            photonView.RPC("SyncGameActiveState", RpcTarget.AllBuffered, false);
            ShowEndGameCanvas();
            hasGameEnded = true;
        }

        if (IsGameActive is true)
        {
            if (TimeRemaining > 0)
                TimeRemaining -= Time.deltaTime;
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (BeginMatchCountdown >= 0)
                {
                    BeginMatchCountdown -= Time.deltaTime;
                    photonView.RPC("SyncMatchBeginCountdown", RpcTarget.AllBuffered, BeginMatchCountdown);
                }
                else
                {
                    IsGameActive = true;
                    BeginMatchCountdownScreen.SetActive(false);
                    photonView.RPC("SyncGameActiveState", RpcTarget.AllBuffered, true);
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
                    BeginMatchCountdownScreen.SetActive(false);
                    IsGameActive = true;
                    Debug.Log("Is active " + IsGameActive);
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
            TeamBlueScore++;
        else if (team == Team.Red)
            TeamRedScore++;
        else
            Debug.Log("Error validating team for this player so point will not count.");

        if (!hasGameEnded && CheckIfGameShouldEnd())
        {
            ShowEndGameCanvas();
            hasGameEnded = true;
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

        IsGameActive = false;

        StartCoroutine(EndGameCountdown());
    }

    private IEnumerator EndGameCountdown()
    {
        int countdown = Mathf.CeilToInt(countdownDuration);
        while (countdown > 0)
        {
            endGameCountdownTxt.text = countdown.ToString();
            yield return new WaitForSeconds(1.0f);
            countdown--;
        }

        photonView.RPC("BackToLobby", RpcTarget.All);
    }

    [PunRPC]
    private void BackToLobby()
    {
        GameManager.instance.StartLoadingBar("S02_Lobby", true);
    }

    private void SetScoreboard() {
        scoreTxtBlue.text = TeamBlueScore.ToString();
        scoreTxtRed.text = TeamRedScore.ToString();
    }

    [PunRPC]
    private void SyncGameActiveState (bool isActive){
        IsGameActive = isActive;
    }
}
