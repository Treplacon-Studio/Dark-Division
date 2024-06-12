using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Walking : MonoBehaviour
    {
        [SerializeField] [Tooltip("Player animation controller.")]
        private PlayerAnimationController pac;
        
        private void Awake()
        {
            ActionsManager.Instance.Walking = this;
        }
        
        public void Run(Vector2 input, bool grounded)
        {
            pac.PlayWalkingAnimation(input, grounded);
        }
    }
}
