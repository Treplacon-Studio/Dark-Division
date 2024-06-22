using UnityEngine;

public class Mag : MonoBehaviour
{
    public enum BulletType
    {
        Ammo_9mm,
        Ammo_7dot62,
        Ammo_50cal,
        Shells
    }
    
    public int size;
    public BulletType ammoType;
}
