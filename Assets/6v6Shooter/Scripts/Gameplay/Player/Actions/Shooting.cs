using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Shooting : MonoBehaviour
    {
        private PlayerAnimationController _pac;
        
        public float fireRate = 0.1f; 
        private float _nextFireTime;
        
        private void Awake()
        {
            _pac = GetComponent<PlayerAnimationController>();
            ActionsManager.Instance.Shooting = this;
        }
        
        void AutomaticFire()
        {
            if (!Input.GetMouseButton(0) || !(Time.time >= _nextFireTime)) 
                return;
           
            _nextFireTime = Time.time + fireRate;
            _pac.PlayShootAnimation(ActionsManager.Instance.Aiming.IsAiming());
        }
        
        public void Run()
        {
            AutomaticFire();
        }
    }
}
