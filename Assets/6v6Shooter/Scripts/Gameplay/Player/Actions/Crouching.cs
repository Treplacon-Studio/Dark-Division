using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Crouching : MonoBehaviour
    {
        [SerializeField] [Tooltip("Player animation controller.")]
        private PlayerAnimationController pac;
        
        private void Awake()
        {
            ActionsManager.Instance.Crouching = this;
        }
        
        public void Run(bool isCrouching)
        {
            return;
        }
    }
}
