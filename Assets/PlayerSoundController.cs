using FMODUnity;
using Photon.Pun;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    [SerializeField] private EventReference weaponShootSound;
    [SerializeField] private PhotonView photonView;

    public void PlayWeaponShootSound()
    {
        PlaySoundLocally();
    }

    private void PlaySoundLocally()
    {
        if (photonView.IsMine)
        {
            // Play the sound locally for the player
            Play3DSound(transform.position);
        }
        // Broadcast the sound event to all other clients (but not yourself)
        photonView.RPC("RPC_PlayWeaponShootSound", RpcTarget.Others, transform.position);
    }

    [PunRPC]
    public void RPC_PlayWeaponShootSound(Vector3 shooterPosition)
    {
        // Check if the RPC call is not from the local player to prevent self-hearing
        if (!photonView.IsMine)
        {
            // Play the sound at the position of the shooter (from another player)
            Play3DSound(shooterPosition);
        }
    }

    private void Play3DSound(Vector3 position)
    {
        // Create an FMOD instance for the 3D sound
        FMOD.Studio.EventInstance soundInstance = RuntimeManager.CreateInstance(weaponShootSound);

        // Set the 3D attributes of the sound instance
        soundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

        // Start playing the sound
        soundInstance.start();

        // Release the instance after it finishes playing
        soundInstance.release();
    }
}
