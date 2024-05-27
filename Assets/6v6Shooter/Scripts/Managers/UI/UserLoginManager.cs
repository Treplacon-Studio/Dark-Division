using Photon.Pun;
using UnityEngine;

namespace UserLogName
{
    public class UserLoginManager : MonoBehaviour
    {
        public void SetPlayerName(string playerName)
        {

            if (ValidateName(playerName))
            {
                PhotonNetwork.NickName = playerName;
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
            // add validation for bad words
            else
            {
                return true;
            }
        }
    }
}
