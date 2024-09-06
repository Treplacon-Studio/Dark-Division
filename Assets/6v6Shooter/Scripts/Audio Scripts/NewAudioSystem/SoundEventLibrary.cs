using System;
using _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem.Helpers;
using AYellowpaper.SerializedCollections;
using FMODUnity;
using UnityEngine;

namespace _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem
{
    [CreateAssetMenu(fileName = "SoundLibrary", menuName = "ScriptableObjects/Sound Library")]
    public class SoundEventLibrary : ScriptableObject
    {
        //public List<SoundEventCategory> sfxEventCategories;
        //[SerializedDictionary("SoundEvent Category", "Dictionary of Sound Event Category")]
        public SerializedDictionary<SoundEvent.Category, SoundEventCategory> soundEventCategories;
        
        
        [Serializable]
        public class SoundEventCategory
        {
            //public SoundEvent.Category categoryName;
            [SerializedDictionary("SoundEvent Type", "Dictionary of Sound Event Types")]
            public SerializedDictionary<SoundEvent.Type, SoundEventType> soundEventTypes;

            public SoundEventCategory()
            {
                soundEventTypes = new SerializedDictionary<SoundEvent.Type, SoundEventType>();
            }
        }

        [Serializable]
        public class SoundEventType
        {
            //public SoundEvent.Type eventName;

            //[SerializedDictionary("EventId", "EventReference")]
            public SerializedDictionary<int, EventReference> eventReferences;
        }

        public EventReference GetEventReference(SoundEvent.Category category, SoundEvent.Type type,
            int id)
        {
            if (soundEventCategories.TryGetValue(category, out SoundEventCategory eventCategory))
            {
                if (eventCategory.soundEventTypes.TryGetValue(type, out SoundEventType eventType))
                {
                    if (eventType.eventReferences.TryGetValue(id, out EventReference eventReference))
                    {
                        return eventReference;
                    }
                    else
                    {
                        return new EventReference { Path = "event:/SFX/DefaultEventNotFound" };
                    }
                }
                else
                {
                    return new EventReference { Path = "event:/SFX/DefaultEventNotFound" };
                }
            }
            else
            {
                return new EventReference { Path = "event:/SFX/DefaultEventNotFound" };
            }
            
        }

        public void OnValidate()
        {
            foreach (SoundEvent.Category category in Enum.GetValues(typeof(SoundEvent.Category)))
            {
                //dd
                if (!soundEventCategories.ContainsKey(category))
                {
                    var newEventCategory = new SoundEventCategory(); 
                    soundEventCategories.Add(category, newEventCategory);

                    foreach (SoundEvent.Type soundEvent in Enum.GetValues(typeof(SoundEvent.Type)))
                    {
                        if (soundEvent.GetCategory() == category)
                        {
                            newEventCategory.soundEventTypes.Add(soundEvent, new SoundEventType());
                        }
                    }
                }
                else
                {
                    var dicCategory = soundEventCategories[category];
                    
                    foreach (SoundEvent.Type soundEvent in Enum.GetValues(typeof(SoundEvent.Type)))
                    {
                        if (soundEvent.GetCategory() == category)
                        {
                            if (!dicCategory.soundEventTypes.ContainsKey(soundEvent))
                            {
                                dicCategory.soundEventTypes.Add(soundEvent, new SoundEventType());
                            }
                        }
                        else
                        {
                            dicCategory.soundEventTypes.Remove(soundEvent);
                        }
                    }
                }
            }
        }
    }
}