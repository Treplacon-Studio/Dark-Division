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
        public int currentAmmo;

        public Pool(int idx, Mag.BulletType bt, int s)
        {
            bulletType = bt;
            size = s;
            id = idx;
            currentAmmo = s;
        }
    }

    [Header("Bullets")]
    [SerializeField] private GameObject assaultRifleBullet;
    [SerializeField] private GameObject pistolBullet;
    [SerializeField] private GameObject shotgunBullet;
    [SerializeField] private GameObject sniperRifleBullet;
    [SerializeField] private GameObject submachineGunBullet;
    
    [SerializeField] [Tooltip("Ammo holder for the bullet.")]
    private Transform ammoHolder;

    [SerializeField] [Tooltip("Pools that will be available in game.")]
    private List<Pool> pools;

    [SerializeField] [Tooltip("Player game object.")]
    private GameObject player;
    
    private Dictionary<int, Queue<GameObject>> _poolDictionary;
    
    private void Start()
    {
       ApplyPools();
    }

    public int GetAmmoPrimary()
    {
        return pools[ActionsManager.Instance.Switching.GetCurrentWeaponID()].currentAmmo;
    }

    public int GetAmmoSecondary()
    {
        return pools[(ActionsManager.Instance.Switching.GetCurrentWeaponID()+1)%2].currentAmmo;
    }

    public int GetMaxAmmo()
    {
        return pools[ActionsManager.Instance.Switching.GetCurrentWeaponID()].size;
    }

    public int ResetAmmo(int id)
    {
        return pools[id].currentAmmo = pools[id].size;
    }

    public void SpawnFromPool(int id, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(id))
        {
            Debug.LogWarning("Pool with id: " + id + " doesn't exist.");
            return;
        }

        var objectToSpawn = _poolDictionary[id].Dequeue();

        var bp = objectToSpawn.GetComponentInChildren<BulletPilot>();
        bp.gameObject.transform.position = position;
        bp.gameObject.transform.rotation = rotation;
        bp.gameObject.transform.Rotate(90, 0, 0, Space.Self);
        bp.ResetHits();
        bp.SetOwner(player);
        objectToSpawn.SetActive(true);

        _poolDictionary[id].Enqueue(objectToSpawn);

        pools[id].currentAmmo--;
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

                var obj = Instantiate(GetProperBullet(), ammoHolder, true);
                obj.transform.SetParent(ammoHolder, false);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            _poolDictionary.Add(pool.id, objectPool);
        }
    }

    private GameObject GetProperBullet()
    {
        switch (pools[ActionsManager.Instance.Switching.GetCurrentWeaponID()].bulletType)
        {
            case Mag.BulletType.AssaultRifle:
                return assaultRifleBullet;
            case Mag.BulletType.Pistol:
                return pistolBullet;
            case Mag.BulletType.Shotgun:
                return shotgunBullet;
            case Mag.BulletType.SubmachineGun:
                return submachineGunBullet;
            case Mag.BulletType.SniperRifle:
                return sniperRifleBullet;
        }

        return null;
    }
}