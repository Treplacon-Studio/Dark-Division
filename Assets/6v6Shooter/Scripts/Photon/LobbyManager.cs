using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public override void OnJoinedRoom() 
    {
        Debug.Log(PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) 
    {
        Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnLeftRoom() 
    {
        SceneManager.LoadScene("S00_MainMenu");
    }

    public void LeaveRoom() 
    {
        PhotonNetwork.LeaveRoom();
    }
}
