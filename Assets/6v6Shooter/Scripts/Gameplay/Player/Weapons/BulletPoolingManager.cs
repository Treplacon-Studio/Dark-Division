using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

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
    public GameObject player;
    
    private Dictionary<int, Queue<GameObject>> _poolDictionary;

    private PhotonView _photonView;

    void Awake()
    {
        _photonView = player.GetComponent<PhotonView>();
    }
    
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

    public void SpawnFromPool(int id, Transform startPointTransform)
    {
        if (!_poolDictionary.ContainsKey(id))
        {
            Debug.LogWarning("Pool with id: " + id + " doesn't exist.");
            return;
        }

        var objectToSpawn = _poolDictionary[id].Dequeue();
        
        var bp = objectToSpawn.GetComponentInChildren<BulletPilot>();
        bp.gameObject.transform.position = startPointTransform.position;
        bp.gameObject.transform.rotation = startPointTransform.rotation;
        bp.gameObject.transform.Rotate(90, 0, 0, Space.Self);
        bp.ResetHits();
        bp.SetOwner(player);
        bp.SetRecoil(ActionsManager.Instance.Switching.WeaponComponent().gameObject.GetComponent<Recoil>());
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
        if (_photonView.IsMine is false)
            return;

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
                
                GameObject bulletTypeToSpawn = GetProperBullet();
                string bulletPrefabName = GetProperBulletPrefabName(bulletTypeToSpawn);
                GameObject newbulletObj = PhotonNetwork.Instantiate(Path.Combine("Weapons", "Bullets", $"{bulletPrefabName}"), Vector3.zero, Quaternion.identity);
                newbulletObj.transform.SetParent(ammoHolder, false);

                //Set the bullets owner photon view to this players photon view
                BulletPilot objectsPilot = newbulletObj.GetComponent<BulletPilot>();
                if (objectsPilot != null)
                    objectsPilot.SetOwnerPhotonView(_photonView);


                newbulletObj.SetActive(false);
                objectPool.Enqueue(newbulletObj);
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

    private string GetProperBulletPrefabName(GameObject bulletType)
    {
        if (bulletType == assaultRifleBullet)
            return "SM_Assaultrifle_Bullet";
        else if (bulletType == pistolBullet)
            return "SM_Pistol_Bullet";
        else if (bulletType == shotgunBullet)
            return "SM_Shotgun_Bullet";
        else if (bulletType == submachineGunBullet)
            return "SM_Sniperrifle_Bullet";
        else if (bulletType == sniperRifleBullet)
            return "SM_Submachine_Bullet";

        return null;
    }
}