using FMODUnity;
using UnityEngine;

namespace _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem
{
    
    public static class SoundEvent
    {
        public enum Category
        {
            Gameplay = 0,
            Weapon = 1,
            UI = 3,

        }

        public enum Type
        {
            //Weapon Category
            Fire = 0,
            Reload = 1,
            //Gameplay Category
            HitMarker = 2,
            //Ui Category
            OnButtonPress = 3,
            OnButtonRelease = 4,
            OnButtonHover = 5,
            OnButtonHoverEnter = 6,
            OnButtonHoverExit = 7,
            
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

        public static void PlayOneShot(Category eventCategory, Type eventType, int id)
        {
            if (AudioAssets.Instance.soundEventLibrary == null)
            {
                Debug.LogError("SoundEventLibrary is not assigned to the SoundManager.");
                return;
            }
            
            EventReference eventReference = AudioAssets.Instance.soundEventLibrary.GetEventReference(eventCategory, eventType,id);
            
            RuntimeManager.PlayOneShot(eventReference);
        }
        
        public static void PlayOneShotAtPosition(Category eventCategory, Type eventType, int id, Vector3 position)
        {
            if (AudioAssets.Instance.soundEventLibrary == null)
            {
                Debug.LogError("SoundEventLibrary is not assigned to the SoundManager.");
                return;
            }
            
            EventReference eventReference = AudioAssets.Instance.soundEventLibrary.GetEventReference(eventCategory, eventType,id);
            
            RuntimeManager.PlayOneShot(eventReference, position);
        }

        
    }
}