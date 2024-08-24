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
        if (photonView.IsMine)
        {
            // Play the sound locally for the shooter (you)
            Debug.Log("Play Weapon Sound Locally");
            AudioManager.Instance.PlayOneShot(weaponShootSound, transform.position);

            // Broadcast the sound event to all other clients
            photonView.RPC("RPC_PlayWeaponShootSound", RpcTarget.Others, transform.position);
        }
    }

    [PunRPC]
    public void RPC_PlayWeaponShootSound(Vector3 shooterPosition)
    {
        // Play the sound at the position of the shooter (enemy or self)
        AudioManager.Instance.PlayOneShot(weaponShootSound, shooterPosition);
    }
}
