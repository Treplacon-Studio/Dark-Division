using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

public class PracticeRangeManager : MonoBehaviourPunCallbacks
{
    public Transform[] playerSpawnPositions; 
    public Transform[] practiceDummyType1SpawnPositions;
    public Transform[] practiceDummyType2SpawnPositions;

    public override void OnJoinedRoom()
    {
        Debug.Log("Player joined the practice range room: " + PhotonNetwork.CurrentRoom.Name);
        StartCoroutine(DelayedPlayerSpawn());  // Add a small delay to ensure network readiness

        if (PhotonNetwork.IsMasterClient)
        {
            SpawnTargetsInPracticeRange();
        }
    }

    // Coroutine to delay player spawn for network readiness
    private IEnumerator DelayedPlayerSpawn()
    {
        yield return new WaitForSeconds(1f);
        SpawnPlayerInPracticeRange();
    }

    private void SpawnPlayerInPracticeRange()
    {
        int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber % playerSpawnPositions.Length;
        Vector3 spawnPosition = playerSpawnPositions[spawnIndex].position;
        Quaternion spawnRotation = playerSpawnPositions[spawnIndex].rotation;

        Debug.Log($"Spawning player at position {spawnPosition}");

        // Attempt to instantiate the player and log errors if any
        try
        {
            PhotonNetwork.Instantiate(Path.Combine("Gameplay", "Player_M01"), spawnPosition, spawnRotation);
            Debug.Log("Player prefab instantiated successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error instantiating player: " + e.Message);
        }
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

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " has left the room.");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("New player entered the room: " + newPlayer.NickName);
        // You can handle additional logic for the new player here if necessary
    }
}
