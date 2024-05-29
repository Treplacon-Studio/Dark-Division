using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun.UtilityScripts;

public class MainMenuSpawnManager : MonoBehaviourPunCallbacks
{
    public static MainMenuSpawnManager instance;

    public GameObject playerMenuPrefab;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SpawnPlayersInMainMenu()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log(PhotonNetwork.NickName + " connected to lobby!");
            if (playerMenuPrefab != null)
            {
                Debug.Log("Spawning in " + PhotonNetwork.NickName);
                int randomPoints = Random.Range(0, 3);
                Vector3[] points = new Vector3[] {
                    new Vector3(-2.182329f, 2.307999f, 0.4655762f),
                    new Vector3(2.97f, 1.04f, 0.08f),
                    new Vector3(3.59f, 1.04f, -2.64f),
                    new Vector3(0.52f, 1.04f, -2.43f)
                };
                GameObject newPlayer = PhotonNetwork.Instantiate(playerMenuPrefab.name, points[randomPoints], Quaternion.identity);
                Debug.Log(PhotonNetwork.NickName + " spawned in!");
            }
        }
    }
}
