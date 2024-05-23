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
    public GameObject GameSelectionPanel;

    #region Unity Methods

    private void Start()
    {
        SetPanelViewability(usernameCreationMenu:true);
    }

    public void SetPanelViewability(bool usernameCreationMenu = false, 
                                    bool selectGamePanel = false, 
                                    bool buttonPanel = false, 
                                    bool selectLoadoutPanel = false,
                                    bool editLoadoutPanel = false,
                                    bool gameSelectionPanel = false)
    {
        UsernameCreationMenu.SetActive(usernameCreationMenu);
        SelectGamePanel.SetActive(selectGamePanel);
        ButtonPanel.SetActive(buttonPanel);
        SelectLoadoutPanel.SetActive(selectLoadoutPanel);
        EditLoadoutPanel.SetActive(editLoadoutPanel);
        GameSelectionPanel.SetActive(gameSelectionPanel);
    }

    public void OnCreateClassSelected() => SetPanelViewability(selectLoadoutPanel:true);
    public void OnBackToMainMenuButtonClicked() => SetPanelViewability(buttonPanel:true);
    public void OnBackToSelectLoadoutButtonClicked() => SetPanelViewability(selectLoadoutPanel:true);

    public void SelectLoadout(int loadoutNum)
    {
        //Eventually call loadoutmanager and pass in this loadoutnum parameter

        SetPanelViewability(editLoadoutPanel:true);
    }

    public void OnPlayGameSelected()
    {
        SetPanelViewability(gameSelectionPanel:true);
    }

    public void FindAnOpenMatch()
    {
        GameManager.instance.OpenLoadingScreen();
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Photon Callbacks

    public void ConnectToPhotonServer()
    {
        string playerName = PhotonNetwork.NickName;
        if (!PhotonNetwork.IsConnected && playerName.Length < 14 && playerName.Length > 3)
        {
            GameManager.instance.OpenLoadingScreen();
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
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4});
    }

    public override void OnJoinedRoom()
    {
        GlobalPlayerSpawnerManager.instance.SpawnPlayersInMainMenu();
        GameManager.instance.CloseLoadingScreen();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("S01_Lobby");
    }

    #endregion
}
