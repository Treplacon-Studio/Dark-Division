using _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem;
using _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIAudioBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool playOnPress = true;
    [Space] 
    public bool playOnHoverEnter = true;
    [Space]
    public bool playOnHoverExit = true;
    [Space]
    public bool playOnExit = false;
    [Space]
    public bool playOnRelease = true;
    

    protected virtual void Awake()
    {
        SubscribeToUIEvents();
    }

    // Abstract method that must be implemented by derived classes
    protected abstract void SubscribeToUIEvents();
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playOnHoverEnter)
        {
            SoundEventBuilder.Create()
                .WithEventType(SoundEvent.Type.OnButtonHoverEnter)
                .PlayOneShot();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (playOnHoverExit)
        {
            SoundEventBuilder.Create()
                .WithEventType(SoundEvent.Type.OnButtonHoverExit)
                .PlayOneShot(); 
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (playOnPress)
        {
            SoundEventBuilder.Create()
                .WithEventType(SoundEvent.Type.OnButtonPress)
                .PlayOneShot();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (playOnRelease)
        {
            SoundEventBuilder.Create()
                .WithEventType(SoundEvent.Type.OnButtonRelease)
                .PlayOneShot();
        }
    }
}
