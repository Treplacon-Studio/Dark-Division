using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro; 

public class MainMenuManager : MonoBehaviourPunCallbacks
{

    public enum MenuNavigationState
    {
        PublicMatch,
        ChangeGamertag,
        None
    }

    public MenuNavigationState currentState = MenuNavigationState.None;

    public GameObject ButtonPanel;
    public GameObject SelectLoadoutPanel;
    public GameObject EditLoadoutPanel;
    public GameObject ShopPanel;

    [Header("GAME MODE")]
    public GameObject SelectGamePanel;
    public GameObject SelectGameModeContainer;
    public GameObject SelectGameTypeContainer;
    public GameObject loadoutList; 
    private int hoveredLoadoutIndex = -1;

    [Header("Loadout UI")]
    public GameObject[] loadoutButtons;
    public GameObject renameScreen; 

    public GameObject inputFieldGameObject;
    private TMP_InputField inputField;

    #region Unity Methods

    private void Start()
    {
        SetPanelViewability();
        ConnectToPhotonServer();
        inputField = inputFieldGameObject.GetComponent<TMP_InputField>();
    }

     private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && hoveredLoadoutIndex >= 0 && hoveredLoadoutIndex < loadoutButtons.Length)
        {
            renameScreen.SetActive(!renameScreen.activeSelf);
        }
    }

    public void SetPanelViewability(bool selectGamePanel = false, 
                                    bool buttonPanel = false, 
                                    bool selectLoadoutPanel = true,
                                    bool editLoadoutPanel = false,
                                    bool gameTypeContainer = false,
                                    bool gameModeContainer = false,
                                    bool shopPanel = false)
    {
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
        string playerName = PhotonNetwork.NickName;
        if (!PhotonNetwork.IsConnected && playerName.Length < 14 && playerName.Length > 3)
        {
            PhotonNetwork.ConnectUsingSettings();
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

    public override void OnCreatedRoom()
    {
        Debug.Log($"{PhotonNetwork.NickName} created a room!");
        SetPanelViewability(buttonPanel:true);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"{PhotonNetwork.NickName} joined the room!");
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

    #region Loadout Rename

    public void OnLoadoutHoverEnter(int loadoutIndex)
    {
        hoveredLoadoutIndex = loadoutIndex;
        Debug.Log("Hovered over loadout index: " + loadoutIndex);
    }

     public void OnLoadoutHoverExit()
    {
        Debug.Log("Exited loadout hover");
    }

    public void RenameSubmit()
{
    if (hoveredLoadoutIndex >= 0 && hoveredLoadoutIndex < loadoutButtons.Length)
    {
        
        Transform loadoutNameTransform = loadoutButtons[hoveredLoadoutIndex].transform.Find("LoadOutName");
        Debug.Log("Loadout Name Transform: " + loadoutNameTransform);
        if (loadoutNameTransform != null)
        {
            TextMeshProUGUI loadoutNameText = loadoutNameTransform.GetComponent<TextMeshProUGUI>();
            Debug.Log("Loadout Name Text: " + loadoutNameText);
            if (loadoutNameText != null)
            {
                loadoutNameText.text = inputField.text;
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on loadout name object.");
            }
        }
        else
        {
            Debug.LogError("Loadout name GameObject not found as a child of loadout button.");
        }
    }
    renameScreen.SetActive(false);
    hoveredLoadoutIndex = -1;
}





    #endregion
}