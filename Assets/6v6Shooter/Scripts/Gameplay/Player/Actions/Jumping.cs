using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Jumping : MonoBehaviour
    {
        [SerializeField] [Tooltip("Player animation controller.")]
        private PlayerAnimationController pac;
        
        public void Run(bool isLanding)
        {
            if(Input.GetButton("Jump"))
                pac.PlayJumpAnimation();
            pac.PlayLandFromAir(isLanding);
        }
    }
}
