using Photon.Pun;
using UnityEngine;

public class GlobalPlayerSpawnerManager : MonoBehaviourPunCallbacks
{
    public static GlobalPlayerSpawnerManager instance;

    public GameObject playerPrefab;

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
            if (playerPrefab != null)
            {
                Debug.Log("Spawning in " + PhotonNetwork.NickName);
                int randomPoints = Random.Range(0, 3);
                Vector3[] points = new Vector3[] {
                    new Vector3(-2.182329f, 2.307999f, 0.4655762f),
                    new Vector3(2.97f, 1.04f, 0.08f),
                    new Vector3(3.59f, 1.04f, -2.64f),
                    new Vector3(0.52f, 1.04f, -2.43f)
                };
                PhotonNetwork.Instantiate(playerPrefab.name, points[randomPoints], Quaternion.identity);
                Debug.Log(PhotonNetwork.NickName + " spawned in!");
            }
        }
    }

    public void SpawnPlayersInGame()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (playerPrefab != null)
            {
                Debug.Log("Spawning in " + PhotonNetwork.NickName);


                string team = TeamManager.GetTeam(PhotonNetwork.LocalPlayer).ToString().ToLower();

                SpawnManager.instance.SpawnPlayer(playerPrefab, team);

                Debug.Log(PhotonNetwork.NickName + " spawned in at team " + team + " spawn point");
            }
            else
            {
                Debug.LogError("playerPrefab null");
            }
        }
        else
        {
            Debug.LogError("PhotonNetwork is not connected!");
        }
    }
}
