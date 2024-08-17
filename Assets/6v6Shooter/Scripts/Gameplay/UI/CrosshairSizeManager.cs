using UnityEngine;
using UnityEngine.UI;

public class CrosshairSizeManager : MonoBehaviour
{
    [Header("Crosshair Components")]
    public Image centerDot;
    public Image[] lines; // Array for the 4 lines (left, top, right, bottom)

    [Header("Size and Position UI Elements")]
    public Slider widthSlider;  // Thickness slider
    public Slider heightSlider; // Length slider
    public Slider gapSlider;
    public Slider dotSizeSlider; // Slider to control the dot size

    [Header("Visibility Toggles")]
    public Toggle dotToggle;
    public Toggle linesToggle;

    void Start()
    {
        InitializeUI();
        AddListeners();
    }

    void InitializeUI()
    {
        // Hard set the size of all lines
        SetLineSize(10, 10); // Default width and height for all lines
        SetCrosshairGap(5);  // Set initial gap value
        SetDotSize(10);      // Default size for the dot
        dotToggle.isOn = true;
        linesToggle.isOn = true;
    }

    void AddListeners()
    {
        widthSlider.onValueChanged.AddListener(delegate { SetCrosshairWidth(widthSlider.value); });
        heightSlider.onValueChanged.AddListener(delegate { SetCrosshairHeight(heightSlider.value); });
        gapSlider.onValueChanged.AddListener(delegate { SetCrosshairGap(gapSlider.value); });
        dotSizeSlider.onValueChanged.AddListener(delegate { SetDotSize(dotSizeSlider.value); });
        dotToggle.onValueChanged.AddListener(delegate { SetDot(dotToggle.isOn); });
        linesToggle.onValueChanged.AddListener(delegate { SetLines(linesToggle.isOn); });
    }

    void SetLineSize(float width, float height)
    {
        // Set the width and height for all lines
        foreach (var line in lines)
        {
            line.rectTransform.sizeDelta = new Vector2(width, height);
        }
    }

    void SetCrosshairWidth(float width)
    {
        // Adjust the width by scaling the respective axis
        float clampedWidth = Mathf.Max(width, 0.1f);

        // Vertical lines (left, right) - adjust x scale
        lines[0].rectTransform.sizeDelta = new Vector2(clampedWidth, lines[0].rectTransform.sizeDelta.y); // Left line
        lines[2].rectTransform.sizeDelta = new Vector2(clampedWidth, lines[2].rectTransform.sizeDelta.y); // Right line

        // Horizontal lines (top, bottom) - adjust y scale
        lines[1].rectTransform.sizeDelta = new Vector2(lines[1].rectTransform.sizeDelta.x, clampedWidth); // Top line
        lines[3].rectTransform.sizeDelta = new Vector2(lines[3].rectTransform.sizeDelta.x, clampedWidth); // Bottom line
    }

    void SetCrosshairHeight(float height)
    {
        // Adjust the height by scaling the respective axis
        float clampedHeight = Mathf.Max(height, 0.1f);

        // Vertical lines (left, right) - adjust y scale
        lines[0].rectTransform.sizeDelta = new Vector2(lines[0].rectTransform.sizeDelta.x, clampedHeight); // Left line
        lines[2].rectTransform.sizeDelta = new Vector2(lines[2].rectTransform.sizeDelta.x, clampedHeight); // Right line

        // Horizontal lines (top, bottom) - adjust x scale
        lines[1].rectTransform.sizeDelta = new Vector2(clampedHeight, lines[1].rectTransform.sizeDelta.y); // Top line
        lines[3].rectTransform.sizeDelta = new Vector2(clampedHeight, lines[3].rectTransform.sizeDelta.y); // Bottom line
    }

    void SetCrosshairGap(float gap)
    {
        lines[0].rectTransform.anchoredPosition = new Vector2(-gap, 0);  // Left line
        lines[1].rectTransform.anchoredPosition = new Vector2(0, gap);   // Top line
        lines[2].rectTransform.anchoredPosition = new Vector2(gap, 0);   // Right line
        lines[3].rectTransform.anchoredPosition = new Vector2(0, -gap);  // Bottom line
    }

    void SetDotSize(float size)
    {
        // Ensure size is at least 0.1
        float clampedSize = Mathf.Max(size, 0.1f);
        centerDot.rectTransform.sizeDelta = new Vector2(clampedSize, clampedSize);
    }

    void SetDot(bool isEnabled)
    {
        centerDot.gameObject.SetActive(isEnabled);
    }

    void SetLines(bool areEnabled)
    {
        foreach (var line in lines)
            line.gameObject.SetActive(areEnabled);
    }
}
