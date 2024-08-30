using UnityEngine;

[CreateAssetMenu(fileName = "GameSettingSO", menuName = "ScriptableObjects/GameSettingSO")]
public class SettingsInfoSO : ScriptableObject
{
    public string title;
    public Sprite image;
    public string description;
}
