using FMODUnity;
using Photon.Pun;
using UnityEngine;

public class PlayerSoundController : MonoBehaviourPunCallbacks
{
    [SerializeField] private RemoteRPCSound remoteRPCSound;

    private void Start()
    {
        // Check if this instance of the player is the local player
        if (photonView.IsMine)
        {
            // This is the local player, so add the FMOD Studio Listener
            gameObject.AddComponent<StudioListener>();
        }
    }

    public void PlayWeaponShootSound()
    {
        Debug.Log("Shoot animation called 1x");
        remoteRPCSound.PlaySound();
    }
}
