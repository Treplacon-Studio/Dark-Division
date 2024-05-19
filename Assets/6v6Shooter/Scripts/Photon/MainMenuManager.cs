using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    public GameObject UsernameCreationMenu;
    public GameObject SelectGamePanel;
    public GameObject ButtonPanel;
    public GameObject SelectLoadoutPanel;
    public GameObject EditLoadoutPanel;

    #region Unity Methods

    private void Start()
    {
        SetPanelViewability(usernameCreationMenu:true);
    }

    public void SetPanelViewability(bool usernameCreationMenu = false, 
                                    bool selectGamePanel = false, 
                                    bool buttonPanel = false, 
                                    bool selectLoadoutPanel = false)
    {
        UsernameCreationMenu.SetActive(usernameCreationMenu);
        SelectGamePanel.SetActive(selectGamePanel);
        ButtonPanel.SetActive(buttonPanel);
        SelectLoadoutPanel.SetActive(selectLoadoutPanel);
    }

    public void OnCreateClassSelected() => SetPanelViewability(selectLoadoutPanel:true);
    public void BackToMainMenuButton() => SetPanelViewability(buttonPanel:true);

    public void SelectLoadout(int loadoutNum)
    {
        //Eventually call loadoutmanager and pass in this loadoutnum parameter

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
