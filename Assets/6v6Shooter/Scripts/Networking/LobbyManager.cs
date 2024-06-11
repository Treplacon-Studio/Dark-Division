using UnityEngine;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Photon.Pun;

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
    [SerializeField] TMP_Text votesForMapOneDisplay;
    [SerializeField] TMP_Text votesForMapTwoDisplay;
    [SerializeField] TMP_Text votesForRandomMapDisplay;

    private PhotonView photonView;
    private int currentTime;

    private int votesForMapOne;
    private int votesForMapTwo;
    private int votesForRandomMap;

    private Dictionary<int, int> playerVotes = new Dictionary<int, int>();

    void Awake()
    {
        if (instance == null) instance = this;
        photonView = GetComponent<PhotonView>();

        if (photonView == null)
        {
            Debug.LogError("PhotonView component missing. Adding PhotonView component.");
            photonView = gameObject.AddComponent<PhotonView>();
        }

        Debug.Log($"PhotonView ID in Awake: {photonView.ViewID}");
    }

    void Start()
    {
        ConnectToPhotonServer();
    }

    private void ClearPlayerListings()
    {
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
                tempListing = Instantiate(teamAPlayerListingPrefab, teamAPlayersContainer);
            else if (team == Team.Red)
                tempListing = Instantiate(teamBPlayerListingPrefab, teamBPlayersContainer);
            else
                continue;

            TMP_Text tempText = tempListing.GetComponentInChildren<TMP_Text>();
            tempText.text = player.NickName;
        }
    }

    public void OnVoteForMapSelected(int mapNum)
    {
        int playerID = PhotonNetwork.LocalPlayer.ActorNumber;

        if (playerVotes.TryGetValue(playerID, out int previousVote))
        {
            // Subtract the previous vote
            switch (previousVote)
            {
                case 1:
                    votesForMapOne--;
                    break;
                case 2:
                    votesForMapTwo--;
                    break;
                case 3:
                    votesForRandomMap--;
                    break;
            }
        }

        // Add the new vote
        switch (mapNum)
        {
            case 1:
                votesForMapOne++;
                break;
            case 2:
                votesForMapTwo++;
                break;
            case 3:
                votesForRandomMap++;
                break;
        }

        // Update the player's vote
        playerVotes[playerID] = mapNum;

        // Update the votes display
        photonView.RPC("UpdateMapVotes", RpcTarget.AllBuffered);
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
        countdownDisplay.text = $"TIME BEFORE MATCH STARTS: {time.ToString()}";
    }

    [PunRPC]
    private void UpdateMapVotes()
    {
        if (photonView.ViewID != 0)
        {
            votesForMapOneDisplay.text = $"VOTES: {votesForMapOne}";
            votesForMapTwoDisplay.text = $"VOTES: {votesForMapTwo}";
            votesForRandomMapDisplay.text = $"VOTES: {votesForRandomMap}";
        }
        else
        {
            Debug.LogError("PhotonView ID is 0, cannot update map votes.");
        }
    }

    [PunRPC]
    private void UpdatePlayerCount()
    {
        Debug.Log($"UpdatePlayerCount called, PhotonView ID: {photonView.ViewID}, PlayerCount: {PhotonNetwork.CurrentRoom.PlayerCount}");
        if (photonView.ViewID != 0)
        {
            playerCountDisplay.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/12 PLAYERS";
        }
        else
        {
            Debug.LogError("PhotonView ID is 0, cannot update player count.");
        }
    }

    [PunRPC]
    public void LoadMap()
    {
        GameManager.instance.StartLoadingBar("S04_PublicMatch", true);
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
        TeamManager.AssignTeam(PhotonNetwork.LocalPlayer);
        Debug.Log($"OnJoinedRoom called, PhotonView ID: {photonView.ViewID}");
        StartCoroutine(UpdatePlayerCountAfterDelay());
        ListPlayers();
    }

    private IEnumerator UpdatePlayerCountAfterDelay()
    {
        yield return new WaitUntil(() => photonView.ViewID != 0);
        Debug.Log($"PhotonView ID after delay: {photonView.ViewID}");
        photonView.RPC("UpdatePlayerCount", RpcTarget.AllBuffered);
    }

    public override void OnLeftRoom()
    {
        GameManager.instance.StartLoadingBar("S01_MainMenu", false);
    }

    public void LeaveRoom()
    {
        TeamManager.LeaveTeam(PhotonNetwork.LocalPlayer);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"OnPlayerEnteredRoom called for {newPlayer.NickName}, PlayerCount: {PhotonNetwork.CurrentRoom.PlayerCount}");
        photonView.RPC("UpdatePlayerCount", RpcTarget.AllBuffered);
        ListPlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"OnPlayerLeftRoom called for {otherPlayer.NickName}, PlayerCount: {PhotonNetwork.CurrentRoom.PlayerCount}");
        photonView.RPC("UpdatePlayerCount", RpcTarget.AllBuffered);
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
        {
            stream.SendNext(currentTime);
            stream.SendNext(votesForMapOne);
            stream.SendNext(votesForMapTwo);
            stream.SendNext(votesForRandomMap);
        }
        else
        {
            currentTime = (int)stream.ReceiveNext();
            votesForMapOne = (int)stream.ReceiveNext();
            votesForMapTwo = (int)stream.ReceiveNext();
            votesForRandomMap = (int)stream.ReceiveNext();
        }
    }
}
