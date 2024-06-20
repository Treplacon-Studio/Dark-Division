using _6v6Shooter.Scripts.Gameplay.Player.Animations;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Sprinting : MonoBehaviour
    {
        private PlayerAnimationController _pac;
        
        private void Awake()
        {
            _pac = GetComponent<PlayerAnimationController>();
            ActionsManager.Instance.Sprinting = this;
        }
        
        public void Run()
        {
            _pac.PlaySprintAnimation(Input.GetKey(KeyCode.LeftShift));
        }
    }
}
