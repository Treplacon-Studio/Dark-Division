using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    public GameObject UsernameCreationMenu;
    public GameObject SelectGamePanel;
    public GameObject ButtonPanel;
    public GameObject LoadoutPanel;

    #region Unity Methods

    private void Start()
    {
        SetPanelViewability(usernameCreationMenu:true);
    }

    public void SetPanelViewability(bool usernameCreationMenu = false, bool selectGamePanel = false, bool buttonPanel = false, bool loadoutPanel = false)
    {
        UsernameCreationMenu.SetActive(usernameCreationMenu);
        SelectGamePanel.SetActive(selectGamePanel);
        ButtonPanel.SetActive(buttonPanel);
        LoadoutPanel.SetActive(loadoutPanel);
    }

    public void OnCreateClassSelected()
    {
        SetPanelViewability(loadoutPanel:true);
    }

    public void BackToButtonPanel()
    {
        SetPanelViewability(buttonPanel:true);
    }

    #endregion

    #region Photon Callbacks

    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            SetPanelViewability(buttonPanel:true);
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + " connected to Photon Server");
        SetPanelViewability(selectGamePanel:true, buttonPanel:true);
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
