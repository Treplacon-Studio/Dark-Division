using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;


public class PracticeTargetSpawner : MonoBehaviour
{
    public int numberOfEnemies = 5;
    public Vector3 minSpawnPosition = new Vector3(0, 0, 0);
    public Vector3 maxSpawnPosition = new Vector3(20, 0, 20);

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < numberOfEnemies; i++)
            {
                Vector3 randomPosition = GetRandomPosition();
                SpawnAndMakeIndependent(randomPosition);
            }
        }
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(minSpawnPosition.x, maxSpawnPosition.x);
        float y = Random.Range(minSpawnPosition.y, maxSpawnPosition.y);
        float z = Random.Range(minSpawnPosition.z, maxSpawnPosition.z);
        return new Vector3(x, y, z);
    }

    void SpawnAndMakeIndependent(Vector3 position)
    {
        GameObject obj = PhotonNetwork.InstantiateRoomObject(Path.Combine("TargetPractice", "SM_DummyOnStick"), position, Quaternion.identity);
        MakeIndependent(obj);
    }

    void MakeIndependent(GameObject obj)
    {
        PhotonView photonView = obj.GetComponent<PhotonView>();

        if (photonView != null)
        {
            photonView.TransferOwnership(0); // Assign to "Scene" to make it independent of any player
            Debug.Log("Assigned PhotonView ID and made it independent: " + photonView.ViewID);
        }
        else
        {
            Debug.LogError("PhotonView component is missing on the object.");
        }
    }
}
