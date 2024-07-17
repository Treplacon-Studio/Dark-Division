using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LoadoutManager : MonoBehaviour
{
    // Playerprefs Information
    private const string LoadoutKey = "Loadout_";
    private const int MaxLoadouts = 6;

    [Header("UI ELEMENTS")]
    public GameObject SelectedLoadoutPreviewPanel;
    public TextMeshProUGUI PrimaryWeaponText;
    public TextMeshProUGUI SecondaryWeaponText;
    

    public List<Loadout> PlayerLoadouts { get; set; }

    void Start()
    {
        SelectedLoadoutPreviewPanel.SetActive(false);
        PlayerLoadouts = LoadAllLoadouts();
    }

    public void SaveLoadout(int slot, Loadout loadout)
    {
        if (slot < 0 || slot >= MaxLoadouts)
            return;

        string json = JsonUtility.ToJson(loadout);
        PlayerPrefs.SetString(LoadoutKey + slot, json);
        PlayerPrefs.Save();
    }

    public Loadout LoadLoadout(int slot)
    {
        if (slot < 0 || slot >= MaxLoadouts)
            return null;

        string json = PlayerPrefs.GetString(LoadoutKey + slot, string.Empty);
        if (string.IsNullOrEmpty(json))
            return null;

        return JsonUtility.FromJson<Loadout>(json);
    }
    
    public List<Loadout> LoadAllLoadouts()
    {
        List<Loadout> loadouts = new();

        for (int i = 0; i < MaxLoadouts; i++)
        {
            string json = PlayerPrefs.GetString(LoadoutKey + i, string.Empty);
            if (string.IsNullOrEmpty(json))
            {
                //If no loadout saved in this slot, create and save a default loadout
                Loadout defaultLoadout = GetDefaultLoadout();
                SaveLoadout(i, defaultLoadout);
                loadouts.Add(defaultLoadout);
            }
            else
            {
                Loadout loadout = JsonUtility.FromJson<Loadout>(json);
                loadouts.Add(loadout);
            }
        }

        return loadouts;
    }

    public void LoadLoadoutByIndex(int loadoutIndex)
    {
        string loadoutJson = PlayerPrefs.GetString(LoadoutKey + loadoutIndex, string.Empty);
        if (string.IsNullOrEmpty(loadoutJson))
            return;

        Loadout loadout = JsonUtility.FromJson<Loadout>(loadoutJson);
        ApplyLoadout(loadout);
    }

    private void ApplyLoadout(Loadout loadout)
    {
        Debug.Log("Applying Loadout:");
        Debug.Log("Primary Weapon: " + loadout.PrimaryWeapon);
        Debug.Log("Secondary Weapon: " + loadout.SecondaryWeapon);
        //Debug.Log("Attachments: " + string.Join(", ", loadout.Attachments));
        
        PrimaryWeaponText.text = loadout.PrimaryWeapon;
        PrimaryWeaponText.text = loadout.SecondaryWeapon;
    }

    private Loadout GetDefaultLoadout()
    {
        return new Loadout("SCAR-Enforcer 557", "TAC - 45", new List<string> { "None", "None" });
    }

    public void ClearLoadout(int slot)
    {
        if (slot < 0 || slot >= MaxLoadouts)
            return;

        PlayerPrefs.DeleteKey(LoadoutKey + slot);
        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class WeaponCategory
{
    public string categoryName;
    public List<GameObject> weapons;
}

[System.Serializable]
public class Loadout
{
    public string PrimaryWeapon;
    public string SecondaryWeapon;
    public List<string> Attachments;

    public Loadout(string primaryWeapon, string secondaryWeapon, List<string> attachments)
    {
        PrimaryWeapon = primaryWeapon;
        SecondaryWeapon = secondaryWeapon;
        Attachments = attachments;
    }
}

[System.Serializable]
public class PrimaryWeapons
{
    public string[] ListOfPrimaryWeapons = {"SCAR-Enforcer 557", "M4A1-Sentinel 254", "Vector-Ghost500", "VEL-Ironclad 308", "Balista Vortex", "DSR - 50", "Gauge 320", "Stoeger - 22"};
}

[System.Serializable]
public class SecondaryWeapons
{
    public string[] ListOfSecondaryWeapons = {"TAC - 45", "FN Five - Eight"};
}

[System.Serializable]
public class MeleeWeapons
{
    public string[] ListOfMeleeWeapons = {"Default Knife", "Butterfly Knife", "Battle Axe", "Katana", "Plasma Sword"};
}

[System.Serializable]
public class LethalWeapons
{
    
}

[System.Serializable]
public class TacticalWeapons
{
    
}