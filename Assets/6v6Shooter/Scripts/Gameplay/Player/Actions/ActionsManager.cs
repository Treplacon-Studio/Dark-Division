using System;
using _6v6Shooter.Scripts.Gameplay.Player.Animations;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class ActionsManager
    {
        
        private static ActionsManager _instance;
        
        public static ActionsManager Instance
        {
            get { return _instance ??= new ActionsManager(); }
        }

        public PlayerAnimationController Pac;
        
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
