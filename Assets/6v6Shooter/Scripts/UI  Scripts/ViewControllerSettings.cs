using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ViewControllerSettings : MonoBehaviour
{
    public GameObject settingCellPrefab;
    public Transform settingsPanel;
    public TextMeshProUGUI settingTitleText;
    public TextMeshProUGUI settingDescriptionText;
    public Image settingImage;

    public SettingsInfoSO[] settingsInfoList; // Array of settings info

    private SettingsInfoSO currentSettingInfo;
    private List<Resolution> availableResolutions;

    private void Start()
    {
        InitializeAvailableResolutions();
        CreateSettingCells();
    }

    private void InitializeAvailableResolutions()
    {
        availableResolutions = new List<Resolution>(Screen.resolutions);
    }

    private void CreateSettingCells()
    {
        foreach (var settingInfo in settingsInfoList)
        {
            GameObject cellObj = Instantiate(settingCellPrefab, settingsPanel);
            SettingCell settingCell = cellObj.GetComponent<SettingCell>();

            if (settingCell != null)
            {
                settingCell.Setup(settingInfo, this);
            }
        }
    }

    public void UpdateSettingPanel(SettingsInfoSO settingInfo)
    {
        if (settingInfo == null) return;

        currentSettingInfo = settingInfo;

        settingTitleText.text = settingInfo.title;
        settingDescriptionText.text = settingInfo.description;
        settingImage.sprite = settingInfo.image;
    }

    public void ChangeSettingValue(string settingTitle, int delta)
    {
        switch (settingTitle)
        {
            case "Quality":
                AdjustQualitySettings(delta);
                break;
            case "Resolution":
                AdjustResolution(delta);
                break;
                // Add more cases for other settings if needed
        }
    }

    private void AdjustQualitySettings(int delta)
    {
        int currentQuality = QualitySettings.GetQualityLevel();
        int qualityCount = QualitySettings.names.Length;

        int newQualityIndex = Mathf.Clamp(currentQuality + delta, 0, qualityCount - 1);
        QualitySettings.SetQualityLevel(newQualityIndex, true);

        // Optionally update the UI to reflect the change
        UpdateSettingPanel(GetSettingInfoByTitle("Quality")); // Assuming "Quality" is the title for quality settings
    }

    private void AdjustResolution(int delta)
    {
        int currentResolutionIndex = GetResolutionIndex();
        int newResolutionIndex = Mathf.Clamp(currentResolutionIndex + delta, 0, availableResolutions.Count - 1);
        Resolution newResolution = availableResolutions[newResolutionIndex];

        Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);

        // Optionally update the UI to reflect the change
        UpdateSettingPanel(GetSettingInfoByTitle("Resolution")); // Assuming "Resolution" is the title for resolution settings
    }

    public int GetSettingValueIndex(string settingTitle)
    {
        if (settingTitle == "Quality")
        {
            return QualitySettings.GetQualityLevel();
        }
        if (settingTitle == "Resolution")
        {
            return GetResolutionIndex();
        }
        // Add more cases for other settings if needed
        return 0;
    }

    public string GetCurrentSettingValueText(string settingTitle, int valueIndex)
    {
        if (settingTitle == "Quality")
        {
            return QualitySettings.names[valueIndex];
        }
        if (settingTitle == "Resolution")
        {
            if (valueIndex >= 0 && valueIndex < availableResolutions.Count)
            {
                var res = availableResolutions[valueIndex];
                return $"{res.width} x {res.height}";
            }
        }
        // Add more cases for other settings if needed
        return "Unknown";
    }

    private int GetResolutionIndex()
    {
        var currentResolution = Screen.currentResolution;
        return availableResolutions.FindIndex(res => res.width == currentResolution.width && res.height == currentResolution.height);
    }

    private SettingsInfoSO GetSettingInfoByTitle(string title)
    {
        foreach (var settingInfo in settingsInfoList)
        {
            if (settingInfo.title == title)
            {
                return settingInfo;
            }
        }
        return null;
    }
}
