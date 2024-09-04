using FMOD;
using UnityEngine;

namespace _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem.Helpers
{
    public class SoundEventBuilder
    {
        private SoundEvent.Category category;
        private SoundEvent.Type eventType;
        private int id = 0;
        
        public static SoundEventBuilder Create()
        {
            return new SoundEventBuilder();
        }
        
        public SoundEventBuilder WithEventType(SoundEvent.Type eventType)
        {
            this.eventType = eventType;
            this.category = eventType.GetCategory();
            return this;
        }
        
        public SoundEventBuilder WithId(int id)
        {
            this.id = id;
            return this;
        }
        
        public void PlayOneShotAttached(GameObject targetObject)
        {
            SoundEvent.PlayOneShotAttached(category, eventType, id, targetObject);
        }

        public void PlayOneShot()
        {
            SoundEvent.PlayOneShot(category, eventType, id);
        }
        
        public void PlayOneShotAtPosition(Vector3 position)
        {
            SoundEvent.PlayOneShotAtPosition(category, eventType, id, position);
        }
    }

}