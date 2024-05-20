using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static LobbyManager instance;

    [SerializeField] Transform playersContainer;
    [SerializeField] GameObject playerListingPrefab;
    [SerializeField] Text roomNameDisplay;

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
