using System;
using System.Collections.Generic;
using UnityEngine;


public class BulletPoolingManager : MonoBehaviour
{
    [Serializable]
    public class Pool
    {
        public int id;
        public Mag.BulletType bulletType;
        public int size; //Amount of bullets in mag

        public Pool(int idx, Mag.BulletType bt, int s)
        {
            bulletType = bt;
            size = s;
            id = idx;
        }
    }

    [SerializeField] [Tooltip("Ammo holder for the bullet.")]
    private Transform ammoHolder;
    
    [SerializeField] [Tooltip("Bullet object to instantiate in pool.")]
    private GameObject bulletObject;

    [SerializeField] [Tooltip("Pools that will be available in game.")]
    private List<Pool> pools;
    
    private Dictionary<int, Queue<GameObject>> _poolDictionary;
    
    private void Start()
    {
       ApplyPools();
    }

    public void SpawnFromPool(int id, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(id))
        {
            Debug.LogWarning("Pool with id: " + id + " doesn't exist.");
            return;
        }

        var objectToSpawn = _poolDictionary[id].Dequeue();

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        _poolDictionary[id].Enqueue(objectToSpawn);
    }

    public void ClearPools()
    {
        pools = new List<Pool>();
    }

    public void AddPool(Pool pool)
    {
        pools.Add(pool);
    }

    public void ApplyPools()
    {
        _poolDictionary = new Dictionary<int, Queue<GameObject>>();

        foreach (var pool in pools)
        {
            var objectPool = new Queue<GameObject>();

            for (var i = 0; i < pool.size; i++)
            {
                if (ammoHolder == null)
                {
                    Debug.LogError("AmmoHolder not found on player!");
                    continue;
                }

                var obj = Instantiate(bulletObject, ammoHolder, true);
                obj.transform.SetParent(ammoHolder, false);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            _poolDictionary.Add(pool.id, objectPool);
        }
    }
}