using FMODUnity;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    [SerializeField] private EventReference weaponShootSound;
    [SerializeField] private PhotonView photonView;

    public void PlayWeaponShootSound()
    {
        // Check if this is the local player
        if (photonView.IsMine)
        {
            // Play shooting sound only locally
            Debug.Log("Play Weapon Sound Locally");
            AudioManager.Instance.PlayOneShot(weaponShootSound, transform.position);
        }
    }
}
