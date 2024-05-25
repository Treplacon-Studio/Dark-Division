using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PublicMatchSpawnManager : MonoBehaviour
{
    public static PublicMatchSpawnManager instance;

    public Transform[] redSpawnPoints;
    public Transform[] blueSpawnPoints;

    private int redNextSpawnIndex = 0;
    private int blueNextSpawnIndex = 0;

    public GameObject cameraPrefab;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public Transform GetNextSpawnPoint(string team)
    {
        Transform spawnPoint;

        if (team == "Red")
        {
            if (redSpawnPoints.Length == 0)
            {
                Debug.LogError("No red spawn points assigned!");
                return null;
            }
            spawnPoint = redSpawnPoints[redNextSpawnIndex];
            redNextSpawnIndex = (redNextSpawnIndex + 1) % redSpawnPoints.Length;
        }
        else if (team == "Blue")
        {
            if (blueSpawnPoints.Length == 0)
            {
                Debug.LogError("No blue spawn points assigned!");
                return null;
            }
            spawnPoint = blueSpawnPoints[blueNextSpawnIndex];
            blueNextSpawnIndex = (blueNextSpawnIndex + 1) % blueSpawnPoints.Length;
        }
        else
        {
            Debug.LogError("Invalid team specified!");
            return null;
        }

        return spawnPoint;
    }

    public void SpawnPlayer(string team)
    {
        Transform spawnPoint = GetNextSpawnPoint(team);

        if (spawnPoint == null)
        {
            Debug.LogError("No spawn point found for team: " + team);
            return;
        }

        Debug.Log("Creating Player");
        GameObject newPlayer = PhotonNetwork.Instantiate(Path.Combine("Gameplay", "N_Player"), spawnPoint.position, spawnPoint.rotation);

        Transform cameraPosition = newPlayer.transform.Find("CameraPosition");

		PhotonView photonView = newPlayer.GetComponent<PhotonView>();
		PlayerMotor playerMotor = newPlayer.GetComponent<PlayerMotor>();

		PlayerTracker.instance.pv.RPC("AddPlayer", RpcTarget.All, photonView.ViewID);
        Debug.Log($"Spawning in {newPlayer}. SPAWNED IN!");

        GameObject instantiatedCamera = Instantiate(cameraPrefab, cameraPosition.transform.position, cameraPosition.transform.rotation);
        instantiatedCamera.transform.SetParent(cameraPosition.transform);
    }
}
