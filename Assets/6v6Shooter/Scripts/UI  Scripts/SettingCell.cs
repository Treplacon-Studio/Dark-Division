using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SettingCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI settingTitle;
    public TextMeshProUGUI currentValueText;
    public Button increaseButton;
    public Button decreaseButton;
    public GameObject background;

    private SettingsInfoSO settingsInfo;
    private ViewControllerSettings viewController;
    private bool isHovered = false;

    public void Setup(SettingsInfoSO settingsInfo, ViewControllerSettings viewController)
    {
        this.settingsInfo = settingsInfo;
        this.viewController = viewController;

        settingTitle.text = settingsInfo.title;

        increaseButton.onClick.RemoveAllListeners();
        increaseButton.onClick.AddListener(() => OnChangeValue(1));
        decreaseButton.onClick.RemoveAllListeners();
        decreaseButton.onClick.AddListener(() => OnChangeValue(-1));

        UpdateUI();
    }

    private void OnChangeValue(int delta)
    {
        if (viewController != null && settingsInfo != null)
        {
            viewController.ChangeSettingValue(settingsInfo.title, delta);
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (settingsInfo != null && viewController != null)
        {
            int currentValueIndex = viewController.GetSettingValueIndex(settingsInfo.title);
            currentValueText.text = viewController.GetCurrentSettingValueText(settingsInfo.title, currentValueIndex);

            SetButtonsActive(isHovered);
            background.SetActive(isHovered);
        }
    }

    private void SetButtonsActive(bool active)
    {
        increaseButton.interactable = active;
        decreaseButton.interactable = active;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateUI();
        if (viewController != null && settingsInfo != null)
        {
            viewController.UpdateSettingPanel(settingsInfo); // Update the panel with the hovered setting's info
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateUI();
    }
}
