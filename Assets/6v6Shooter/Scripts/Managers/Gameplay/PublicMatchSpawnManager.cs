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
        {
            instance = this;
        }

        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("PhotonView component missing from PublicMatchSpawnManager.");
        }
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

    [PunRPC]
    public void SpawnPlayer(string team)
    {
        Transform spawnPoint = GetRandomSpawnPoint(team);

        if (spawnPoint == null)
        {
            Debug.LogError("No spawn point found for team: " + team);
            return;
        }

        Debug.Log("Creating Player");
        GameObject newPlayer = PhotonNetwork.Instantiate(Path.Combine("Gameplay", "N_Player"), spawnPoint.position, spawnPoint.rotation);

        Transform cameraPosition = FindCameraPositionWithinNewPlayer(newPlayer.transform, "CAMERAPOSITION");

        PhotonView playerPhotonView = newPlayer.GetComponent<PhotonView>();
        PlayerMotor playerMotor = newPlayer.GetComponent<PlayerMotor>();

        if (PlayerTracker.instance != null && PlayerTracker.instance.pv != null)
        {
            PlayerTracker.instance.pv.RPC("AddPlayer", RpcTarget.All, playerPhotonView.ViewID);
            Debug.Log($"Spawning in {newPlayer}. SPAWNED IN!");
        }
        else
        {
            Debug.LogError("PlayerTracker instance or PhotonView not initialized.");
        }

        GameObject instantiatedCamera = PhotonNetwork.Instantiate(Path.Combine("Gameplay", "FirstPersonCamera"), cameraPosition.transform.position, cameraPosition.transform.rotation);
        instantiatedCamera.transform.SetParent(cameraPosition.transform);
        if (photonView.IsMine)
            playerMotor.fpsCamera = instantiatedCamera;

        photonView.RPC("MarkSpawnPointOccupied", RpcTarget.AllBuffered, spawnPoint, team);

    }

    Transform FindCameraPositionWithinNewPlayer(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindCameraPositionWithinNewPlayer(child, name);
            if (result != null)
                return result;
        }
        return null;
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
            photonView.RPC("SpawnPlayer", RpcTarget.MasterClient, team);
        }
        else
        {
            Debug.LogError("PhotonView not initialized.");
        }
    }
}
