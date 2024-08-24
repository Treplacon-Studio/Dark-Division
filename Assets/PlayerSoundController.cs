using FMODUnity;
using Photon.Pun;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    [SerializeField] private RemoteRPCSound remoteRPCSound;

    public void PlayWeaponShootSound()
    {
        Debug.Log("Shoot animation called 1x");
        remoteRPCSound.PlaySoundLocally();
    }
}
