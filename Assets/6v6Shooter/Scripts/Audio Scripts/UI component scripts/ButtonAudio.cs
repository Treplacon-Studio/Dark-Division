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
        button.onClick.AddListener(() => PlaySound(onClickSound));
    }
}
