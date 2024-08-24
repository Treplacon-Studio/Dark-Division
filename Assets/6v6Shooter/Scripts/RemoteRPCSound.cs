using FMODUnity;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteRPCSound : MonoBehaviourPunCallbacks
{
    [SerializeField] private EventReference weaponShootSound;

    public void PlaySound()
    {
        if (photonView.IsMine)
        {
            Play3DSound(transform.position);
            photonView.RPC("RPC_PlayWeaponShootSound", RpcTarget.Others, transform.position);
        }
    }

    [PunRPC]
    public void RPC_PlayWeaponShootSound(Vector3 shooterPosition)
    {
        if (!photonView.IsMine)
            Play3DSound(shooterPosition);
    }

    public void Play3DSound(Vector3 position)
    {
        FMOD.Studio.EventInstance soundInstance = RuntimeManager.CreateInstance(weaponShootSound);
        soundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        soundInstance.start();
        StartCoroutine(ReleaseSoundInstance(soundInstance));
    }

    private IEnumerator ReleaseSoundInstance(FMOD.Studio.EventInstance instance)
    {
        yield return new WaitForSeconds(0.01f);
        instance.release();
    }
}
