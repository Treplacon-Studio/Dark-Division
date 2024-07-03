using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Collections;

public class TeamDeathmatchManager : MonoBehaviourPunCallbacks
{
    public static TeamDeathmatchManager instance;

    [Header("TIME")]
    public float TimeRemaining = 600.0f;

    [Header("SCORE")]
    public int TeamBlueScore = 0;
    public int TeamRedScore = 0;

    [Header("END GAME")]
    public GameObject endGameCanvas;
    public GameObject redWinsText;
    public GameObject blueWinsText;
    public GameObject drawText;
    public TextMeshProUGUI countdownText;
    public float countdownDuration = 10.0f;

    public bool IsGameOver { get; private set; }

    private bool hasGameEnded = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Update()
    {
        if (!hasGameEnded && CheckIfGameShouldEnd())
        {
            TimeRemaining = 0.0f;
            ShowEndGameCanvas();
            hasGameEnded = true;
        }

        if (TimeRemaining > 0)
            TimeRemaining -= Time.deltaTime;
    }

    public bool CheckIfGameShouldEnd()
    {
        if (TimeRemaining < 1 || TeamBlueScore >= 75 || TeamRedScore >= 75)
            return true;

        return false;
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

        // Check if game should end after adding a point
        if (!hasGameEnded && CheckIfGameShouldEnd())
        {
            ShowEndGameCanvas();
            hasGameEnded = true;
        }
    }

    private void ShowEndGameCanvas()
    {
        endGameCanvas.SetActive(true);

        if (TeamBlueScore > TeamRedScore)
        {
            blueWinsText.SetActive(true);
            redWinsText.SetActive(false);
        }
        else if (TeamRedScore > TeamBlueScore)
        {
            redWinsText.SetActive(true);
            blueWinsText.SetActive(false);
        }
        else if (TeamRedScore == TeamBlueScore)
        {
            drawText.SetActive(false);
        }
        else
        {
            redWinsText.SetActive(false);
            blueWinsText.SetActive(false);
        }

        IsGameOver = true;

        StartCoroutine(EndGameCountdown());
    }

    private IEnumerator EndGameCountdown()
    {
        int countdown = Mathf.CeilToInt(countdownDuration);
        while (countdown > 0)
        {
            countdownText.text = countdown.ToString() + "s";
            yield return new WaitForSeconds(1.0f); // Update every second
            countdown--;
        }

        //BackToLobby RPC method for all players
        photonView.RPC("BackToLobby", RpcTarget.All);
    }

    [PunRPC]
    private void BackToLobby()
    {
        GameManager.instance.StartLoadingBar("S02_Lobby", true);
    }
}
