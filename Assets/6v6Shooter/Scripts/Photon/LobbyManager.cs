using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    void Start() 
    {
        Debug.Log("Lobby scene loaded");
        if (PhotonNetwork.IsConnected) 
        {
            Debug.Log(PhotonNetwork.NickName + " connected to lobby!");
            if (playerPrefab != null) 
            {
                Debug.Log("Spawning player...");
                int randomPoints = Random.Range(0, 5);
                Vector3[] points = new Vector3[] {
                    new Vector3(-3.13f, 1, 0.1884947f),
                    new Vector3(-1.16f, 1, 0.1884947f),
                    new Vector3(0.4f, 1, 0.1884947f),
                    new Vector3(0.3f, 1, 0.1884947f),
                    new Vector3(0.5f, 1, 0.1884947f),
                    new Vector3(0.6f, 1, 0.1884947f)
                };
                PhotonNetwork.Instantiate(playerPrefab.name, points[randomPoints], Quaternion.identity);
            }
        }
    }

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
