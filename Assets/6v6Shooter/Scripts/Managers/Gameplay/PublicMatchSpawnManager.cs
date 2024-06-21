using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PublicMatchSpawnManager : MonoBehaviourPunCallbacks
{
    public static PublicMatchSpawnManager instance;

    public Transform[] redSpawnPoints;
    public Transform[] blueSpawnPoints;

    private List<Transform> occupiedRedSpawnPoints = new List<Transform>();
    private List<Transform> occupiedBlueSpawnPoints = new List<Transform>();

    private PhotonView photonView;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        photonView = GetComponent<PhotonView>();
        if (photonView == null)
            Debug.LogError("PhotonView component missing from PublicMatchSpawnManager.");
        else
            photonView.ViewID = 10000;
    }

    public Transform GetRandomSpawnPoint(string team)
    {
        Transform spawnPoint = null;
        List<Transform> availableSpawnPoints = new List<Transform>();

        if (team == "Red")
        {
            foreach (Transform point in redSpawnPoints)
            {
                if (!occupiedRedSpawnPoints.Contains(point))
                {
                    availableSpawnPoints.Add(point);
                }
            }
        }
        else if (team == "Blue")
        {
            foreach (Transform point in blueSpawnPoints)
            {
                if (!occupiedBlueSpawnPoints.Contains(point))
                {
                    availableSpawnPoints.Add(point);
                }
            }
        }
        else
        {
            Debug.LogError("Invalid team specified!");
            return null;
        }

        if (availableSpawnPoints.Count == 0)
        {
            Debug.LogError("No available spawn points for team: " + team);
            return null;
        }

        // pick random spawn point
        int randomIndex = Random.Range(0, availableSpawnPoints.Count);
        spawnPoint = availableSpawnPoints[randomIndex];

        return spawnPoint;
    }

    [PunRPC]
    public void MarkSpawnPointOccupied(Transform spawnPoint, string team)
    {
        if (team == "Red")
        {
            occupiedRedSpawnPoints.Add(spawnPoint);
        }
        else if (team == "Blue")
        {
            occupiedBlueSpawnPoints.Add(spawnPoint);
        }
    }

    public void SpawnPlayer(string team)
    {     
        Debug.Log($"WE ARE SPAWNING PLAYER TO TEAM {team}"); 
        Transform spawnPoint = GetRandomSpawnPoint(team);

        if (spawnPoint == null)
        {
            Debug.LogError("No spawn point found for team: " + team);
            return;
        }

        Debug.Log("Creating Player");
        GameObject newPlayer = PhotonNetwork.Instantiate(Path.Combine("Gameplay", "Player_M01"), spawnPoint.position, spawnPoint.rotation);
        PhotonView playerPhotonView = newPlayer.GetComponent<PhotonView>();
        Debug.Log($"{newPlayer} INSTANTIATED INTO THE SCENE"); 

        photonView.RPC("MarkSpawnPointOccupied", RpcTarget.AllBuffered, spawnPoint, team);
    }

    private IEnumerator FreeSpawnPointAfterDelay(Transform spawnPoint, string team, float delay)
    {
        yield return new WaitForSeconds(delay);
        FreeSpawnPoint(spawnPoint, team);
    }

    public void FreeSpawnPoint(Transform spawnPoint, string team)
    {
        if (team == "Red")
        {
            occupiedRedSpawnPoints.Remove(spawnPoint);
        }
        else if (team == "Blue")
        {
            occupiedBlueSpawnPoints.Remove(spawnPoint);
        }
    }

    public void RequestSpawnPlayer(string team)
    {
        if (photonView != null)
        {
            photonView.RPC("SpawnPlayer", RpcTarget.All, team);
        }
        else
        {
            Debug.LogError("PhotonView not initialized.");
        }
    }
}
