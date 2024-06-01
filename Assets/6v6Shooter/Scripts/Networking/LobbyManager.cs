using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static LobbyManager instance;

    [SerializeField] Transform teamAPlayersContainer;
    [SerializeField] Transform teamBPlayersContainer;
    [SerializeField] GameObject teamAPlayerListingPrefab;
    [SerializeField] GameObject teamBPlayerListingPrefab;
    [SerializeField] TMP_Text gameModeNameText;
    [SerializeField] TMP_Text countdownDisplay;
    [SerializeField] TMP_Text playerCountDisplay;

    private PhotonView photonView;
    private int currentTime;

    void Awake() {
        if (instance == null) instance = this;
        photonView = GetComponent<PhotonView>();
        Debug.Log($"PhotonView ID: {photonView.ViewID}");
    }

    void Start() {
        ConnectToPhotonServer();
    }

    private void ClearPlayerListings() {
        for (int i = teamAPlayersContainer.childCount - 1; i >= 0; i--) {
            Destroy(teamAPlayersContainer.GetChild(i).gameObject);
        }
        for (int i = teamBPlayersContainer.childCount - 1; i >= 0; i--) {
            Destroy(teamBPlayersContainer.GetChild(i).gameObject);
        }
    }

    public void ListPlayers() {
        ClearPlayerListings();
        foreach (Player player in PhotonNetwork.PlayerList) {
            Team? team = TeamManager.GetTeam(player);
            GameObject tempListing;
            if (team == Team.Blue)
                tempListing = Instantiate(teamAPlayerListingPrefab, teamAPlayersContainer);
            else if (team == Team.Red)
                tempListing = Instantiate(teamBPlayerListingPrefab, teamBPlayersContainer);
            else
                continue;
            TMP_Text tempText = tempListing.GetComponentInChildren<TMP_Text>();
            tempText.text = player.NickName;
        }
    }

    [PunRPC]
    public void StartTimer(int startTime) {
        currentTime = startTime;
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine() {
        while (currentTime > 0) {
            photonView.RPC("UpdateTimerDisplay", RpcTarget.All, currentTime);
            yield return new WaitForSeconds(1);
            currentTime--;
        }
        photonView.RPC("UpdateTimerDisplay", RpcTarget.All, currentTime);
        photonView.RPC("LoadMap", RpcTarget.All);
    }

    [PunRPC]
    private void UpdateTimerDisplay(int time) {
        countdownDisplay.text = $"TIME BEFORE MATCH STARTS: {time.ToString()}";
    }

    [PunRPC]
    private void UpdatePlayerCount() {
        if (photonView.ViewID != 0) {
            playerCountDisplay.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/12 PLAYERS";
        } else {
            Debug.LogError("PhotonView ID is 0, cannot update player count.");
        }
    }

    [PunRPC]
    public void LoadMap() {
        GameManager.instance.StartLoadingBar("S03_PublicMatch", true);
    }

    public void ConnectToPhotonServer() {
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        Debug.Log(PhotonNetwork.NickName + " connected to Photon Server");
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom() {
        TeamManager.AssignTeam(PhotonNetwork.LocalPlayer);
        photonView.RPC("UpdatePlayerCount", RpcTarget.All);
        ListPlayers();
    }

    public override void OnLeftRoom() {
        GameManager.instance.StartLoadingBar("S00_MainMenu", false);
    }

    public void LeaveRoom() {
        TeamManager.LeaveTeam(PhotonNetwork.LocalPlayer);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        photonView.RPC("UpdatePlayerCount", RpcTarget.All);
        ListPlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        photonView.RPC("UpdatePlayerCount", RpcTarget.All);
        ListPlayers();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (changedProps.ContainsKey("team")) {
            Team? team = TeamManager.GetTeam(targetPlayer);
            if (team.HasValue) {
                Debug.Log($"{targetPlayer.NickName} assigned to {team.Value} team.");
            } else {
                Debug.Log($"{targetPlayer.NickName} has not been assigned to any team.");
            }
            ListPlayers();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(currentTime);
            stream.SendNext(playerCountDisplay);
        } else {
            currentTime = (int)stream.ReceiveNext();
            playerCountDisplay = (TMP_Text)stream.ReceiveNext();
        }
    }

    public void OnDestroy() {
        if (photonView != null) {
            PhotonNetwork.RemoveRPCs(photonView);
        }
    }
}
