using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThrowableSO", menuName = "ScriptableObjects/ThrowableSO")]
public class ThrowableSO : ScriptableObject
{
    [Header("Prefab references")]
    public GameObject dummyPrefab;
    public GameObject actualPrefab;

    [Header("Prefab variables")]
    public float ThrowForce;
    public float ThrowUpwardForce;

    public bool isLethal; 
    public bool isThrowingKnife; // To distinguish if the throwable is a knife
    public int ammoCount;
    public float coolDownTime;

    public bool HasAmmo() => ammoCount > 0;
    public void ConsumeAmmo() => ammoCount--;
}
