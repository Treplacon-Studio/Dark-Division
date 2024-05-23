using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun.UtilityScripts;

public class LobbyManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static LobbyManager instance;

    [SerializeField] Transform teamAPlayersContainer;
    [SerializeField] Transform teamBPlayersContainer;
    [SerializeField] GameObject playerListingPrefab;
    [SerializeField] Text gameModeNameText;
    [SerializeField] Text countdownDisplay;
    [SerializeField] int countdownTime = 10;

    private PhotonView photonView;

    private int currentTime;

    void Start() {
        if (instance == null) instance = this;
        photonView = GetComponent<PhotonView>();
        ConnectToPhotonServer();
    }

    private void ClearPlayerListings() {
        for (int i = teamAPlayersContainer.childCount - 1; i >= 0; i--)  
        {
            Destroy(teamAPlayersContainer.GetChild(i).gameObject);
        }

        for (int i = teamBPlayersContainer.childCount - 1; i >= 0; i--)  
        {
            Destroy(teamBPlayersContainer.GetChild(i).gameObject);
        }
    }

    public void ListPlayers()
    {
        ClearPlayerListings();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Team? team = TeamManager.GetTeam(player);
            GameObject tempListing;

            if (team == Team.Blue)
                tempListing = Instantiate(playerListingPrefab, teamAPlayersContainer);
            else if (team == Team.Red)
                tempListing = Instantiate(playerListingPrefab, teamBPlayersContainer);
            else
                continue;

            Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
            tempText.text = player.NickName;
        }
    }


    [PunRPC]
    public void StartTimer(int startTime)
    {
        currentTime = startTime;
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        while (currentTime > 0)
        {
            photonView.RPC("UpdateTimerDisplay", RpcTarget.All, currentTime);
            yield return new WaitForSeconds(1);
            currentTime--;
        }

        photonView.RPC("UpdateTimerDisplay", RpcTarget.All, currentTime);
        photonView.RPC("LoadMap", RpcTarget.All);
    }

    [PunRPC]
    private void UpdateTimerDisplay(int time)
    {
        countdownDisplay.text = $"Time remaining: {time.ToString()}";
    }

    [PunRPC]
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
        TeamManager.AssignTeam(PhotonNetwork.LocalPlayer);
        ListPlayers();
        GameManager.instance.CloseLoadingScreen();
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
        ListPlayers();
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer) {
        ListPlayers();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        
        if (changedProps.ContainsKey("team"))
        {
            Team? team = TeamManager.GetTeam(targetPlayer);
            if (team.HasValue)
            {
                Debug.Log($"{targetPlayer.NickName} assigned to {team.Value} team.");
            }
            else
            {
                Debug.Log($"{targetPlayer.NickName} has not been assigned to any team.");
            }
            
            ListPlayers();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(currentTime);
        else
            currentTime = (int)stream.ReceiveNext();
    }
}
