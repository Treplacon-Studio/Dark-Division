using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType {
    Primary,
    Secondary,
    Melee
}

public enum Weapon {
    SCAREnforcer557,
    M4A1Sentinel254,
    VectorGhost500,
    VELIronclad308,
    BalistaVortex,
    DSR50,
    Gauge320,
    Stoeger22,
    TAC45,
    FNFive8,
    DefaultKnife,
    ButterflyKnife,
    BattleAxe,
    Katana,
    PlasmaSword
}

public class WeaponManager : MonoBehaviour
{
    public WeaponType currentWeaponType;
    public Weapon currentWeapon;

    public Transform firePoint;
    public string bulletType;

    public float damage = 10f;
	public float range = 100f;
	public float fireRate = 15f;
	float nextTimeToFire = 0f;

    private BulletPoolingManager bulletPool;

    void Awake()
    {
        currentWeaponType = WeaponType.Primary;
        currentWeapon = Weapon.SCAREnforcer557;

        bulletPool = GetComponent<BulletPoolingManager>();
        if (bulletPool == null)
            Debug.Log("Cannot find the BulletPoolingManager script!");
    }

    void Update()
    {
        if (currentWeapon == Weapon.SCAREnforcer557 && Time.time >= nextTimeToFire)
        {
            if (Input.GetButton("Fire1"))
            {
                nextTimeToFire = Time.time + 3.2f/fireRate;
                Shoot();
            }
        }        
    }

    void Shoot()
    {
        bulletPool.SpawnFromPool(bulletType, firePoint.position, firePoint.rotation);
    }
}
