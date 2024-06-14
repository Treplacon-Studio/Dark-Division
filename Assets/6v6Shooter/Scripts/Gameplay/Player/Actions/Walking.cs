using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Walking : MonoBehaviour
    {
        private PlayerAnimationController _pac;
        
        private void Awake()
        {
            _pac = GetComponent<PlayerAnimationController>();
            ActionsManager.Instance.Walking = this;
        }
        
        public void Run(Vector2 input, bool grounded)
        {
            _pac.PlayWalkingAnimation(input, grounded);
        }
    }
}
