using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PublicMatchSpawnManager : MonoBehaviourPunCallbacks
{
    public enum GameModeType
    {
        None,
        TrainingMode,
        TeamDeathmatch
    }

    public static PublicMatchSpawnManager instance;

    public Transform[] redSpawnPoints;
    public Transform[] blueSpawnPoints;

    public GameModeType selectedGameModeType = GameModeType.None;

    private List<Transform> occupiedRedSpawnPoints = new List<Transform>();
    private List<Transform> occupiedBlueSpawnPoints = new List<Transform>();

    public Transform practiceRangeSpawnPosition;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        string currentScene = SceneHandler.Instance.GetCurrentSceneName();
        if (currentScene == "S05_PracticeRange")
            SpawnPlayerInPracticeRange();
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

    // Pick a random spawn point
    int randomIndex = Random.Range(0, availableSpawnPoints.Count);
    spawnPoint = availableSpawnPoints[randomIndex];

    Debug.Log($"Selected spawn point for team {team}: {spawnPoint.position}");

    return spawnPoint;
}


    [PunRPC]
    public void MarkSpawnPointOccupied(Vector3 position, Quaternion rotation, string team)
    {
        Transform spawnPoint = FindSpawnPointByPositionAndRotation(position, rotation);

        if (spawnPoint == null)
        {
            Debug.LogError("No spawn point found for position and rotation");
            return;
        }

        if (team == "Red")
            occupiedRedSpawnPoints.Add(spawnPoint);
        else if (team == "Blue")
            occupiedBlueSpawnPoints.Add(spawnPoint);

        Debug.Log($"Spawn point marked occupied for team {team}: {spawnPoint.position}");
    }

    [PunRPC]
    public void SpawnPlayer(string team)
    {
        Debug.Log($"Spawning player for team {team}");

        Transform spawnPoint = GetRandomSpawnPoint(team);

        if (spawnPoint == null)
        {
            Debug.LogError("No spawn point found for team: " + team);
            return;
        }

        GameObject newPlayer = PhotonNetwork.Instantiate(Path.Combine("Gameplay", "Player_M01"), spawnPoint.position, spawnPoint.rotation);
        PhotonView playerPhotonView = newPlayer.GetComponent<PhotonView>();

        photonView.RPC("MarkSpawnPointOccupied", RpcTarget.AllBuffered, spawnPoint.position, spawnPoint.rotation, team);
    }

    private IEnumerator FreeSpawnPointAfterDelay(Transform spawnPoint, string team, float delay)
    {
        yield return new WaitForSeconds(delay);
        FreeSpawnPoint(spawnPoint, team);
    }

    public void FreeSpawnPoint(Transform spawnPoint, string team)
    {
        if (team == "Red")
            occupiedRedSpawnPoints.Remove(spawnPoint);
        else if (team == "Blue")
            occupiedBlueSpawnPoints.Remove(spawnPoint);

        Debug.Log($"Spawn point freed for team {team}: {spawnPoint.position}");
    }

    public void RequestSpawnPlayer(string team)
    {
        if (photonView != null)
            photonView.RPC("SpawnPlayer", RpcTarget.All, team);
        else
            Debug.LogError("PhotonView not initialized.");
    }

    private Transform FindSpawnPointByPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        foreach (Transform spawnPoint in redSpawnPoints)
        {
            if (spawnPoint.position == position && spawnPoint.rotation == rotation)
                return spawnPoint;
        }

        foreach (Transform spawnPoint in blueSpawnPoints)
        {
            if (spawnPoint.position == position && spawnPoint.rotation == rotation)
                return spawnPoint;
        }

        return null;
    }

    public void SpawnPlayerInPracticeRange()
    {
        practiceRangeSpawnPosition.position = new Vector3(-25, 0, 0);
        PhotonNetwork.Instantiate(Path.Combine("Gameplay", "Player_M01"), practiceRangeSpawnPosition.position, practiceRangeSpawnPosition.rotation);
    }
}
