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

                case SoundEvent.Type.OnButtonPress:
                case SoundEvent.Type.OnButtonHover:
                case SoundEvent.Type.OnButtonRelease:
                case SoundEvent.Type.OnButtonHoverEnter:
                case SoundEvent.Type.OnButtonHoverExit:
                   return SoundEvent.Category.UI;

                default:
                    throw new System.ArgumentException("Unknown event type");
            }
        }
    }

}