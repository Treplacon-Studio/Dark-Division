namespace _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem.Helpers
{
    public static class EventTypeExtensions
    {
        // Método que asigna automáticamente la categoría según el tipo de evento
        public static SoundEvent.Category GetCategory(this SoundEvent.Type eventType)
        {
            switch (eventType)
            {
                case SoundEvent.Type.Fire:
                case SoundEvent.Type.Reload:
                    return SoundEvent.Category.Weapon;

                //case SoundManager.EventType.Jump:
                //case SoundManager.EventType.Hit:
                   // return SoundManager.EventsCategory.PlayerEvents;

                default:
                    throw new System.ArgumentException("Unknown event type");
            }
        }
    }

}