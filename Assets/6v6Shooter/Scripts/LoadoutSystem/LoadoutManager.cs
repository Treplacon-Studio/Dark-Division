using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutManager : MonoBehaviour
{
    public List<GameObject> primaryWeapons;
}

[System.Serializable]
public class Loadout
{
    public int PrimaryWeaponIndex;
    public int SecondaryWeaponIndex;
    public List<int> AttachmentIndices;

    public Loadout(int primaryWeaponIndex, int secondaryWeaponIndex, List<int> attachmentIndices)
    {
        PrimaryWeaponIndex = primaryWeaponIndex;
        SecondaryWeaponIndex = secondaryWeaponIndex;
        AttachmentIndices = attachmentIndices;
    }
}
