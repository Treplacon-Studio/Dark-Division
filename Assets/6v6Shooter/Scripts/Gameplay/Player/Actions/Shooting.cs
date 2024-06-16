using _6v6Shooter.Scripts.Gameplay.Player.Weapons;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Shooting : MonoBehaviour
    {
        [SerializeField] [Tooltip("Weapon in hands.")]
        private WeaponInfo.WeaponName weaponName;

        [SerializeField] [Tooltip("Bullet pool manager component to shot bullets.")]
        private BulletPoolingManager bpm;

        [SerializeField] [Tooltip("Weapon that is currently held in hands.")]
        private GameObject weaponObject;
        
        private Transform _bulletStartPoint;
        private PlayerAnimationController _pac;
        private Weapon _weapon;
        private float _nextFireTime;
        
        private void Awake()
        {
            _pac = GetComponent<PlayerAnimationController>();
            ActionsManager.Instance.Shooting = this;
            _weapon = new Weapon(weaponName);
            _bulletStartPoint = GameObject.Find("BulletStartPoint").transform;
            if(_bulletStartPoint == null)
                Debug.LogError("Bullet start point not found.");
        }

        public void ChangeWeapon(WeaponInfo.WeaponName wn)
        {
            _weapon = new Weapon(wn);
        }
        
        private void AutomaticFire()
        {
            if (!Input.GetMouseButton(0) || !(Time.time >= _nextFireTime)) 
                return;
           
            _nextFireTime = Time.time + _weapon.Info().Stats().FireRate;
            bpm.SpawnFromPool(_weapon.Info().Stats().BType, 
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
