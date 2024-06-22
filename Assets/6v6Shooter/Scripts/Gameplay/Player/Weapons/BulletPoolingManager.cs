using System;
using System.Collections.Generic;
using UnityEngine;


public class BulletPoolingManager : MonoBehaviour
{
    [Serializable]
    public class Pool
    {
        public Mag.BulletType bulletType;
        public int size; //Amount of bullets in mag
    }

    [SerializeField] [Tooltip("Ammo holder for the bullet.")]
    private Transform ammoHolder;
    
    [SerializeField] [Tooltip("Bullet object to instantiate in pool.")]
    private GameObject bulletObject;

    [SerializeField] [Tooltip("Pools that will be available in game.")]
    private List<Pool> pools;
    
    private Dictionary<Mag.BulletType, Queue<GameObject>> _poolDictionary;
    
    private void Start()
    {
        _poolDictionary = new Dictionary<Mag.BulletType, Queue<GameObject>>();

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

            _poolDictionary.Add(pool.bulletType, objectPool);
        }
    }

    public void SpawnFromPool(Mag.BulletType bulletType, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(bulletType))
        {
            Debug.LogWarning("Pool with bulletType " + bulletType + " doesn't exist.");
            return;
        }

        var objectToSpawn = _poolDictionary[bulletType].Dequeue();

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        _poolDictionary[bulletType].Enqueue(objectToSpawn);
    }
}