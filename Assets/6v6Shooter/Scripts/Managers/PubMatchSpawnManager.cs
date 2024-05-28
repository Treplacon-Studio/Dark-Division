using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun.UtilityScripts;

public class PubMatchSpawnManager : MonoBehaviourPunCallbacks
{
    public static PubMatchSpawnManager instance;


    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SpawnPlayerInGame()
    {
        string team = TeamManager.GetTeam(PhotonNetwork.NickName);
        PublicMatchSpawnManager.instance.SpawnPlayer(team);
    }
}
