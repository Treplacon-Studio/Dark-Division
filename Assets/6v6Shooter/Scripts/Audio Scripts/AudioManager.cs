using UnityEngine;
using FMODUnity; // Required for EventReference
using FMOD.Studio;
using System.Collections.Generic;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    // Static instance that can be accessed from anywhere
    public static AudioManager Instance { get; private set; }

    private Dictionary<string, EventInstance> eventInstances = new Dictionary<string, EventInstance>();
    private Dictionary<string, Bus> audioBuses = new Dictionary<string, Bus>();

    [Header("Audio Groups")]
    public string musicBusPath = "bus:/Music";
    public string sfxBusPath = "bus:/SFX";
    public string uiBusPath = "bus:/UI";

    private void Awake()
    {
        // Implementing the Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure the AudioManager persists across scenes
            InitializeBuses();
        }
        else
        {
            Destroy(gameObject); // Enforce the Singleton pattern by destroying duplicates
        }
    }

    private void InitializeBuses()
    {
        audioBuses[musicBusPath] = RuntimeManager.GetBus(musicBusPath);
        audioBuses[sfxBusPath] = RuntimeManager.GetBus(sfxBusPath);
        audioBuses[uiBusPath] = RuntimeManager.GetBus(uiBusPath);
    }

    public void SetBusVolume(string busPath, float volume)
    {
        if (audioBuses.ContainsKey(busPath))
        {
            audioBuses[busPath].setVolume(volume);
        }
    }

    public void MuteBus(string busPath, bool mute)
    {
        if (audioBuses.ContainsKey(busPath))
        {
            audioBuses[busPath].setMute(mute);
        }
    }

    public void PlayOneShot(EventReference eventReference, Vector3 position)
    {
        if (eventReference.IsNull == false)
        {
            RuntimeManager.PlayOneShot(eventReference, position);
        }
    }

    public void PlayEvent(EventReference eventReference, string eventId)
    {
        if (!eventInstances.ContainsKey(eventId))
        {
            EventInstance instance = RuntimeManager.CreateInstance(eventReference);
            eventInstances[eventId] = instance;
            instance.start();
        }
    }

    public void StopEvent(string eventId)
    {
        if (eventInstances.ContainsKey(eventId))
        {
            eventInstances[eventId].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            eventInstances[eventId].release();
            eventInstances.Remove(eventId);
        }
    }

    public void SetParameter(string eventId, string parameterName, float value)
    {
        if (eventInstances.ContainsKey(eventId))
        {
            eventInstances[eventId].setParameterByName(parameterName, value);
        }
    }

    public void FadeOutEvent(string eventId, float fadeDuration)
    {
        if (eventInstances.ContainsKey(eventId))
        {
            StartCoroutine(FadeOutCoroutine(eventId, fadeDuration));
        }
    }

    private IEnumerator FadeOutCoroutine(string eventId, float fadeDuration)
    {
        if (eventInstances.TryGetValue(eventId, out EventInstance instance))
        {
            instance.getVolume(out float currentVolume);
            float time = 0f;

            while (time < fadeDuration)
            {
                float volume = Mathf.Lerp(currentVolume, 0f, time / fadeDuration);
                instance.setVolume(volume);
                time += Time.deltaTime;
                yield return null;
            }

            StopEvent(eventId);
        }
    }

    public void StopAllSounds()
    {
        foreach (var instance in eventInstances.Values)
        {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
        }
        eventInstances.Clear();
    }
}
