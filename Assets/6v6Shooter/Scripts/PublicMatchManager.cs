using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PublicMatchManager : MonoBehaviourPunCallbacks
{
    void Awake()
    {
        GlobalPlayerSpawnerManager.instance.SpawnPlayersInGame();
    }
}
