using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;
    public Transform[] redSpawnPoints;
    public Transform[] blueSpawnPoints;

    private int redNextSpawnIndex = 0;
    private int blueNextSpawnIndex = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public Transform GetNextSpawnPoint(string team)
    {
        Transform spawnPoint;

        if (team == "red")
        {
            if (redSpawnPoints.Length == 0)
            {
                Debug.LogError("No red spawn points assigned!");
                return null;
            }
            spawnPoint = redSpawnPoints[redNextSpawnIndex];
            redNextSpawnIndex = (redNextSpawnIndex + 1) % redSpawnPoints.Length;
        }
        else if (team == "blue")
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

    public void SpawnPlayer(GameObject playerPrefab, string team)
    {
        Transform spawnPoint = GetNextSpawnPoint(team);

        if (spawnPoint == null)
        {
            Debug.LogError("No spawn point found for team: " + team);
            return;
        }

        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
