using UnityEngine;

namespace _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem.Helpers
{
    public class SoundEventBuilder
    {
        private SoundEvent.Category category;
        private SoundEvent.Type eventType;
        private int id = 0;

        // Método estático para iniciar la construcción del evento
        public static SoundEventBuilder Create()
        {
            return new SoundEventBuilder();
        }

        // Este método ahora asigna automáticamente la categoría basada en el EventType
        public SoundEventBuilder WithEventType(SoundEvent.Type eventType)
        {
            this.eventType = eventType;
            this.category = eventType.GetCategory();  // Asigna la categoría basada en el tipo de evento
            return this;
        }

        // Método para asignar el ID del evento
        public SoundEventBuilder WithId(int id)
        {
            this.id = id;
            return this;
        }

        // Método para reproducir el evento
        public void PlayOneShotAttached(GameObject targetObject)
        {
            SoundEvent.PlayOneShotAttached(category, eventType, id, targetObject);
        }
    }

}