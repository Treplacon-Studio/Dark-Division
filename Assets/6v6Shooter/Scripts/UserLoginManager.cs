using Photon.Pun;
using UnityEngine;

public class UserLoginManager : MonoBehaviour
{
    public void SetPlayerName(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            Debug.Log("Player name is invalid!");
            return;
        }

        PhotonNetwork.NickName = playerName;
    }
}
