using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public GameObject UsernameCreationMenu;
    public GameObject ConnectionStatusPanel;
    public GameObject SelectGamePanel;

    #region Unity Methods

    void Start()
    {
        SetPanelViewability(true, false, false);
    }

    public void SetPanelViewability(bool usernameCreationMenu, bool connectionStatusPanel, bool selectGamePanel)
    {
        UsernameCreationMenu.SetActive(usernameCreationMenu);
        ConnectionStatusPanel.SetActive(connectionStatusPanel);
        SelectGamePanel.SetActive(selectGamePanel);
    }

    #endregion

    #region Photon Callbacks

    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            SetPanelViewability(false, true, false);
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + " connected to Photon Server");
        SetPanelViewability(false, false, true);
    }

    public override void OnConnected() => Debug.Log("Connected to Internet");

    public void JoinOrCreateRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 12});
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("S01_Lobby");
    }

    #endregion
}
