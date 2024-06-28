using UnityEngine;
using TMPro; 
using Photon.Pun;

public class TeamDeathmatchManager : MonoBehaviourPunCallbacks
{
    public static TeamDeathmatchManager instance;

    [Header("TIME")]
    public float TimeRemaining = 600.0f;
    public TMP_Text TimeRemainingText;

    [Header("SCORE")]
    public int TeamAScore = 0;
    public int TeamBScore = 0;
    public TMP_Text TeamAAScoreText;
    public TMP_Text TeamBScoreText;

	void Awake() {
        if(instance == null) 
            instance = this;
    }

    void Start()
    {
        TeamAAScoreText.text = $"{TeamAScore}";
        TeamBScoreText.text = $"{TeamBScore}";
    }

    void Update()
    {
        if (CheckIfGameShouldEnd())
        {
            TimeRemaining = 0.0f;
            photonView.RPC("UpdateTimerDisplay", RpcTarget.AllBuffered);
            return;
        }
        
        if (TimeRemaining > 0)
        {
            TimeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();
            photonView.RPC("UpdateTimerDisplay", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(TimeRemaining / 60);
        int seconds = Mathf.FloorToInt(TimeRemaining % 60);

        TimeRemainingText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    bool CheckIfGameShouldEnd()
    {
        if (TimeRemaining < 1 || TeamAScore == 75 || TeamBScore == 75)
            return true;

        return false;
    }

    public void AddPointForTeam()
    {
        Debug.Log(PhotonNetwork.LocalPlayer);

        Team? team = TeamManager.GetTeam(PhotonNetwork.LocalPlayer);
        if (team == Team.Blue)
            TeamAScore++;
        else if (team == Team.Red)
            TeamBScore++;
        else
            Debug.Log("Error validating team for this player so point will not count.");


        photonView.RPC("UpdateTeamScores", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void UpdateTeamScores()
    {
        TeamAAScoreText.text = $"{TeamAScore}";
        TeamBScoreText.text = $"{TeamBScore}";
    }
}
