using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CrosshairColorManager : MonoBehaviour
{
    [Header("Crosshair Components")]
    public Image centerDot;
    public Image[] lines; // Array for the 4 lines (left, top, right, bottom)

    [Header("Color UI Elements")]
    public TMP_Dropdown colorDropdown;
    public Slider opacitySlider;

    [Header("Customization Data")]
    public CustomizationColorsPalette colorsPalette;

    private List<Color> predefinedColors = new List<Color>();

    void Start()
    {
        InitializeUI();
        AddListeners();
    }

    void InitializeUI()
    {
        PopulateColorDropdown(colorDropdown);

        // Set default colors
        SetCrosshairColor(predefinedColors[0]); // White for lines

        // Set default opacity to 1
        opacitySlider.value = 1;
        SetCrosshairOpacity(1);
    }

    void PopulateColorDropdown(TMP_Dropdown dropdown)
    {
        dropdown.ClearOptions();
        List<string> colorLabels = new List<string>();

        foreach (var colorOption in colorsPalette.colors)
        {
            predefinedColors.Add(colorOption.color);
            colorLabels.Add(colorOption.name);
        }

        dropdown.AddOptions(colorLabels);
    }

    void AddListeners()
    {
        colorDropdown.onValueChanged.AddListener(delegate { OnColorChanged(); });
        opacitySlider.onValueChanged.AddListener(delegate { SetCrosshairOpacity(opacitySlider.value); });
    }

    void OnColorChanged()
    {
        SetCrosshairColor(GetColorFromDropdown(colorDropdown));
    }

    void SetCrosshairColor(Color color)
    {
        // Set the color of the Image components for both the center dot and lines
        centerDot.color = color;
        foreach (var line in lines)
        {
            line.color = color;
        }
    }

    void SetCrosshairOpacity(float opacity)
    {
        Color color = centerDot.color;
        color.a = opacity;
        SetCrosshairColor(color);
    }

    Color GetColorFromDropdown(TMP_Dropdown dropdown)
    {
        return predefinedColors[dropdown.value];
    }
}
