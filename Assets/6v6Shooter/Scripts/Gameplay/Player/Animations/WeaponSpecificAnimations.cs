using _6v6Shooter.Scripts.Gameplay.Player.Weapons;
using UnityEditor.Animations;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Animations
{
    public class WeaponSpecificAnimations : MonoBehaviour
    {
        [SerializeField] [Tooltip("Reloading animation.")]
        private Animator anim;

        [SerializeField] 
        private RuntimeAnimatorController scarController;
        
        [SerializeField] 
        private RuntimeAnimatorController m4a1Controller;
        
        [SerializeField] 
        private RuntimeAnimatorController dsr50Controller;

        public void ChangeAnimations(WeaponInfo.WeaponName n)
        {
            switch (n)
            {
                case WeaponInfo.WeaponName.ScarEnforcer557: 
                    ChangeWeaponAnimations(scarController);
                    break;
                case WeaponInfo.WeaponName.M4A1Sentinel254: 
                    ChangeWeaponAnimations(m4a1Controller);
                    break;
                case WeaponInfo.WeaponName.Dsr50: 
                    ChangeWeaponAnimations(dsr50Controller);
                    break;
            }
        }
        
        private void ChangeWeaponAnimations(RuntimeAnimatorController controller)
        {
            anim.runtimeAnimatorController = controller;
        }
    }
}
