using _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem;
using _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem.Helpers;
using UnityEngine.UI;
using UnityEngine;

public class ToggleAudio : UIAudioBase
{
    private Toggle toggle;

    protected override void Awake()
    {
        toggle = GetComponent<Toggle>();
        if (toggle == null)
        {
            Debug.LogWarning("Toggle component missing.");
            return;
        }

        base.Awake(); // Call the base Awake to ensure SubscribeToUIEvents is called
    }

    protected override void SubscribeToUIEvents()
    {
        toggle.onValueChanged.AddListener((isOn) =>
        {
            SoundEventBuilder.Create()
                .WithEventType(SoundEvent.Type.OnButtonPress)
                .PlayOneShot();
        });
    }
}
