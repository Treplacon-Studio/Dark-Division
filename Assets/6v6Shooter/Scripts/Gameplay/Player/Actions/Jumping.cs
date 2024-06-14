using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Jumping : MonoBehaviour
    {
        private PlayerAnimationController _pac;
        
        private void Awake()
        {
            _pac = GetComponent<PlayerAnimationController>();
            ActionsManager.Instance.Jumping = this;
        }
        
        public void Run(bool isLanding)
        {
            if(Input.GetButton("Jump"))
                _pac.PlayJumpAnimation();
            _pac.PlayLandFromAir(isLanding);
        }
    }
}
