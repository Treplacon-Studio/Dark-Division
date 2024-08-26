using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSoundData", menuName = "ScriptableObjects/WeaponSoundData", order = 2)]
public class WeaponSoundsSO : ScriptableObject
{
    [Header("Firing sounds")]
    public EventReference fireSound;
    public EventReference suppressedfireSound;

    [Header("Magazine sounds")]
    public EventReference reloadSound;
    public EventReference emptyClipSound;
}
