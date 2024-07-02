using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PracticeRangeManager : MonoBehaviourPunCallbacks
{
    public Transform spawnPoint;

    void Start()
    {
        GameObject newPlayer = PhotonNetwork.Instantiate(Path.Combine("Gameplay", "Player_M01"), spawnPoint.position, spawnPoint.rotation);
    }
}
