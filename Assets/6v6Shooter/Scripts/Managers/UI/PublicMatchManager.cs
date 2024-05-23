using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PublicMatchManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        GlobalPlayerSpawnerManager.instance.SpawnPlayersInGame();
    }
}
