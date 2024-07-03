using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] TMP_Text mapOneVoteDisplay;
    [SerializeField] TMP_Text mapTwoVoteDisplay;
    [SerializeField] TMP_Text randomMapVoteDisplay;
    [SerializeField] GameObject startButton; //Delete this eventually

    private PhotonView photonView;
    private int currentTime;

    private int mapOneVote;
    private int mapTwoVote;
    private int randomMapVote;
    private Sprite mapImage;

    public Sprite mapOne;
    public Sprite mapTwo;

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

        startButton.SetActive(false);
    }

    void Start()
    {
        ConnectToPhotonServer();
        mapImage = null;
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
            switch (previousVote)
            {
                case 1:
                    mapOneVote--;
                    break;
                case 2:
                    mapTwoVote--;
                    break;
                case 3:
                    randomMapVote--;
                    break;
            }
        }

        switch (mapNum)
        {
            case 1:
                mapOneVote++;
                break;
            case 2:
                mapTwoVote++;
                break;
            case 3:
                randomMapVote++;
                break;
        }

        playerVotes[playerID] = mapNum;

        photonView.RPC("UpdateMapVotes", RpcTarget.AllBuffered, mapOneVote, mapTwoVote, randomMapVote);
    }

    //[PunRPC]
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
    private void UpdateMapVotes(int mapOneVotes, int mapTwoVotes, int randomMapVotes)
    {
        mapOneVote = mapOneVotes;
        mapTwoVote = mapTwoVotes;
        randomMapVote = randomMapVotes;

        mapOneVoteDisplay.text = $"VOTES: {mapOneVote}";
        mapTwoVoteDisplay.text = $"VOTES: {mapTwoVote}";
        randomMapVoteDisplay.text = $"VOTES: {randomMapVote}";
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
        string selectedMap;

        if (mapOneVote > mapTwoVote && mapOneVote > randomMapVote)
        {
            selectedMap = "S04_Railway"; 
            mapImage = mapOne;
        }
        else if (mapTwoVote > mapOneVote && mapTwoVote > randomMapVote)
        {
            selectedMap = "S04_PublicMatch"; 
            mapImage = mapTwo;
        }
        else if (randomMapVote > mapOneVote && randomMapVote > mapTwoVote)
        {
            selectedMap = GetRandomMap();
            mapImage = mapTwo;
        }
        else
        {
            selectedMap = "S04_PublicMatch";
            mapImage = mapTwo;
        }

        GameManager.instance.StartLoadingBar(selectedMap, true, mapImage);
    }

    private string GetRandomMap()
    {
        List<string> maps = new List<string> {"S04_PublicMatch", "S04_Railway"};
        return maps[Random.Range(0, maps.Count)];
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

    //Delete this method eventually
    public void GiveMasterClientStartButton()
    {
        if (PhotonNetwork.IsMasterClient)
            startButton.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        TeamManager.AssignTeam(PhotonNetwork.LocalPlayer);
        Debug.Log($"OnJoinedRoom called, PhotonView ID: {photonView.ViewID}");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
            GiveMasterClientStartButton();
        }

        StartCoroutine(UpdatePlayerCountAfterDelay());
        ListPlayers();
        GameManager.instance.CloseLoadingScreen();
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
        photonView.RPC("UpdatePlayerCount", RpcTarget.All);
        ListPlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"OnPlayerLeftRoom called for {otherPlayer.NickName}, PlayerCount: {PhotonNetwork.CurrentRoom.PlayerCount}");
        photonView.RPC("UpdatePlayerCount", RpcTarget.All);
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
            stream.SendNext(mapOneVote);
            stream.SendNext(mapTwoVote);
            stream.SendNext(randomMapVote);
        }
        else
        {
            currentTime = (int)stream.ReceiveNext();
            mapOneVote = (int)stream.ReceiveNext();
            mapTwoVote = (int)stream.ReceiveNext();
            randomMapVote = (int)stream.ReceiveNext();
        }
    }
}
