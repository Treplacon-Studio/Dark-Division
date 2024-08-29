using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Required for pointer event interfaces

public class SettingCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Inactive")]
    public GameObject inactiveSetting;
    public TextMeshProUGUI settingTitleInactive;

    [Header("Active")]
    public GameObject activeSetting;
    public TextMeshProUGUI settingTitleActive;
    public Button increaseButton;
    public Button decreaseButton;

    [Header("Current Value Display")]
    public TextMeshProUGUI currentValueText; // New field for displaying the current setting value

    private SettingsInfoSO settingsInfo;
    private ViewControllerSettings viewController;
    private bool isHovered = false;

    public void Setup(SettingsInfoSO settingsInfo, ViewControllerSettings viewController)
    {
        this.settingsInfo = settingsInfo;
        this.viewController = viewController;

        // Initialize UI elements
        settingTitleActive.text = settingsInfo.title;
        settingTitleInactive.text = settingsInfo.title;

        // Setup button listeners
        increaseButton.onClick.AddListener(() => ChangeValue(1));
        decreaseButton.onClick.AddListener(() => ChangeValue(-1));

        // Update UI to reflect the current state
        UpdateUI();
    }

    private void ChangeValue(int delta)
    {
        if (viewController != null)
        {
            viewController.ChangeSettingValue(settingsInfo, delta);
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        // Show active or inactive state based on whether the user is hovering
        activeSetting.SetActive(isHovered);
        inactiveSetting.SetActive(!isHovered);

        // Update the current value display based on the setting's current state
        if (settingsInfo.options != null && settingsInfo.options.Length > 0)
        {
            // Assuming that the setting value is stored somewhere; you can fetch it here.
            int currentValue = settingsInfo.GetDefaultValue(); // Replace this with actual value retrieval logic

            // Update the text to display the current value
            currentValueText.text = settingsInfo.options[currentValue];
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Detect mouse hover
        isHovered = true;
        UpdateUI();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Detect when mouse leaves
        isHovered = false;
        UpdateUI();
    }
}
