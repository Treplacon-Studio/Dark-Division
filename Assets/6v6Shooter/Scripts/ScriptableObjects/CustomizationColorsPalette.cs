using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CustomizationColorsPalette", menuName = "ScriptableObjects/CustomizationColorsPalette", order = 1)]
public class CustomizationColorsPalette : ScriptableObject
{
    [System.Serializable]
    public struct ColorOption
    {
        public string name;
        public Color color;
    }

    public List<ColorOption> colors;
}
