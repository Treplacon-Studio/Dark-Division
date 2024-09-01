using System;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private WeaponInfo _weaponInfo;

    [SerializeField] [Tooltip("Weapon name.")]
    private WeaponInfo.WeaponName weaponName;

    [SerializeField] [Tooltip("Scriptable Object for weapon attachments.")]
    private WeaponAttachments weaponAttachments;

    [SerializeField] [Tooltip("Bullet start point game object.")]
    private GameObject bulletStartPoint;

    [Space]

    [SerializeField] private GameObject barrelSocket;
    [SerializeField] private GameObject underbarrelSocket;
    [SerializeField] private GameObject magSocket;
    [SerializeField] private GameObject stockSocket;
    [SerializeField] private GameObject sightSocket;
    [SerializeField] private GameObject standSocket;

    [Space]

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

    [SerializeField] [Tooltip("Weapon stands attachments (snipers only)")]
    private GameObject[] stands;

    private GameObject _mag;
    private GameObject _barrel;
    private GameObject _underBarrel;
    private GameObject _sight;
    private GameObject _stock;
    private GameObject _stand;

    public void Awake()
    {
        _weaponInfo ??= new WeaponInfo(weaponName);
        ResetAttachments();
        ApplyAttachmentsAssaultRifle(null, 0);
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

    private bool HasStand()
    {
        return weaponName is WeaponInfo.WeaponName.Dsr50 or WeaponInfo.WeaponName.BalistaVortex;
    }

    public void ApplyAttachmentsAssaultRifle(int[,] attachments, int index)
    {
        if (weaponAttachments == null)
        {
            Debug.LogError("WeaponAttachments ScriptableObject is not assigned.");
            return;
        }

        ResetAttachments();

        if (attachments == null)
        {
            if (magSocket != null) HandleMagChange(0);
            if (barrelSocket != null) HandleBarrelChange(-1);
            if (sightSocket != null) HandleSightChange(-1);
            if (stockSocket != null) HandleStockChange(-1);
            if (underbarrelSocket != null) HandleUnderBarrelChange(-1);

            if (HasStand() && standSocket != null)
                HandleStandChange(-1);
        }
        else
        {
            if (magSocket != null) HandleMagChange(attachments[index, 0]);
            if (barrelSocket != null) HandleBarrelChange(attachments[index, 1]);
            if (sightSocket != null) HandleSightChange(attachments[index, 2]);
            if (stockSocket != null) HandleStockChange(attachments[index, 3]);
            if (underbarrelSocket != null) HandleUnderBarrelChange(attachments[index, 4]);

            if (HasStand() && standSocket != null)
                HandleStandChange(attachments[index, 5]);
        }
    }


    public GameObject GetStartPoint()
    {
        return bulletStartPoint;
    }

    private void ResetAttachments()
    {
        if (weaponAttachments == null) return;

        if (magSocket != null)
        {
            foreach (var o in weaponAttachments.mags) o.SetActive(false);
        }

        if (barrelSocket != null)
        {
            foreach (var o in weaponAttachments.barrels) o.SetActive(false);
        }

        if (underbarrelSocket != null)
        {
            foreach (var o in weaponAttachments.underBarrels) o.SetActive(false);
        }

        if (sightSocket != null)
        {
            foreach (var o in weaponAttachments.sights) o.SetActive(false);
        }

        if (stockSocket != null)
        {
            foreach (var o in weaponAttachments.stocks) o.SetActive(false);
        }

        if (standSocket != null)
        {
            foreach (var o in weaponAttachments.stands) o.SetActive(false);
        }
    }


    private void HandleMagChange(int ind)
    {
        if (weaponAttachments == null || magSocket == null) return;

        foreach (var m in weaponAttachments.mags)
            m.SetActive(false);

        if (ind == -1 || ind >= weaponAttachments.mags.Length)
            return;

        weaponAttachments.mags[ind].SetActive(true);
        _mag = weaponAttachments.mags[ind];
    }


    private void HandleBarrelChange(int ind)
    {
        if (weaponAttachments == null || barrelSocket == null) return;

        foreach (var m in weaponAttachments.barrels)
            m.SetActive(false);

        if (ind == -1 || ind >= weaponAttachments.barrels.Length)
            return;

        weaponAttachments.barrels[ind].SetActive(true);
        _barrel = weaponAttachments.barrels[ind];
    }


    private void HandleUnderBarrelChange(int ind)
    {
        if (weaponAttachments == null || underbarrelSocket == null) return;

        foreach (var m in weaponAttachments.underBarrels)
            m.SetActive(false);

        if (ind == -1 || ind >= weaponAttachments.underBarrels.Length)
            return;

        weaponAttachments.underBarrels[ind].SetActive(true);
        _underBarrel = weaponAttachments.underBarrels[ind];
    }

    private void HandleSightChange(int ind)
    {
        if (weaponAttachments == null || sightSocket == null) return;

        foreach (var m in weaponAttachments.sights)
            m.SetActive(false);

        if (ind == -1 || ind >= weaponAttachments.sights.Length)
            return;

        weaponAttachments.sights[ind].SetActive(true);
        _sight = weaponAttachments.sights[ind];
    }


    private void HandleStockChange(int ind)
    {
        if (weaponAttachments == null || stockSocket == null) return;

        foreach (var m in weaponAttachments.stocks)
            m.SetActive(false);

        if (ind == -1 || ind >= weaponAttachments.stocks.Length)
            return;

        weaponAttachments.stocks[ind].SetActive(true);
        _stock = weaponAttachments.stocks[ind];
    }


    private void HandleStandChange(int ind)
    {
        if (weaponAttachments == null || standSocket == null) return;

        foreach (var m in weaponAttachments.stands)
            m.SetActive(false);

        if (ind == -1 || ind >= weaponAttachments.stands.Length)
            return;

        weaponAttachments.stands[ind].SetActive(true);
        _stand = weaponAttachments.stands[ind];
    }
}



    public class WeaponInfo
{
    //All weapons available in game
    public enum WeaponName
    {
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
        {
            WeaponName.ScarEnforcer557,
            new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Primary)
        },
        {
            WeaponName.M4A1Sentinel254,
            new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Primary)
        },
        {
            WeaponName.VectorGhost500,
            new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Primary)
        },
        {
            WeaponName.VelIronclad308,
            new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Primary)
        },
        {
            WeaponName.BalistaVortex,
            new WeaponStats(40, 10, 0.1f, 10, WeaponStats.WeaponType.Primary)
        },
        {
            WeaponName.Dsr50,
            new WeaponStats(40, 10, 0.1f, 10, WeaponStats.WeaponType.Primary)
        },
        {
            WeaponName.Gauge320,
            new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Secondary)
        },
        {
            WeaponName.Stoeger22,
            new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Secondary)
        },
        {
            WeaponName.Tac45,
            new WeaponStats(10, 10, 0.5f, 10, WeaponStats.WeaponType.Secondary)
        },
        {
            WeaponName.FnFive8,
            new WeaponStats(10, 10, 0.5f, 50, WeaponStats.WeaponType.Secondary)
        },
        {
            WeaponName.DefaultKnife,
            new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Melee)
        },
        {
            WeaponName.ButterflyKnife,
            new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Melee)
        },
        {
            WeaponName.BattleAxe,
            new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Melee)
        },
        {
            WeaponName.Katana,
            new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Melee)
        },
        {
            WeaponName.PlasmaSword,
            new WeaponStats(10, 10, 0.1f, 10, WeaponStats.WeaponType.Melee)
        }
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

    public void UpdateStats(WeaponStats weaponStats)
    {
        _weaponMapping[_weaponName] = weaponStats;
        _weaponStats = GetWeaponStats(_weaponName);
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
    public enum WeaponType
    {
        Primary,
        Secondary,
        Melee
    }
    
    public float Damage;
    public float Range;
    public float FireRate;
    public float BulletSpeed;
    public WeaponType WType;

    public WeaponStats(float damage, float range, float fireRate, float bulletSpeed, WeaponType wType)
    {
        Damage = damage;
        Range = range;
        FireRate = fireRate;
        BulletSpeed = bulletSpeed;
        WType = wType;
    }
}

[Serializable]
public class WeaponAnimation
{
    public WeaponInfo.WeaponName name;
    public AnimationClip clip;
}