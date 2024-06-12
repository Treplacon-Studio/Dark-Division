using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Shooting : MonoBehaviour
    {
        [SerializeField] [Tooltip("Player animation controller.")]
        private PlayerAnimationController pac;
        
        public void Run()
        {
            if(Input.GetMouseButtonDown(0))
                pac.PlayShootAnimation();
        }
    }
}
