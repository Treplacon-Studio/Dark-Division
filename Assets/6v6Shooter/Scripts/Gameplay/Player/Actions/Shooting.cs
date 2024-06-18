using _6v6Shooter.Scripts.Gameplay.Player.Animations;
using _6v6Shooter.Scripts.Gameplay.Player.Weapons;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Shooting : MonoBehaviour
    {
        [SerializeField] [Tooltip("Bullet pool manager component to shot bullets.")]
        private BulletPoolingManager bpm;
        
        private Transform _bulletStartPoint;
        private PlayerAnimationController _pac;
        private float _nextFireTime;
        
        private void Awake()
        {
            _pac = GetComponent<PlayerAnimationController>();
            ActionsManager.Instance.Shooting = this;
            _bulletStartPoint = ActionsManager.Instance.Switching.WeaponComponent().GetStartPoint().transform;
            if(_bulletStartPoint == null)
                Debug.LogError("Bullet start point not found.");
        }
        
        private void AutomaticFire()
        {
            if (!Input.GetMouseButton(0) || !(Time.time >= _nextFireTime))
                return;
            
            var wi = ActionsManager.Instance.Switching.WeaponComponent().Info().Stats();
            _nextFireTime = Time.time + wi.FireRate;
            bpm.SpawnFromPool(wi.BType, 
                _bulletStartPoint.transform.position,
                _bulletStartPoint.transform.rotation);
            _pac.PlayShootAnimation(ActionsManager.Instance.Aiming.IsAiming());
        }
        
        public void Run()
        {
            AutomaticFire();
        }
    }
}
