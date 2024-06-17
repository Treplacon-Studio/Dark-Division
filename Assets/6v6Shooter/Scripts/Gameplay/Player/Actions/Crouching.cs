using _6v6Shooter.Scripts.Gameplay.Player.Animations;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Crouching : MonoBehaviour
    {
        private PlayerAnimationController _pac;
        
        private void Awake()
        {
            _pac = GetComponent<PlayerAnimationController>();
            ActionsManager.Instance.Crouching = this;
        }
        
        public void Run(bool isCrouching)
        {
            return;
        }
    }
}
