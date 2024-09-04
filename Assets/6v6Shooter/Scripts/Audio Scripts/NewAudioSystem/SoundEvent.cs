using FMODUnity;
using UnityEngine;

namespace _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem
{
    
    public static class SoundEvent
    {
        public enum Category
        {
            Weapon,
            PlayerEvents
        }

        public enum Type
        {
            Fire,
            Reload
        }
        
        
        public static void PlayOneShotAttached(Category eventCategory,Type eventType, int id, GameObject targetObject)
        {
            if (AudioAssets.Instance.soundEventLibrary == null)
            {
                Debug.LogError("SoundEventLibrary is not assigned to the SoundManager.");
                return;
            }
            
            EventReference eventReference = AudioAssets.Instance.soundEventLibrary.GetEventReference(eventCategory, eventType, id);
            
            RuntimeManager.PlayOneShotAttached(eventReference, targetObject);
        }
        
    }
}