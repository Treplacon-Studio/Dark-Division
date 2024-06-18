using _6v6Shooter.Scripts.Gameplay.Player.Animations;
using _6v6Shooter.Scripts.Gameplay.Player.Weapons;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Switching : MonoBehaviour
    {
        [SerializeField] [Tooltip("Specific weapon animations.")]
        private WeaponSpecificAnimations wpa;
        
        [SerializeField] [Tooltip("Guns that player has.")]
        private GameObject[] equippedGuns;
            
        [SerializeField] [Tooltip("Socket where the gun is kept.")]
        private GameObject gunSocket;
        
        private GameObject _weapon;
        private Transform _bulletStartPoint;
        private PlayerAnimationController _pac;
        private float _nextFireTime;
        private int _gunInHandsIndex;
        private bool _weaponInitialized;
        
        private void Awake()
        {
            _pac = GetComponent<PlayerAnimationController>();
            ActionsManager.Instance.Switching = this;
            _gunInHandsIndex = 0;
            
            foreach (var gun in equippedGuns)
            {
                if (gun != null)
                    gun.SetActive(false);
                else
                    Debug.LogError("Equipped gun is null!");
            }
            
            SwitchWeapon(_gunInHandsIndex);
            _weaponInitialized = true;
        }

        private void SwitchWeapon(int wn)
        {
            _gunInHandsIndex = wn;
            foreach (var w in equippedGuns)
                w.SetActive(false);
            wpa.ChangeAnimations(equippedGuns[wn].GetComponent<Weapon>().Info().Name());
            _weapon = equippedGuns[wn];
            _weapon.SetActive(true);
        }
        
        public void Run()
        {
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f) //Scrolled up
            {
                if(_gunInHandsIndex == 0)
                    SwitchWeapon(equippedGuns.Length-1);
                else
                    SwitchWeapon(_gunInHandsIndex-1);
            }
            else if (scroll < 0f) //Scrolled down
            {
                SwitchWeapon((_gunInHandsIndex+1) % equippedGuns.Length);
            }
        }

        public Weapon WeaponComponent()
        {
            if (!_weaponInitialized)
                return equippedGuns[_gunInHandsIndex].GetComponent<Weapon>();
            if (_weapon != null)
                return _weapon.GetComponent<Weapon>();
            
            Debug.LogError("Internal weapon error.");
            return null;
        }
    }
}
