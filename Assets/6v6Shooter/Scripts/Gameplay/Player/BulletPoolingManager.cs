using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
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
