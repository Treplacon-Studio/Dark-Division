using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public void JoinOrCreateRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 12});
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("S01_Lobby");
    }
}
