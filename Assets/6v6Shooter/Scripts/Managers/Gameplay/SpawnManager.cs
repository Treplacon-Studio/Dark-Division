//This script is designed for managing spawn points for each team

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform[] redSpawnPoints;
    public Transform[] blueSpawnPoints;

    private int redNextSpawnIndex = 0;
    private int blueNextSpawnIndex = 0;

    public void SpawnPlayer(GameObject player, string team)
    {
        Transform spawnPoint;

        if (team == "red")
        {
            spawnPoint = redSpawnPoints[redNextSpawnIndex];
            redNextSpawnIndex = (redNextSpawnIndex + 1) % redSpawnPoints.Length;
        }
        else
        {
            spawnPoint = blueSpawnPoints[blueNextSpawnIndex];
            blueNextSpawnIndex = (blueNextSpawnIndex + 1) % blueSpawnPoints.Length;
        }

        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;
    }
}