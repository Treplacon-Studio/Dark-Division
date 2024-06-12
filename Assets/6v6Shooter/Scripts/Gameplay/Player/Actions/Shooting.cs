using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Shooting : MonoBehaviour
    {
        [SerializeField] [Tooltip("Player animation controller.")]
        private PlayerAnimationController pac;
        
        private void Awake()
        {
            ActionsManager.Instance.Shooting = this;
        }
        
        public void Run()
        {
            if(Input.GetMouseButtonDown(0))
                pac.PlayShootAnimation();
        }
    }
}
