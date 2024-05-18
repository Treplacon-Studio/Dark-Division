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
        Debug.Log("Lobby scene loaded");
        if (PhotonNetwork.IsConnected) 
        {
            Debug.Log(PhotonNetwork.NickName + " connected to lobby!");
            if (playerPrefab != null) 
            {
                Debug.Log("Spawning player...");
                int randomPoints = Random.Range(0, 5);
                Vector3[] points = new Vector3[] {
                    new Vector3(-3.13f, 1, 0.1884947f),
                    new Vector3(-1.16f, 1, 0.1884947f),
                    new Vector3(0.4f, 1, 0.1884947f),
                    new Vector3(0.3f, 1, 0.1884947f),
                    new Vector3(0.5f, 1, 0.1884947f),
                    new Vector3(0.6f, 1, 0.1884947f)
                };
                PhotonNetwork.Instantiate(playerPrefab.name, points[randomPoints], Quaternion.identity);
            }
        }
    }
}
