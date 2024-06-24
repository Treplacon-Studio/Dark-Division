using UnityEngine;

public class Mag : MonoBehaviour
{
    public enum BulletType
    {
        AssaultRifle,
        Pistol,
        Shotgun,
        SubmachineGun,
        SniperRifle
    }
    
    public int size;
    public BulletType ammoType;
}
