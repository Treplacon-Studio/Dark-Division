using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace _6v6Shooter.Scripts.Gameplay.Player.Weapons
{
    public class Weapon : MonoBehaviour
    {
        private WeaponInfo _weaponInfo;
        
        [SerializeField] [Tooltip("Weapon name.")]
        private WeaponInfo.WeaponName weaponName;
        
        [SerializeField] [Tooltip("Bullet start point game object.")]
        private GameObject bulletStartPoint;
        
        [SerializeField] [Tooltip("Mag socket of the weapon.")]
        private GameObject magSocket;
        
        [SerializeField] [Tooltip("Weapon mags attachments.")]
        private GameObject[] mags;
        
        [SerializeField] [Tooltip("Weapon barrels attachments.")]
        private GameObject[] barrels;
        
        [SerializeField] [Tooltip("Weapon under barrels attachments.")]
        private GameObject[] underBarrels;
        
        [SerializeField] [Tooltip("Weapon sights attachments.")]
        private GameObject[] sights;
        
        [SerializeField] [Tooltip("Weapon stocks attachments.")]
        private GameObject[] stocks;

        private GameObject _mag;
        private GameObject _barrel;
        private GameObject _underBarrel;
        private GameObject _sight;
        private GameObject _stock;
        
        public void Awake()
        {
            _weaponInfo ??= new WeaponInfo(weaponName);
            ResetAttachments();
            ApplyAttachmentsAssaultRifle(0, -1, -1, -1, -1);
        }

        public WeaponInfo Info()
        {
            return _weaponInfo ??= new WeaponInfo(weaponName);
        }
        
        public GameObject GetMag()
        {
            return _mag;
        }

        public GameObject GetMagSocket()
        {
            return magSocket;
        }

        public void ApplyAttachmentsAssaultRifle(int aMag = -1, int aBarrel = -1, int aUnderBarrel = -1, int aSight = -1, int aStock = -1)
        {
            HandleMagChange(aMag);
            HandleBarrelChange(aBarrel);
            HandleSightChange(aSight);
            HandleStockChange(aStock);
        }

        public GameObject GetStartPoint()
        {
            return bulletStartPoint;
        }

        private void ResetAttachments()
        {
            foreach(var o in mags) o.SetActive(false);
            foreach(var o in barrels) o.SetActive(false);
            foreach(var o in sights) o.SetActive(false);
            foreach(var o in stocks) o.SetActive(false);
        }

        private void HandleMagChange(int ind)
        {
            foreach(var m in mags)
                m.SetActive(false);
            
            if (ind == -1) 
                return;
            
            if(ind >= mags.Length)
                Debug.LogError($"There is no mag with index given: {ind}.");

            mags[ind].SetActive(true);
            _mag = mags[ind];
        }
        
        private void HandleBarrelChange(int ind)
        {
            foreach(var m in barrels)
                m.SetActive(false);
            
            if (ind == -1) 
                return;
            
            if(ind >= barrels.Length)
                Debug.LogError($"There is no barrel with index given: {ind}.");

            barrels[ind].SetActive(true);
            _barrel = barrels[ind];
        }
        
        private void HandleUnderBarrelChange(int ind)
        {
            foreach(var m in underBarrels)
                m.SetActive(false);
            
            if (ind == -1) 
                return;
            
            if(ind >= underBarrels.Length)
                Debug.LogError($"There is no under barrel with index given: {ind}.");

            underBarrels[ind].SetActive(true);
            _underBarrel = underBarrels[ind];
        }
        
        private void HandleSightChange(int ind)
        {
            foreach(var m in sights)
                m.SetActive(false);
            
            if (ind == -1) 
                return;
            
            if(ind >= sights.Length)
                Debug.LogError($"There is no sight with index given: {ind}.");

            sights[ind].SetActive(true);
            _sight = sights[ind];
        }
        
        private void HandleStockChange(int ind)
        {
            foreach(var m in stocks)
                m.SetActive(false);
            
            if (ind == -1) 
                return;
            
            if(ind >= stocks.Length)
                Debug.LogError($"There is no stock with index given: {ind}.");

            stocks[ind].SetActive(true);
            _stock = stocks[ind];
        }
    }
    
    
    
    public class WeaponInfo
    {
        //All weapons available in game
        public enum WeaponName {
            ScarEnforcer557,
            M4A1Sentinel254,
            VectorGhost500,
            VelIronclad308,
            BalistaVortex,
            Dsr50,
            Gauge320,
            Stoeger22,
            Tac45,
            FnFive8,
            DefaultKnife,
            ButterflyKnife,
            BattleAxe,
            Katana,
            PlasmaSword
        }
        
        //Mapping weapon to it's type
        private Dictionary<WeaponName, WeaponStats> _weaponMapping = new()
        {
            { WeaponName.ScarEnforcer557, new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Primary, WeaponStats.BulletType.Ammo_9mm) },
            { WeaponName.M4A1Sentinel254, new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Primary, WeaponStats.BulletType.Ammo_9mm) },
            { WeaponName.VectorGhost500, new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Primary, WeaponStats.BulletType.Ammo_9mm) },
            { WeaponName.VelIronclad308, new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Primary, WeaponStats.BulletType.Ammo_9mm) },
            { WeaponName.BalistaVortex, new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Primary, WeaponStats.BulletType.Ammo_9mm) },
            { WeaponName.Dsr50, new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Primary, WeaponStats.BulletType.Ammo_9mm) },
            { WeaponName.Gauge320, new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Secondary, WeaponStats.BulletType.Ammo_9mm) },
            { WeaponName.Stoeger22, new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Secondary, WeaponStats.BulletType.Ammo_9mm) },
            { WeaponName.Tac45, new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Secondary, WeaponStats.BulletType.Ammo_9mm) },
            { WeaponName.FnFive8, new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Secondary, WeaponStats.BulletType.Ammo_9mm) },
            { WeaponName.DefaultKnife, new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Melee, WeaponStats.BulletType.Ammo_9mm) },
            { WeaponName.ButterflyKnife, new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Melee, WeaponStats.BulletType.Ammo_9mm) },
            { WeaponName.BattleAxe, new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Melee, WeaponStats.BulletType.Ammo_9mm) },
            { WeaponName.Katana, new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Melee, WeaponStats.BulletType.Ammo_9mm) },
            { WeaponName.PlasmaSword, new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Melee, WeaponStats.BulletType.Ammo_9mm) }
        };
        
        private WeaponName _weaponName;
        private WeaponStats _weaponStats;

        public WeaponInfo(WeaponName name)
        {
            _weaponName = name;
            _weaponStats = GetWeaponStats(name);
        }
        
        private WeaponStats GetWeaponStats(WeaponName weaponName)
        {
            if (_weaponMapping.TryGetValue(weaponName, out var weaponStats))
                return weaponStats;
            throw new ArgumentException("Invalid weapon name: ", nameof(weaponName));
        }
        
        public WeaponStats Stats()
        {
            return _weaponStats;
        }

        public WeaponName Name()
        {
            return _weaponName;
        }
    }

    
    
    public struct WeaponStats
    {
        public enum WeaponType {
            Primary,
            Secondary,
            Melee
        }

        public enum BulletType
        {
            Ammo_9mm,
            Ammo_7dot62,
            Ammo_50cal, 
            Shells
        }
        
        public float Damage;
        public float Range;
        public float FireRate;
        public float BulletSpeed;
        public BulletType BType;
        public WeaponType WType;
        
        public WeaponStats(float damage, float range, float fireRate, float bulletSpeed, WeaponType wType, BulletType bType)
        {
            Damage = damage;
            Range = range;
            FireRate = fireRate;
            BulletSpeed = bulletSpeed;
            WType = wType;
            BType = bType;
        }
    }
}
