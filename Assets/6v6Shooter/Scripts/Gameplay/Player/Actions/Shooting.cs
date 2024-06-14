using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Shooting : MonoBehaviour
    {
        private PlayerAnimationController _pac;
        
        private void Awake()
        {
            _pac = GetComponent<PlayerAnimationController>();
            ActionsManager.Instance.Shooting = this;
        }
        
        public void Run()
        {
            if(Input.GetMouseButtonDown(0))
                _pac.PlayShootAnimation();
        }
    }
}
