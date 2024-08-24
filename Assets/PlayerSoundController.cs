using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    [SerializeField] private EventReference weaponShootSound;

    public void PlayWeaponShootSound()
    {
        //Play shooting sound
        AudioManager.Instance.PlayOneShot(weaponShootSound, transform.position);
    }
}
