using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    public GameObject UsernameCreationMenu;
    public GameObject ButtonPanel;
    public GameObject SelectLoadoutPanel;
    public GameObject EditLoadoutPanel;
    public GameObject ShopPanel;

    [Header("GAME MODE")]
    public GameObject SelectGamePanel;
    public GameObject SelectGameModeContainer;
    public GameObject SelectGameTypeContainer;

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
                                    bool gameTypeContainer = false,
                                    bool gameModeContainer = false,
                                    bool shopPanel = false)
    {
        UsernameCreationMenu.SetActive(usernameCreationMenu);
        SelectGamePanel.SetActive(selectGamePanel);
        ButtonPanel.SetActive(buttonPanel);
        SelectLoadoutPanel.SetActive(selectLoadoutPanel);
        EditLoadoutPanel.SetActive(editLoadoutPanel);
        SelectGameTypeContainer.SetActive(gameTypeContainer);
        SelectGameModeContainer.SetActive(gameModeContainer);
        ShopPanel.SetActive(shopPanel);
    }

    public void OnPlayGameSelected() => SetPanelViewability(selectGamePanel:true, gameTypeContainer:true);
    public void OnQuickplaySelected() => SetPanelViewability(selectGamePanel:true, gameModeContainer:true);
    public void OnRankedPlaySelected() {}
    public void OnPracticeRangeSelected() {}
    public void OnCreateClassSelected() => SetPanelViewability(selectLoadoutPanel:true);
    public void SelectLoadout(int loadoutNum) => SetPanelViewability(editLoadoutPanel:true);
    public void OnStoreSelected() => SetPanelViewability(shopPanel:true);

    //Back buttons
    public void OnBackToMainMenuButtonClicked() => SetPanelViewability(buttonPanel:true);
    public void OnBackToSelectLoadoutButtonClicked() => SetPanelViewability(selectLoadoutPanel:true);

    public void FindAnOpenMatch()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Photon Callbacks

    public void ConnectToPhotonServer()
    {
        string playerName = PhotonNetwork.NickName;
        if (!PhotonNetwork.IsConnected && playerName.Length < 14 && playerName.Length > 3)
        {
            PhotonNetwork.ConnectUsingSettings();
            SetPanelViewability();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + " connected to Photon Server");
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
        //MainMenuSpawnManager.instance.SpawnPlayersInMainMenu();
        SetPanelViewability(buttonPanel:true);
    }

    public override void OnLeftRoom()
    {
        GameManager.instance.StartLoadingBar("S01_Lobby", false);
    }

    #endregion
}
