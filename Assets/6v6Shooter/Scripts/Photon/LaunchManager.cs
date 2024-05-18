using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public GameObject UsernameCreationMenu;
    public GameObject SelectGamePanel;

    public GameObject playerPrefab;

    #region Unity Methods

    void Start()
    {
        SetPanelViewability(true, false);
    }

    public void SetPanelViewability(bool usernameCreationMenu, bool selectGamePanel)
    {
        UsernameCreationMenu.SetActive(usernameCreationMenu);
        SelectGamePanel.SetActive(selectGamePanel);
    }

    #endregion

    #region Photon Callbacks

    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            SetPanelViewability(false, false);
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + " connected to Photon Server");
        SetPanelViewability(false, true);
        CreateRoom();
    }

    public override void OnConnected() => Debug.Log("Connected to Internet");

    public void JoinOrCreateRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 1});
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 12});
    }

    public override void OnJoinedRoom()
    {
        GlobalPlayerSpawnerManager.instance.SpawnPlayersInMainMenu();
    }

    #endregion
}
