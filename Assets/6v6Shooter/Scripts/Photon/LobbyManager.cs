using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static LobbyManager instance;

    [SerializeField] Transform playersContainer;
    [SerializeField] GameObject playerListingPrefab;
    [SerializeField] Text gameModeNameText;
    [SerializeField] Text countdownDisplay;
    [SerializeField] int countdownTime = 10;

    void Start() {
        if (instance == null) instance = this;
        ConnectToPhotonServer();
    }

    private void ClearPlayerListings() {
        for (int i = playersContainer.childCount - 1; i >= 0; i--)  {
            Destroy(playersContainer.GetChild(i).gameObject);
        }
    }

    private void ListPlayers() {
        foreach (Player player in PhotonNetwork.PlayerList) {
            GameObject tempListing = Instantiate(playerListingPrefab, playersContainer);
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
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer) {
        ClearPlayerListings();
        ListPlayers();
    }
}
