using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ViewControllerSettings : MonoBehaviour
{
    [Header("Settings Prefab")]
    public GameObject settingCellPrefab;  // Assign the prefab with SettingCell script

    [Header("Settings Data")]
    public SettingsInfoSO[] settingsData;  // Assign the array of ScriptableObjects here

    [Header("Settings Container")]
    public Transform settingsContainer;  // The parent transform where the setting cells will be spawned

    private List<SettingCell> settingCells = new List<SettingCell>();

    private void Start()
    {
        InitializeSettings();
    }

    private void InitializeSettings()
    {
        foreach (var settingInfo in settingsData)
        {
            GameObject cell = Instantiate(settingCellPrefab, settingsContainer);
            SettingCell settingCell = cell.GetComponent<SettingCell>();
            if (settingCell != null)
            {
                settingCell.Setup(settingInfo, this);
                settingCells.Add(settingCell);
            }
            else
            {
                Debug.LogError("SettingCell component missing on prefab.");
            }
        }
    }

    public void ChangeSettingValue(SettingsInfoSO settingInfo, int delta)
    {
        // Here you should handle the actual setting change logic
        // For example, you might save it to player preferences or apply it directly to game settings
        int currentValue = settingInfo.GetDefaultValue(); // This should actually retrieve the current value
        settingInfo.ChangeValue(ref currentValue, delta);

        Debug.Log($"Setting {settingInfo.setting}: Value changed to {currentValue}");
        // Optionally update UI or save new value
    }
}
