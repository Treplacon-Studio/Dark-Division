using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class ActionsManager
    {
        private static ActionsManager _instance;
        
        public static ActionsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ActionsManager();
                }
                return _instance;
            }
        }

        public Reloading Reloading;
        public Inspecting Inspecting;
        public Aiming Aiming;
        public Shooting Shooting;
        public Switching Switching;

        public Walking Walking;
        public Sprinting Sprinting;
        public Jumping Jumping;
        public Crouching Crouching;
    }
}
