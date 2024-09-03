using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMuzzleFlash : MonoBehaviour
{
    private Shooting shootingScript;
    [SerializeField] private ParticleSystem muzzleFlash;

    private void OnEnable()
    {
        shootingScript = FindObjectOfType<Shooting>();
        if (shootingScript != null)
        {
            shootingScript.OnShoot += PlayMuzzleOnce;
        }
        else
        {
            Debug.Log("Paniek");
        }
    }

    private void OnDisable()
    {
        if (shootingScript != null)
        {
            shootingScript.OnShoot -= PlayMuzzleOnce;
        }
    }

    public void PlayMuzzleOnce()
    {
        Debug.Log("Playing muzzle");
        muzzleFlash?.Play();
    }
}
