using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro; 
using System.Collections;
using System.IO;

public class MainMenuManager : MonoBehaviourPunCallbacks
{

    public enum MenuNavigationState
    {
        PublicMatch,
        ChangeGamertag,
        HostPracticeRange,
        JoinPracticeRange,
        None
    }

    public MenuNavigationState currentState = MenuNavigationState.None;

    public MM_CinemaCC cameraManage;

    public GameObject ButtonPanel;
    public GameObject SelectLoadoutPanel;
    public GameObject EditLoadoutPanel;
    public GameObject ShopPanel;
    public GameObject SettingsPanel;
    private string playerRoomCode;

    [Header("GAME MODE")]
    public GameObject SelectGamePanel;
    public GameObject SelectGameModeContainer;
    public GameObject SelectPraticeModeContainer;
    public GameObject SelectGameTypeContainer;
    public GameObject SelectNavRow;
    public GameObject loadoutList; 
    private int hoveredLoadoutIndex = -1;
    public GameObject joinScreen;
    public TMP_InputField roomCodeInputField;

    [Header("Loadout UI")]
    public GameObject[] loadoutButtons;
    public GameObject renameScreen;  

    public GameObject inputFieldGameObject;
    private TMP_InputField inputField;
    [SerializeField] private GameObject errMsg;
    private static readonly string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private string targetRoomCode; // Store the room code of the practice range we want to join
    private bool isRejoiningRoom = false;

    #region Unity Methods

    private void Start()
    {
        currentState = MenuNavigationState.None;
        SetPanelViewability();
        ConnectToPhotonServer();
        inputField = inputFieldGameObject.GetComponent<TMP_InputField>();
        cameraManage.SetCameraPriority("main");
    }

