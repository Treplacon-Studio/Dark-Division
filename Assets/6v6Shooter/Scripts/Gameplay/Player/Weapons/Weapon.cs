using System;
using System.Collections.Generic;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Weapons
{
    public class Weapon
    {
        private WeaponInfo _weaponInfo;

        public Weapon(WeaponInfo.WeaponName name)
        {
            _weaponInfo = new WeaponInfo(name);
        }

        public WeaponInfo Info()
        {
            return _weaponInfo;
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
