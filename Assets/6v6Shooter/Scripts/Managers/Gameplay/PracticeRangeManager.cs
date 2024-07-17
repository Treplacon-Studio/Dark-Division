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
        SpawnTargetsInPracticeRange();
        SpawnPlayerInPracticeRange();
    }

    private void SpawnPlayerInPracticeRange()
    {
        practiceRangeSpawnPosition.position = new Vector3(-25, 0, 0);
        PhotonNetwork.Instantiate(Path.Combine("Gameplay", "Player_M01"), practiceRangeSpawnPosition.position, practiceRangeSpawnPosition.rotation);
    }

    private void SpawnTargetsInPracticeRange()
    {
        foreach (Transform spawnPosition in practiceDummyType1SpawnPositions)
        {
            PhotonNetwork.Instantiate(Path.Combine("TargetPractice", "SM_DummyOnStick"), spawnPosition.position, spawnPosition.rotation);
        }

        foreach (Transform spawnPosition in practiceDummyType2SpawnPositions)
        {
            PhotonNetwork.Instantiate(Path.Combine("TargetPractice", "DummiesOnstick"), spawnPosition.position, spawnPosition.rotation);
        }
    }
}