     private void Update()
    {
        if (!inputField.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.R) && hoveredLoadoutIndex >= 0 && hoveredLoadoutIndex < loadoutButtons.Length)
            {
                renameScreen.SetActive(!renameScreen.activeSelf);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && renameScreen.activeSelf)
        {
            renameScreen.SetActive(false);
        }
    }

    public void SetPanelViewability(bool selectGamePanel = false, 
                                    bool buttonPanel = false, 
                                    bool selectLoadoutPanel = false,
                                    bool editLoadoutPanel = false,
                                    bool gameTypeContainer = false,
                                    bool navRow = false,
                                    bool gameModeContainer = false,
                                    bool praticeModeContainer = false,
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
        SelectPraticeModeContainer.SetActive(praticeModeContainer);
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
    public void OnPracticeRangeSelected() => SetPanelViewability(selectGamePanel:true, praticeModeContainer:true, navRow:true);
    public void OnHostPracticeRangeSelected() 
    {
        currentState = MenuNavigationState.HostPracticeRange;
        string roomCode = GenerateRoomCode(6);
        targetRoomCode = roomCode;
        RoomManager.SetRoomCode(roomCode);

        if (PhotonNetwork.IsConnectedAndReady) {
            if (PhotonNetwork.InRoom) {

                PhotonNetwork.LeaveRoom();
            } else {
                CreatePracticeRangeRoom(roomCode); 
            }
        } else {
            Debug.Log("Not connected to Master Server. Connecting...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void OnJoinPracticeRangeSelected() => joinScreen.SetActive(true);

    public void OnJoinRangeRoomSelected() 
    {
        string roomCode = roomCodeInputField.text.ToUpper();
        if (!string.IsNullOrEmpty(roomCode))
        {
            currentState = MenuNavigationState.JoinPracticeRange;
            targetRoomCode = roomCode;

            Debug.Log("Leaving current room to join practice range: " + roomCode);

            // Set rejoining flag to true
            isRejoiningRoom = true;

            PhotonNetwork.LeaveRoom(); 
        }
        else
        {
            Debug.LogWarning("Room code is empty, cannot join.");
        }
    }


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
        Application.Quit();
    }

    //Back buttons
    public void OnBackToMainMenuButtonClicked() 
    {
        SetPanelViewability(buttonPanel:true);
        cameraManage.SetCameraPriority("main");
    }
    public void OnBackToSelectLoadoutButtonClicked() => SetPanelViewability(selectLoadoutPanel:true);
    public void OnCancelRename() => renameScreen.SetActive(false);
    public void OnCancelJoin() => joinScreen.SetActive(false);

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
        if (isRejoiningRoom && !string.IsNullOrEmpty(targetRoomCode))
        {
            TryJoinTargetRoom();
        }
        
        if (currentState == MenuNavigationState.HostPracticeRange)
        {
            // Check if we're still in a room and leave if necessary
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                CreatePracticeRangeRoom(targetRoomCode);
            }
        }
        else if (currentState == MenuNavigationState.None)
        {
            Debug.Log(PhotonNetwork.NickName + " connected to Photon Server");

            playerRoomCode = GenerateRoomCode(6);
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = 1, IsVisible = true, IsOpen = true };
            PhotonNetwork.CreateRoom(playerRoomCode, roomOptions, TypedLobby.Default);
            Debug.Log("Main Menu room created with code: " + playerRoomCode);
        }
    }



    public override void OnConnected() => Debug.Log("Connected to Internet");

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4});
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

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Room creation failed: {message} (Error Code: {returnCode})");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined room: " + PhotonNetwork.CurrentRoom.Name);

        if (currentState == MenuNavigationState.JoinPracticeRange)
        {
            // Make sure you are loading the practice range scene correctly
            GameManager.instance.StartLoadingBar("S05_PracticeRange", true);

            // Instantiate the player once the scene has been loaded
            PhotonNetwork.Instantiate(Path.Combine("Gameplay", "Player_M01"), Vector3.zero, Quaternion.identity);
        }
    }


    private void CreatePracticeRangeRoom(string roomCode)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4; 

        PhotonNetwork.CreateRoom(roomCode, roomOptions);
        Debug.Log("Practice range room created with code: " + roomCode);

        GameManager.instance.StartLoadingBar("S05_PracticeRange", true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Failed to join room: {message}");
    }

    public override void OnLeftRoom()
    {
        Debug.Log($"{PhotonNetwork.NickName} left the room!");

        // Reconnect if not connected to Photon
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogError("Not connected to Photon, reconnecting...");
            PhotonNetwork.ConnectUsingSettings();
            return;
        }

        Debug.Log("Current state is: " + currentState);

        if (isRejoiningRoom && PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                TryJoinTargetRoom();
            }
            else
            {
                Debug.Log("Waiting to connect back to the Master Server before joining room...");
            }
        }

        switch (currentState)
        {
            case MenuNavigationState.PublicMatch:
                Debug.Log("Switch case: PublicMatch");
                GameManager.instance.StartLoadingBar("S02_Lobby", false);
                break;

            case MenuNavigationState.ChangeGamertag:
                Debug.Log("Switch case: ChangeGamertag");
                GameManager.instance.StartLoadingBar("S00_UserLogin", false);
                break;

            case MenuNavigationState.JoinPracticeRange:
                Debug.Log("Switch case: JoinPracticeRange");
                if (!string.IsNullOrEmpty(targetRoomCode))
                {
                    PhotonNetwork.JoinRoom(targetRoomCode);
                    Debug.Log("Attempting to join practice range with code: " + targetRoomCode);
                    targetRoomCode = null;
                }
                else
                {
                    Debug.LogError("Room code is null or empty!");
                }
                break;

            default:
                Debug.Log("Menu navigation state was not specified!");
                break;
        }
    }




    #endregion

    #region Loadout Rename

    private IEnumerator DeactivateAfterTime(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

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
    if (string.IsNullOrWhiteSpace(inputField.text))
        {
            errMsg.SetActive(true);
            StartCoroutine(DeactivateAfterTime(errMsg, 3f)); // Start coroutine to deactivate after 3 seconds
            return;
        }

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
        errMsg.SetActive(false);
    }

    private string GenerateRoomCode(int length = 6)
    {
        char[] roomCode = new char[length];
        System.Random random = new System.Random();

        for (int i = 0; i < length; i++)
        {
            roomCode[i] = letters[random.Next(letters.Length)];
        }

        return new string(roomCode);
    }

    private void TryJoinTargetRoom()
{
    if (!string.IsNullOrEmpty(targetRoomCode))
    {
        Debug.Log("Attempting to join practice range with code: " + targetRoomCode);
        PhotonNetwork.JoinRoom(targetRoomCode);
        targetRoomCode = null;
        isRejoiningRoom = false; // Reset the flag
    }
    else
    {
        Debug.LogError("Room code is null or empty!");
    }
}
    
    #endregion
}