using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairCustomizer : MonoBehaviour
{
    [Header("Crosshair Components")]
    public Image centerDot;
    public Image horizontalLine;
    public Image verticalLine;

    [Header("Customization UI Elements")]
    public TMP_Dropdown colorDropdown;
    public Slider thicknessSlider;
    public Slider lengthSlider;
    public Slider gapSlider;
    public Slider opacitySlider;
    public Toggle outlineToggle;
    public Slider outlineThicknessSlider;
    public Slider outlineOpacitySlider;
    public TMP_Dropdown crosshairTypeDropdown;

    void Start()
    {
        UnlockCursor();

        // Initialize UI elements with default values
        InitializeUI();

        // Add listeners to UI elements
        AddListeners();
    }

    void InitializeUI()
    {
        // Set default values for the crosshair customization
        SetCrosshairColor(Color.white);
        SetCrosshairThickness(2);
        SetCrosshairLength(20);
        SetCrosshairGap(5);
        SetCrosshairOpacity(1);
        SetOutline(false);
        SetCrosshairType(0); // Assuming 0 is the default crosshair type
    }

    void AddListeners()
    {
        colorDropdown.onValueChanged.AddListener(delegate { OnColorChanged(); });
        thicknessSlider.onValueChanged.AddListener(delegate { OnThicknessChanged(); });
        lengthSlider.onValueChanged.AddListener(delegate { OnLengthChanged(); });
        gapSlider.onValueChanged.AddListener(delegate { OnGapChanged(); });
        opacitySlider.onValueChanged.AddListener(delegate { OnOpacityChanged(); });
        outlineToggle.onValueChanged.AddListener(delegate { OnOutlineToggle(); });
        outlineThicknessSlider.onValueChanged.AddListener(delegate { OnOutlineThicknessChanged(); });
        outlineOpacitySlider.onValueChanged.AddListener(delegate { OnOutlineOpacityChanged(); });
        crosshairTypeDropdown.onValueChanged.AddListener(delegate { OnCrosshairTypeChanged(); });
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnColorChanged()
    {
        Color selectedColor = GetColorFromDropdown();
        SetCrosshairColor(selectedColor);
    }

    void OnThicknessChanged()
    {
        float thickness = thicknessSlider.value;
        SetCrosshairThickness(thickness);
    }

    void OnLengthChanged()
    {
        float length = lengthSlider.value;
        SetCrosshairLength(length);
    }

    void OnGapChanged()
    {
        float gap = gapSlider.value;
        SetCrosshairGap(gap);
    }

    void OnOpacityChanged()
    {
        float opacity = opacitySlider.value;
        SetCrosshairOpacity(opacity);
    }

    void OnOutlineToggle()
    {
        bool isEnabled = outlineToggle.isOn;
        SetOutline(isEnabled);
    }

    void OnOutlineThicknessChanged()
    {
        if (outlineToggle.isOn)
        {
            float outlineThickness = outlineThicknessSlider.value;
            SetOutlineThickness(outlineThickness);
        }
    }

    void OnOutlineOpacityChanged()
    {
        if (outlineToggle.isOn)
        {
            float outlineOpacity = outlineOpacitySlider.value;
            SetOutlineOpacity(outlineOpacity);
        }
    }

    void OnCrosshairTypeChanged()
    {
        int type = crosshairTypeDropdown.value;
        SetCrosshairType(type);
    }

    // Methods to apply customization
    void SetCrosshairColor(Color color)
    {
        centerDot.color = color;
        horizontalLine.color = color;
        verticalLine.color = color;
    }

    void SetCrosshairThickness(float thickness)
    {
        horizontalLine.rectTransform.sizeDelta = new Vector2(horizontalLine.rectTransform.sizeDelta.x, thickness);
        verticalLine.rectTransform.sizeDelta = new Vector2(thickness, verticalLine.rectTransform.sizeDelta.y);
    }

    void SetCrosshairLength(float length)
    {
        horizontalLine.rectTransform.sizeDelta = new Vector2(length, horizontalLine.rectTransform.sizeDelta.y);
        verticalLine.rectTransform.sizeDelta = new Vector2(verticalLine.rectTransform.sizeDelta.x, length);
    }

    void SetCrosshairGap(float gap)
    {
        horizontalLine.rectTransform.anchoredPosition = new Vector2(0, gap);
        verticalLine.rectTransform.anchoredPosition = new Vector2(gap, 0);
    }

    void SetCrosshairOpacity(float opacity)
    {
        Color color = centerDot.color;
        color.a = opacity;
        SetCrosshairColor(color);
    }

    void SetOutline(bool isEnabled)
    {
        // Implement outline logic, such as enabling or disabling outline components
    }

    void SetOutlineThickness(float thickness)
    {
        // Implement outline thickness adjustment
    }

    void SetOutlineOpacity(float opacity)
    {
        // Implement outline opacity adjustment
    }

    void SetCrosshairType(int type)
    {
        switch (type)
        {
            case 0: // Default crosshair
                centerDot.gameObject.SetActive(true);
                horizontalLine.gameObject.SetActive(true);
                verticalLine.gameObject.SetActive(true);
                break;
            case 1: // Circle
                centerDot.gameObject.SetActive(false);
                horizontalLine.gameObject.SetActive(false);
                verticalLine.gameObject.SetActive(false);
                // Implement circle logic
                break;
            case 2: // Crosshairs
                centerDot.gameObject.SetActive(false);
                horizontalLine.gameObject.SetActive(true);
                verticalLine.gameObject.SetActive(true);
                break;
            case 3: // Circle & Crosshairs
                centerDot.gameObject.SetActive(true);
                horizontalLine.gameObject.SetActive(true);
                verticalLine.gameObject.SetActive(true);
                // Implement circle logic
                break;
        }
    }

    Color GetColorFromDropdown()
    {
        // Implement logic to get color from dropdown selection
        return Color.white; // Placeholder
    }
}
