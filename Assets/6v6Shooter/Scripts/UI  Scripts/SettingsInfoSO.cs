using UnityEngine;

public enum InGameSettingsType
{
    Resolution,
    Quality,
    AntiAliasing,
    Shadows,
    TextureQuality,
    AnisotropicFiltering,
    VSync,
    FullScreenMode,
    Brightness,
    Contrast,
    AmbientOcclusion, // Only if post-processing is enabled
    Bloom, // Only if post-processing is enabled
    MotionBlur, // Only if post-processing is enabled
    DepthOfField, // Only if post-processing is enabled
    RenderDistance,
    ShadowResolution,
    SoftShadows,
    ShadowDistance,
    ParticleEffects,
    Reflections,
    ChromaticAberration, // Only if post-processing is enabled
    LensFlare,
    ScreenSpaceReflections, // URP specific
    ShadowCascade, // URP specific
    Volumetrics, // URP specific (via assets or extensions)
    GlobalIllumination
}

public enum SettingType
{
    Toggle,
    ThreeChoice,
    FiveChoice
}


[CreateAssetMenu(fileName = "GameSettingSO", menuName = "ScriptableObjects/GameSettingSO")]
public class SettingsInfoSO : ScriptableObject
{
    public InGameSettingsType setting;
    public SettingType settingType;        
    public string title;
    public Sprite image;
    public string description;

    public string[] options;               

    public virtual int GetDefaultValue()
    {
        return 0;
    }

    public virtual void ChangeValue(ref int currentValue, int delta)
    {
        switch (settingType)
        {
            case SettingType.Toggle:
                currentValue = (currentValue == 0) ? 1 : 0;
                break;
            case SettingType.ThreeChoice:
                currentValue = Mathf.Clamp(currentValue + delta, 0, 2);
                break;
            case SettingType.FiveChoice:
                currentValue = Mathf.Clamp(currentValue + delta, 0, 4);
                break;
        }
        Debug.Log($"Setting {setting}: New Value {currentValue}");
    }
}
