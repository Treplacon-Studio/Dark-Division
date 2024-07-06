using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PracticeRangeManager : MonoBehaviourPunCallbacks
{
    public Transform practiceRangeSpawnPosition;

    void Start()
    {
        SpawnPlayerInPracticeRange();
        SpawnTargetsInPracticeRange();
    }

    private void SpawnPlayerInPracticeRange()
    {
        practiceRangeSpawnPosition.position = new Vector3(-25, 0, 0);
        PhotonNetwork.Instantiate(Path.Combine("Gameplay", "Player_M01"), practiceRangeSpawnPosition.position, practiceRangeSpawnPosition.rotation);
    }

    private void SpawnTargetsInPracticeRange()
    {
        
    }
}
