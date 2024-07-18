using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MainMenuManager : MonoBehaviourPunCallbacks
{

    public enum MenuNavigationState
    {
        PublicMatch,
        ChangeGamertag,
        None
    }

    public MenuNavigationState currentState = MenuNavigationState.None;

    public MM_CinemaCC cameraManage;

    public GameObject ButtonPanel;
    public GameObject SelectLoadoutPanel;
    public GameObject EditLoadoutPanel;
    public GameObject ShopPanel;
    public GameObject SettingsPanel;

    [Header("GAME MODE")]
    public GameObject SelectGamePanel;
    public GameObject SelectGameModeContainer;
    public GameObject SelectGameTypeContainer;
    public GameObject SelectNavRow;
    public GameObject loadoutList; 


    #region Unity Methods

    private void Start()
    {
        SetPanelViewability();
        ConnectToPhotonServer();
        cameraManage.SetCameraPriority("main");
    }

    public void SetPanelViewability(bool selectGamePanel = false, 
                                    bool buttonPanel = false, 
                                    bool selectLoadoutPanel = false,
                                    bool editLoadoutPanel = false,
                                    bool gameTypeContainer = false,
                                    bool navRow = false,
                                    bool gameModeContainer = false,
                                    bool shopPanel = false,
                                    bool settingsPanel = false)
    {
        SelectGamePanel.SetActive(selectGamePanel);
        ButtonPanel.SetActive(buttonPanel);
        SelectLoadoutPanel.SetActive(selectLoadoutPanel);
        EditLoadoutPanel.SetActive(editLoadoutPanel);
        SelectGameTypeContainer.SetActive(gameTypeContainer);
        SelectNavRow.SetActive(navRow);
        SelectGameModeContainer.SetActive(gameModeContainer);
        ShopPanel.SetActive(shopPanel);
        SettingsPanel.SetActive(settingsPanel);
    }

    public void OnPlayGameSelected()
    { 
        SetPanelViewability(selectGamePanel:true, gameTypeContainer:true, navRow:true);
        cameraManage.SetCameraPriority("mode");
    }
    public void OnQuickplaySelected() => SetPanelViewability(selectGamePanel:true, gameModeContainer:true, navRow:true);
    public void OnRankedPlaySelected() {}
    public void OnPracticeRangeSelected() => GameManager.instance.StartLoadingBar("S05_PracticeRange", false);
    public void OnCreateClassSelected() 
    {
        SetPanelViewability(selectLoadoutPanel:true);
        cameraManage.SetCameraPriority("workbench");
    }
    public void SelectLoadout(int loadoutNum) => SetPanelViewability(editLoadoutPanel:true);
    public void OnStoreSelected() 
    {
        SetPanelViewability(shopPanel:true);
        cameraManage.SetCameraPriority("shop");
    }
    public void OnSettingsSelected() 
    {
        SetPanelViewability(settingsPanel:true);
        cameraManage.SetCameraPriority("settings");
    }
    public void OnQuitSelected() {
        cameraManage.SetCameraPriority("exit");
    }

    //Back buttons
    public void OnBackToMainMenuButtonClicked() 
    {
        SetPanelViewability(buttonPanel:true);
        cameraManage.SetCameraPriority("main");
    }
    public void OnBackToSelectLoadoutButtonClicked() => SetPanelViewability(selectLoadoutPanel:true);

    public void FindAnOpenMatch()
    {
        currentState = MenuNavigationState.PublicMatch;
        PhotonNetwork.LeaveRoom();
    }

    public void OnChangeGamertagSelected()
    {
        currentState = MenuNavigationState.ChangeGamertag;
        PlayerPrefsManager.ClearKey("PlayerNickname");
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Photon Callbacks

    public void ConnectToPhotonServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + " connected to Photon Server");
        CreateRoom();
    }

    public override void OnConnected() => Debug.Log("Connected to Internet");

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 1});
    }

    public void JoinOrCreateRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4});
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"{PhotonNetwork.NickName} created a room!");
        SetPanelViewability(buttonPanel:true);
        GameManager.instance.CloseLoadingScreen();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"{PhotonNetwork.NickName} joined the room!");
        GameManager.instance.CloseLoadingScreen();
    }

    public override void OnLeftRoom()
    {
        Debug.Log($"{PhotonNetwork.NickName} left the room!");
        PhotonNetwork.Disconnect();
        
        if (currentState == MenuNavigationState.PublicMatch)
            GameManager.instance.StartLoadingBar("S02_Lobby", false);
        else if (currentState == MenuNavigationState.ChangeGamertag)
            GameManager.instance.StartLoadingBar("S00_UserLogin", false);
        else
            Debug.Log("Menu navigation state was not specified!");
    }

    #endregion
}