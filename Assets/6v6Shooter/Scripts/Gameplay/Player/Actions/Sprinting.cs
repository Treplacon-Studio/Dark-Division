using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Sprinting : MonoBehaviour
    {
        [SerializeField] [Tooltip("Player animation controller.")]
        private PlayerAnimationController pac;
        
        public void Run()
        {
            pac.PlaySprintAnimation(Input.GetKey(KeyCode.LeftShift));
        }
    }
}
