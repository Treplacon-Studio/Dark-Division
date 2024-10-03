using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

public class PracticeRangeManager : MonoBehaviourPunCallbacks
{
    public Transform practiceRangeSpawnPosition;
    public Transform[] practiceDummyType1SpawnPositions;
    public Transform[] practiceDummyType2SpawnPositions;

    void Start()
    {
        SpawnPlayerInPracticeRange();

        if (PhotonNetwork.IsMasterClient)
        {
            SpawnTargetsInPracticeRange();
        }
    }

    private void SpawnPlayerInPracticeRange()
    {
        // Instantiate player at the designated spawn position
        practiceRangeSpawnPosition.position = new Vector3(-25, 0, 0);
        PhotonNetwork.Instantiate(Path.Combine("Gameplay", "Player_M01"), practiceRangeSpawnPosition.position, practiceRangeSpawnPosition.rotation);
    }

    private void SpawnTargetsInPracticeRange()
    {
        // Example: Spawn Type 1 Dummies
        foreach (var spawnPosition in practiceDummyType1SpawnPositions)
        {
            PhotonNetwork.Instantiate(Path.Combine("TargetPractice", "SM_DummyOnStick"), spawnPosition.position, spawnPosition.rotation);
        }

        // Example: Spawn Type 2 Dummies
        foreach (var spawnPosition in practiceDummyType2SpawnPositions)
        {
            PhotonNetwork.Instantiate(Path.Combine("TargetPractice", "DummiesOnstick"), spawnPosition.position, spawnPosition.rotation);
        }
    }

    // This callback is triggered when the local player successfully joins a room
    public override void OnJoinedRoom()
    {
        Debug.Log("Player joined the practice range room: " + PhotonNetwork.CurrentRoom.Name);
    }

    // If you want to handle specific player actions on leaving the room, implement this callback
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " has left the room.");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        
        // Example: Send current game state to the new player
        photonView.RPC("SyncGameState", newPlayer);
    }
}
