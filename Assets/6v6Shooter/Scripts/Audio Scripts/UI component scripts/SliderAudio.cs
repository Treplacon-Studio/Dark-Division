using _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem;
using _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem.Helpers;
using UnityEngine.UI;
using UnityEngine;

public class SliderAudio : UIAudioBase
{
    private Slider slider;

    protected override void Awake()
    {
        slider = GetComponent<Slider>();
        if (slider == null)
        {
            Debug.LogWarning("Slider component missing.");
            return;
        }

        base.Awake(); // Call the base Awake to ensure SubscribeToUIEvents is called
    }

    protected override void SubscribeToUIEvents()
    {
        slider.onValueChanged.AddListener((value) =>
        {
            SoundEventBuilder.Create()
                .WithEventType(SoundEvent.Type.OnButtonPress)
                .PlayOneShot();
        });
    }
}
