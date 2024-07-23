using UnityEngine;

[CreateAssetMenu(fileName = "New Color Palette", menuName = "SO/ColorPalette", order = 1)]
public class ColorPalette : ScriptableObject
{
    [System.Serializable]
    public struct NamedColor
    {
        public string name;
        public Color color;
    }

    public NamedColor[] namedColors;

    public Color GetColor(string name)
    {
        foreach (var namedColor in namedColors)
        {
            if (namedColor.name == name)
            {
                return namedColor.color;
            }
        }
        Debug.LogError("Color name not found.");
        return Color.black; // Default to black if name is not found
    }

    public Color GetColor(int index)
    {
        if (index < 0 || index >= namedColors.Length)
        {
            Debug.LogError("Color index out of range.");
            return Color.black; // Default to black if index is out of range
        }
        return namedColors[index].color;
    }
}
