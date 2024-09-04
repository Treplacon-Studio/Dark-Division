using _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem;
using _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem.Helpers;
using UnityEngine.UI;
using UnityEngine;

public class ButtonAudio : UIAudioBase
{
    private Button button;

    protected override void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogWarning("Button component missing.");
            return;
        }

        base.Awake(); // Call the base Awake to ensure SubscribeToUIEvents is called
    }

    protected override void SubscribeToUIEvents()
    {
        button.onClick.AddListener(() =>
        {
            SoundEventBuilder.Create()
                .WithEventType(SoundEvent.Type.OnButtonPress)
                .PlayOneShot();
        });
    }
}
