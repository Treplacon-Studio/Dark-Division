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
    private List<Resolution> predefinedResolutions;

    private void Start()
    {
        InitializePredefinedResolutions();
        CreateSettingCells();
    }

    private void InitializePredefinedResolutions()
    {
        // Predefine the list of resolutions
        predefinedResolutions = new List<Resolution>
        {
            new Resolution { width = 1920, height = 1080 },  // 1080p
            new Resolution { width = 1280, height = 720 },   // 720p
            new Resolution { width = 1600, height = 900 },   // 900p
            new Resolution { width = 1366, height = 768 },   // 768p
            new Resolution { width = 2560, height = 1440 },  // 1440p
            new Resolution { width = 3840, height = 2160 }   // 4K
        };
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
        int newResolutionIndex = Mathf.Clamp(currentResolutionIndex + delta, 0, predefinedResolutions.Count - 1);
        Resolution newResolution = predefinedResolutions[newResolutionIndex];

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
            if (valueIndex >= 0 && valueIndex < predefinedResolutions.Count)
            {
                var res = predefinedResolutions[valueIndex];
                return $"{res.width} x {res.height}";
            }
        }
        // Add more cases for other settings if needed
        return "Unknown";
    }

    private int GetResolutionIndex()
    {
        Resolution currentResolution = Screen.currentResolution;
        return predefinedResolutions.FindIndex(res => res.width == Screen.width && res.height == Screen.height);
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
