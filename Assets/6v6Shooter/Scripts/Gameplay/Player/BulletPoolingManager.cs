using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class BulletPoolingManager : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string bulletType;
        public GameObject prefab;
        public int size; //amount of bullets in chamber
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    public GameObject player;
    
    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                Transform ammoHolder = player.transform.Find("AmmoHolder");
                if (ammoHolder == null)
                {
                    Debug.LogError("AmmoHolder not found on player!");
                    continue;
                }

                GameObject obj = PhotonNetwork.Instantiate(Path.Combine("Gameplay", pool.prefab.name), ammoHolder.position, ammoHolder.rotation);
                obj.transform.SetParent(ammoHolder, false);

                BulletController bulletController = obj.GetComponent<BulletController>();
                if (bulletController is not null)
                    bulletController.ownedByPlayer = player; 
                else Debug.Log("Bullet Controller - cannot find!");

                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.bulletType, objectPool);
        }
    }

    public GameObject SpawnFromPool(string bulletType, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(bulletType))
        {
            Debug.LogWarning("Pool with bulletType " + bulletType + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[bulletType].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        PhotonView pv = objectToSpawn.GetComponent<PhotonView>();
        if (pv != null)
            pv.RequestOwnership(); //Transfer owner to whoever created the bullet

        poolDictionary[bulletType].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
