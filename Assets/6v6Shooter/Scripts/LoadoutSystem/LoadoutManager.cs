using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadoutManager : MonoBehaviour
{
    // Playerprefs Information
    private const string LoadoutKey = "Loadout_";
    private const int MaxLoadouts = 6;

    [Header("UI ELEMENTS")]
    public GameObject SelectedLoadoutPreviewPanel;
    public TextMeshProUGUI PrimaryWeaponText;
    public TextMeshProUGUI SecondaryWeaponText;
    public TextMeshProUGUI PrimaryWeaponTypeText;
    public TextMeshProUGUI SecondaryWeaponTypeText;

    [Header("WEAPON ITEMS")]
    public List<WeaponItem> PrimaryWeaponItems;
    public List<WeaponItem> SecondaryWeaponItems;
    
    [Header("LOADOUTS")]
    public List<Loadout> PlayerLoadouts;

    [Header("LOADOUT RENAMING")]
    public GameObject RenameLoadoutModal;
    public Button[] LoadoutButtons;
    private Button _hoveredButton;

    void Start()
    {
        ClearAllLoadouts();

        SetLoadoutButtonsForRenaming();

        RenameLoadoutModal.SetActive(false);
        SelectedLoadoutPreviewPanel.SetActive(false);
        PlayerLoadouts = LoadAllLoadouts();
    }

    void Update()
    {
        UpdateHoveredButton();
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

        SelectedLoadoutPreviewPanel.SetActive(true);
    }

    private void ApplyLoadout(Loadout loadout)
    {
        Debug.Log("Applying Loadout:");
        Debug.Log("Primary Weapon: " + loadout.PrimaryWeapon.WeaponName);
        Debug.Log("Secondary Weapon: " + loadout.SecondaryWeapon.WeaponName);
        
        PrimaryWeaponText.text = loadout.PrimaryWeapon.WeaponName;
        SecondaryWeaponText.text = loadout.SecondaryWeapon.WeaponName;
    }

    private Loadout GetDefaultLoadout()
    {
        WeaponItem defaultPrimaryWeapon = PrimaryWeaponItems[0];
        WeaponItem defaultSecondaryWeapon = SecondaryWeaponItems[0];

        return new Loadout(
            defaultPrimaryWeapon,
            defaultSecondaryWeapon
        );
    }

    public void ClearLoadout(int slot)
    {
        if (slot < 0 || slot >= MaxLoadouts)
            return;

        PlayerPrefs.DeleteKey(LoadoutKey + slot);
        PlayerPrefs.Save();
    }

    public void ClearAllLoadouts()
    {
        for (int i = 0; i < MaxLoadouts; i++)
        {
            PlayerPrefs.DeleteKey(LoadoutKey + i);
        }
        PlayerPrefs.Save();
    }

    #region  RENAMING LOADOUT

    private void SetLoadoutButtonsForRenaming()
    {
        foreach (Button button in LoadoutButtons)
        {
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((eventData) => { OnPointerEnter(button); });
            trigger.triggers.Add(entry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((eventData) => { OnPointerExit(); });
            trigger.triggers.Add(exitEntry);
        }
    }

    private void UpdateHoveredButton()
    {
        if (_hoveredButton != null && Input.GetKeyDown(KeyCode.R))
            ShowRenameLoadoutModal();
    }

    private void OnPointerEnter(Button button)
    {
        _hoveredButton = button;
    }

    private void OnPointerExit()
    {
        _hoveredButton = null;
    }

    private void ShowRenameLoadoutModal()
    {
        RenameLoadoutModal.SetActive(true);
    }

    #endregion
}

[System.Serializable]
public class Loadout
{
    public WeaponItem PrimaryWeapon;
    public WeaponItem SecondaryWeapon;

    public Loadout(WeaponItem primaryWeapon, WeaponItem secondaryWeapon)
    {
        PrimaryWeapon = primaryWeapon;
        SecondaryWeapon = secondaryWeapon;
    }
}

[System.Serializable]
public class WeaponItem
{
    public string WeaponName;                                             //Name that is shown
    public string WeaponPrefabName;                                       //Name of prefab
    public WeaponType WeaponTypeItem = WeaponType.None;                   //Assault, Submachine, etc
    public WeaponCategory WeaponCategoryItem = WeaponCategory.None;       //Primary/Secondary/Melee/None

    public enum WeaponCategory
    {
        Primary,
        Secondary, 
        Melee,
        None
    }

    public enum WeaponType
    {
        Assault,
        Submachine,
        Sniper,
        Shotgun,
        Pistol,
        None
    }

    public WeaponItem(string weaponName, string weaponPrefabName, WeaponType weaponTypeItem, WeaponCategory weaponCategoryItem)
    {
        WeaponName = weaponName;
        WeaponPrefabName = weaponPrefabName;
        WeaponTypeItem = weaponTypeItem;
        WeaponCategoryItem = weaponCategoryItem;
    }
}