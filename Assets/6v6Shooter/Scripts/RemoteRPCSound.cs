using FMODUnity;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class RemoteRPCSound : MonoBehaviourPunCallbacks
{
    [SerializeField] private EventReference weaponShootSound;

    // Visualization parameters
    public float minDistance = 5f; // Distance at which sound starts attenuating
    public float maxDistance = 20f; // Distance at which sound is inaudible
    public Color minDistanceColor = Color.green; // Color for minimum distance sphere
    public Color maxDistanceColor = Color.red; // Color for maximum distance sphere
    public float debugSphereRadius = 0.1f; // Radius of the debug spheres

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

    private void OnDrawGizmos()
    {
        // Draw spheres around the position where the sound is played
        Gizmos.color = minDistanceColor;
        Gizmos.DrawWireSphere(transform.position, minDistance);

        Gizmos.color = maxDistanceColor;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw filled spheres for better visualization when selected
        Gizmos.color = minDistanceColor;
        Gizmos.DrawSphere(transform.position, debugSphereRadius);

        Gizmos.color = maxDistanceColor;
        Gizmos.DrawSphere(transform.position, debugSphereRadius);
    }
}
