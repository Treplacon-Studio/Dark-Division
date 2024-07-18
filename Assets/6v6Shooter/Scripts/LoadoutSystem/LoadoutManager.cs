using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadoutManager : MonoBehaviour
{
    // Playerprefs Information
    private const string LoadoutKey = "Loadout_";
    private const int MaxLoadouts = 6;

    public MM_CinemaCC cameraManager;
    public MainMenuManager MainMenuManager;

    [Header("UI ELEMENTS")]
    public GameObject SelectedLoadoutPreviewPanel;
    public GameObject SelectWeaponButtonContainer;
    public GameObject SelectWeaponButtonPrefab;
    public TextMeshProUGUI PrimaryWeaponText;
    public TextMeshProUGUI SecondaryWeaponText;
    public TextMeshProUGUI PrimaryWeaponTypeText;
    public TextMeshProUGUI SecondaryWeaponTypeText;

    [Header("LOADOUT RENAMING")]
    public GameObject RenameLoadoutModal;
    public GameObject LoadoutButtonPrefab;
    public Transform LoadoutButtonsContainer;
    public Button SubmitButton;
    public TMP_InputField LoadoutRenameInputField;
    private GameObject _hoveredButton;
    private GameObject _lastHoveredButton;
    private int _currentRenameLoadoutIndex;
    private GameObject[] LoadoutButtons;

    [Header("INSTANTIATE POSITION")]
    public Transform WeaponSpawnPoint;
    private GameObject _lastWeaponShown;

    [Header("WEAPON ITEMS")]
    public List<WeaponItem> PrimaryWeaponItems;
    public List<WeaponItem> SecondaryWeaponItems;
    
    [Header("LOADOUTS")]
    public List<Loadout> PlayerLoadouts;


    void Start()
    {
        //ClearAllLoadouts(); //This is just for testing

        PlayerLoadouts = LoadAllLoadouts();
        LoadoutButtons = new GameObject[MaxLoadouts];
        InstantiateLoadoutButtons();

        SetLoadoutButtonsForRenaming();

        RenameLoadoutModal.SetActive(false);
        SelectedLoadoutPreviewPanel.SetActive(false);
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
                Loadout defaultLoadout = GetDefaultLoadout(i + 1);
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

        //Instantiate the primary weapon prefab
        InstantiateWeapon(loadout.PrimaryWeapon.WeaponPrefabName);
    }

    private Loadout GetDefaultLoadout(int index)
    {
        WeaponItem defaultPrimaryWeapon = PrimaryWeaponItems[0];
        WeaponItem defaultSecondaryWeapon = SecondaryWeaponItems[0];

        return new Loadout(
            "Loadout " + index,
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

    public void OnCancelRename() => RenameLoadoutModal.SetActive(false);

    private void SetLoadoutButtonsForRenaming()
    {
        SubmitButton.onClick.AddListener(OnRenameLoadoutClicked);

        foreach (GameObject button in LoadoutButtons)
        {
            EventTrigger trigger = button.AddComponent<EventTrigger>();

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

    private void OnPointerEnter(GameObject button)
    {
        _hoveredButton = button;
        _lastHoveredButton = _hoveredButton;
    }

    private void OnPointerExit()
    {
        _hoveredButton = null;
    }

    private void ShowRenameLoadoutModal()
    {
        for (int i = 0; i < LoadoutButtons.Length; i++)
        {
            if (LoadoutButtons[i] == _lastHoveredButton)
            {
                _currentRenameLoadoutIndex = i;
                break;
            }
        }

        //Set the text field to the current loadout name
        LoadoutRenameInputField.text = PlayerLoadouts[_currentRenameLoadoutIndex].LoadoutName;

        RenameLoadoutModal.SetActive(true);
    }

    private void OnRenameLoadoutClicked()
    {
        TextMeshProUGUI loadoutNameText = _lastHoveredButton.GetComponentInChildren<TextMeshProUGUI>();
        loadoutNameText.text = LoadoutRenameInputField.text;

        //Update the loadout name
        PlayerLoadouts[_currentRenameLoadoutIndex].LoadoutName = LoadoutRenameInputField.text;

        SaveLoadout(_currentRenameLoadoutIndex, PlayerLoadouts[_currentRenameLoadoutIndex]);

        RenameLoadoutModal.SetActive(false);
    }

    private void InstantiateLoadoutButtons()
    {
        for (int i = 0; i < PlayerLoadouts.Count; i++)
        {
            GameObject button = Instantiate(LoadoutButtonPrefab, LoadoutButtonsContainer);
            TextMeshProUGUI loadoutNameText = button.GetComponentInChildren<TextMeshProUGUI>();
            loadoutNameText.text = PlayerLoadouts[i].LoadoutName;
            
            //Set button event trigger for renaming
            EventTrigger trigger = button.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((eventData) => { OnPointerEnter(button); });
            trigger.triggers.Add(entry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((eventData) => { OnPointerExit(); });
            trigger.triggers.Add(exitEntry);

            //istener to load the loadout
            Button btnComponent = button.GetComponent<Button>();
            int loadoutIndex = i;
            btnComponent.onClick.AddListener(() => LoadLoadoutByIndex(loadoutIndex));

            LoadoutButtons[i] = button;
        }
    }

    #endregion

    #region  WEAPON PREFAB DISPLAY

    private void InstantiateWeapon(string weaponPrefabName)
    {
        if (_lastWeaponShown != null)
            Destroy(_lastWeaponShown);

        GameObject weaponPrefab = Resources.Load<GameObject>($"Weapons/LoadoutWeaponPrefabs/{weaponPrefabName}");
        if (weaponPrefab != null)
            _lastWeaponShown = Instantiate(weaponPrefab, WeaponSpawnPoint.position, WeaponSpawnPoint.rotation, WeaponSpawnPoint);
        else
            Debug.LogError($"Weapon prefab '{weaponPrefabName}' not found in Resources/Weapons folder.");
    }

    #endregion

    #region  SELECT WEAPON SCREEN

    public void ViewWeaponDisplay()
    {
        SelectedLoadoutPreviewPanel.SetActive(false);
        LoadoutButtonsContainer.gameObject.SetActive(false);
        MainMenuManager.SetPanelViewability(editLoadoutPanel:true);
        cameraManager.SetCameraPriority("weaponDisplay");
        InstantiateWeaponSelectionButtons();
    }

    private void InstantiateWeaponSelectionButtons()
    {
        //Clear exisiting
        foreach (Transform child in SelectWeaponButtonContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (WeaponItem weaponItem in PrimaryWeaponItems)
        {
            if (weaponItem.WeaponTypeItem == WeaponItem.WeaponType.Assault)
            {
                GameObject weaponButton = Instantiate(SelectWeaponButtonPrefab, SelectWeaponButtonContainer.transform);

                TextMeshProUGUI weaponButtonText = weaponButton.GetComponentInChildren<TextMeshProUGUI>();
                if (weaponButtonText != null)
                    weaponButtonText.text = weaponItem.WeaponName;

                //Add event listener on button after instantiation
                Button weaponButtonComponent = weaponButton.GetComponent<Button>();
                if (weaponButtonComponent != null)
                    weaponButtonComponent.onClick.AddListener(() => OnWeaponSelected(weaponItem));
            }
        }
    }

    private void OnWeaponSelected(WeaponItem selectedWeapon)
    {
        Debug.Log("Selected Weapon: " + selectedWeapon.WeaponName);
    }
    
    #endregion
}

[System.Serializable]
public class Loadout
{
    public string LoadoutName;
    public WeaponItem PrimaryWeapon;
    public WeaponItem SecondaryWeapon;

    public Loadout(string loadoutName, WeaponItem primaryWeapon, WeaponItem secondaryWeapon)
    {
        LoadoutName = loadoutName;
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