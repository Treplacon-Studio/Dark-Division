using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun.UtilityScripts;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static LobbyManager instance;

    [SerializeField] Transform teamAPlayersContainer;
    [SerializeField] Transform teamBPlayersContainer;
    [SerializeField] GameObject playerListingPrefab;
    [SerializeField] Text gameModeNameText;
    [SerializeField] Text countdownDisplay;
    [SerializeField] int countdownTime = 10;

    void Start() {
        if (instance == null) instance = this;
        ConnectToPhotonServer();
    }

    private void ClearPlayerListings() {
        for (int i = teamAPlayersContainer.childCount - 1; i >= 0; i--)  {
            Destroy(teamAPlayersContainer.GetChild(i).gameObject);
        }

        for (int i = teamBPlayersContainer.childCount - 1; i >= 0; i--)  {
            Destroy(teamBPlayersContainer.GetChild(i).gameObject);
        }
    }

    private void ListPlayers() {
        foreach (Player player in PhotonNetwork.PlayerList) {
            GameObject tempListing = Instantiate(playerListingPrefab, teamAPlayersContainer);
            Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
            tempText.text = player.NickName;
        }
    }

    public void StartCountdown()
    {
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        int currentTime = countdownTime;
        while (currentTime > 0)
        {
            countdownDisplay.text = $"Time remaining: {currentTime.ToString()}";
            yield return new WaitForSeconds(1);
            currentTime--;
        }

        countdownDisplay.text = "Time remaining: 0";
        LoadMap();
    }

    public void LoadMap()
    {
        Debug.Log("LOADING MAP...");
        PhotonNetwork.LoadLevel("S03_PublicMatch");
    }

    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + " connected to Photon Server");
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom() 
    {
        Debug.Log($"{PhotonNetwork.NickName} joined to {PhotonNetwork.CurrentRoom.Name}");
        ListPlayers();
        GameManager.instance.CloseLoadingScreen();

        ClearPlayerListings();
        ListPlayers();
    }

    public override void OnLeftRoom() 
    {
        SceneManager.LoadScene("S00_MainMenu");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.Log($"{newPlayer.NickName} joined to {PhotonNetwork.CurrentRoom.Name} {PhotonNetwork.CurrentRoom.PlayerCount}");
        ClearPlayerListings();
        ListPlayers();
        JoinPlayerToATeam(newPlayer);
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer) {
        ClearPlayerListings();
        ListPlayers();
    }

    public void JoinPlayerToATeam(Player newPlayer)
    {
        int blueTeamCount = PhotonTeamsManager.Instance.GetTeamMembersCount("Blue");
        int redTeamCount = PhotonTeamsManager.Instance.GetTeamMembersCount("Red");
        Debug.Log($"{blueTeamCount} .. {redTeamCount}");
        if (PhotonNetwork.LocalPlayer.JoinTeam("Blue"))
        {
            Debug.Log($"{newPlayer.NickName} joined team blue.");
        }
        else
        {
            Debug.Log($"Failed to join team blue.");
        }

        ListPlayers();
    }
}
