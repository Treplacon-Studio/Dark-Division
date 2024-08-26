using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAudio : MonoBehaviour
{
    public WeaponSoundsSO soundData;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = AudioManager.Instance; // Get the AudioManager instance
    }

    public void PlayFireSound(Vector3 position)
    {
        if (soundData != null && soundData.fireSound.IsNull == false)
        {
            audioManager.PlayOneShot(soundData.fireSound, position);
        }
    }

    public void PlayReloadSound(Vector3 position)
    {
        if (soundData != null && soundData.reloadSound.IsNull == false)
        {
            audioManager.PlayOneShot(soundData.reloadSound, position);
        }
    }

    public void PlayEmptyClipSound(Vector3 position)
    {
        if (soundData != null && soundData.emptyClipSound.IsNull == false)
        {
            audioManager.PlayOneShot(soundData.emptyClipSound, position);
        }
    }
}
