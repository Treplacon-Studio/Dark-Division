using Photon.Pun;
using UnityEngine;
using TMPro;

public class UserLoginManager : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;
    public GameObject UserLoginPanel;

    private void Start()
    {
        UserLoginPanel.SetActive(false);
        LoadNickname();
    }

    private void LoadNickname()
    {
        string playerName = PlayerPrefsManager.LoadString("PlayerNickname");        
        if (!string.IsNullOrEmpty(playerName))
        {
            PhotonNetwork.NickName = playerName;
            GameManager.instance.StartLoadingBar("S01_MainMenu", false);
        }
        else
            UserLoginPanel.SetActive(true);
    }

    public void SetPlayerName()
    {
        string playerName = playerNameText.text;
        if (ValidateName(playerName))
        {
            PhotonNetwork.NickName = playerName;
            PlayerPrefsManager.SaveString("PlayerNickname", playerName);
            GameManager.instance.StartLoadingBar("S01_MainMenu", false);
        }
    }

    private bool ValidateName(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("Player name is invalid!");
            return false;
        }

        else if (playerName.Length > 14)
        {
            Debug.Log("Player name cannot exceed 14 characters");
            return false;
        }
        else if (playerName.Length < 3)
        {
            Debug.Log("Player name must be at least 3 characters");
            return false;
        }
        else
        {
            return true;
        }
    }
}
