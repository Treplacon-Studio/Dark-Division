using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PracticeRangeManager : MonoBehaviourPunCallbacks
{
    public Transform practiceRangeSpawnPosition;
    public Transform[] practiceDummyType1SpawnPositions;
    public Transform[] practiceDummyType2SpawnPositions;

    void Start()
    {
        // Each player spawns themselves in the practice range
        SpawnPlayerInPracticeRange();

        // Only the Master Client should spawn the targets
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnTargetsInPracticeRange();
        }
    }

     private void SpawnPlayerInPracticeRange()
    {
        // Instantiate player at the designated spawn position
        Vector3 spawnPosition = practiceRangeSpawnPosition.position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
        PhotonNetwork.Instantiate(Path.Combine("Gameplay", "Player_M01"), spawnPosition, practiceRangeSpawnPosition.rotation);
    }

    private void SpawnTargetsInPracticeRange()
    {
        // Spawn first type of practice dummies
        foreach (Transform spawnPosition in practiceDummyType1SpawnPositions)
        {
            PhotonNetwork.Instantiate(Path.Combine("TargetPractice", "SM_DummyOnStick"), spawnPosition.position, spawnPosition.rotation);
        }

        // Spawn second type of practice dummies
        foreach (Transform spawnPosition in practiceDummyType2SpawnPositions)
        {
            PhotonNetwork.Instantiate(Path.Combine("TargetPractice", "DummiesOnstick"), spawnPosition.position, spawnPosition.rotation);
        }

        Debug.Log("All targets spawned in the practice range.");
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
}
