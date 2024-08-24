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
        Debug.Log("Play Weapon Sound");
        AudioManager.Instance.PlayOneShot(weaponShootSound, transform.position);
    }
}
