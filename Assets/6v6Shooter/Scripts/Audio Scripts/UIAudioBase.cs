using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIAudioBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public EventReference onClickSound;
    public bool playOnPress = true;
    [Space] 
    public EventReference onHoverEnterSound;
    public bool playOnHoverEnter = true;
    [Space]
    public EventReference onHoverExitSound;
    public bool playOnHoverExit = true;
    [Space]
    public EventReference onPressSound;
    public bool playOnExit = false;
    [Space]
    public EventReference onReleaseSound;
    public bool playOnRelease = true;
    

    protected virtual void Awake()
    {
        SubscribeToUIEvents();
    }

    // Abstract method that must be implemented by derived classes
    protected abstract void SubscribeToUIEvents();

    protected void PlaySound(EventReference sound)
    {
        if (AudioManager.Instance != null && !sound.IsNull)
        {
            AudioManager.Instance.PlayOneShot(sound, Vector3.zero);
        }
        else
        {
            Debug.LogWarning("AudioManager or sound is not set.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playOnHoverEnter)
        {
            PlaySound(onHoverEnterSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (playOnHoverExit)
        {
            PlaySound(onHoverExitSound); // Optionally change this to another sound if needed
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (playOnPress)
        {
            PlaySound(onPressSound);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (playOnRelease)
        {
            PlaySound(onReleaseSound);
        }
    }
}
