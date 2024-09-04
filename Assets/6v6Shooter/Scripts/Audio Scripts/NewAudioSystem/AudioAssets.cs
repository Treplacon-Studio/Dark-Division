using UnityEngine;

namespace _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem
{
    public class AudioAssets : MonoBehaviour
    {
        private static AudioAssets _instance;

        public static AudioAssets Instance
        {
            get
            {
                if (_instance == null) _instance = (Instantiate(Resources.Load("AudioAssets")) as GameObject)
                    ?.GetComponent<AudioAssets>();
                return _instance;
            }
        }
        
        public SoundEventLibrary soundEventLibrary;
        
    }
}
