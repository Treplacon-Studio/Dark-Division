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
            //InitializeBuses();
        }
        else
        {
            Destroy(gameObject); // Enforce the Singleton pattern by destroying duplicates
        }
    }

    //private void InitializeBuses()
    //{
    //    audioBuses[musicBusPath] = RuntimeManager.GetBus(musicBusPath);
    //    audioBuses[sfxBusPath] = RuntimeManager.GetBus(sfxBusPath);
    //    audioBuses[uiBusPath] = RuntimeManager.GetBus(uiBusPath);
    //}
    
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
    
    public void SetParameter(string eventId, string parameterName, float value)
    {
        if (eventInstances.ContainsKey(eventId))
        {
            eventInstances[eventId].setParameterByName(parameterName, value);
        }
    }
}
