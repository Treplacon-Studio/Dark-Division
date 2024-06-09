using UnityEngine;
using UnityEngine.UI;
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

    public GameObject ButtonPanel;
    public GameObject SelectLoadoutPanel;
    public GameObject EditLoadoutPanel;
    public GameObject ShopPanel;

    [Header("GAME MODE")]
    public GameObject SelectGamePanel;
    public GameObject SelectGameModeContainer;
    public GameObject SelectGameTypeContainer;

    public GameObject loadoutList;
    public GameObject renameInputField;
    private int hoveredLoadoutIndex = -1;
    private bool isRenaming = false;

    #region Unity Methods

    private void Start()
    {
        SetPanelViewability();
        ConnectToPhotonServer();
    }

    private void Update()
    {
        if (isRenaming && Input.GetKeyDown(KeyCode.Return))
        {
            RenameSelectedLoadout();
        }
        else if (!isRenaming && hoveredLoadoutIndex >= 0 && Input.GetKeyDown(KeyCode.R))
        {
            ShowRenameInputField();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R key pressed!");
        }
    }

    public void SetPanelViewability(bool selectGamePanel = false,
                                    bool buttonPanel = false,
                                    bool selectLoadoutPanel = false,
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

    public void OnPlayGameSelected() => SetPanelViewability(selectGamePanel: true, gameTypeContainer: true);
    public void OnQuickplaySelected() => SetPanelViewability(selectGamePanel: true, gameModeContainer: true);
    public void OnRankedPlaySelected() { }
    public void OnPracticeRangeSelected() => GameManager.instance.StartLoadingBar("S05_PraticeRange", true);
    public void OnCreateClassSelected() => SetPanelViewability(selectLoadoutPanel: true);
    public void SelectLoadout(int loadoutNum) => SetPanelViewability(editLoadoutPanel: true);
    public void OnStoreSelected() => SetPanelViewability(shopPanel: true);
    public void OnQuitSelected() => Application.Quit();
    // UnityEditor.EditorApplication.isPlaying = false; use this code if testing in editor

    // Back buttons
    public void OnBackToMainMenuButtonClicked() => SetPanelViewability(buttonPanel: true);
    public void OnBackToSelectLoadoutButtonClicked() => SetPanelViewability(selectLoadoutPanel: true);

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
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 1 });
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"{PhotonNetwork.NickName} created a room!");
        SetPanelViewability(buttonPanel: true);
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

    #region Loadout Renaming Methods

    public void OnLoadoutHoverEnter(int loadoutIndex)
    {
        hoveredLoadoutIndex = loadoutIndex;
        Debug.Log("Hovered over loadout index: " + loadoutIndex);
    }

    public void OnLoadoutHoverExit()
    {
        hoveredLoadoutIndex = -1;
        Debug.Log("Exited loadout hover");
    }

    private void ShowRenameInputField()
    {
        Debug.Log("ShowRenameInputField called");
        isRenaming = true;
        renameInputField.SetActive(true);
        InputField inputFieldComponent = renameInputField.GetComponent<InputField>();
        inputFieldComponent.Select();
    }

    public void RenameSelectedLoadout()
    {
        if (hoveredLoadoutIndex < 0)
        {
            Debug.LogWarning("No loadout selected to rename.");
            return;
        }

        InputField inputFieldComponent = renameInputField.GetComponent<InputField>();
        string newName = inputFieldComponent.text;

        if (string.IsNullOrEmpty(newName))
        {
            Debug.LogWarning("New name cannot be empty.");
            return;
        }

        // Find the loadout button that needs renaming
        Transform loadoutButton = loadoutList.transform.GetChild(hoveredLoadoutIndex);
        Text loadoutButtonText = loadoutButton.GetComponentInChildren<Text>();

        if (loadoutButtonText != null)
        {
            loadoutButtonText.text = newName;
            Debug.Log("Loadout renamed to: " + newName);
        }
        else
        {
            Debug.LogWarning("Loadout button text component not found.");
        }

        // Clear the input field after renaming
        inputFieldComponent.text = "";
        renameInputField.SetActive(false);
        isRenaming = false;
    }

    #endregion
}
