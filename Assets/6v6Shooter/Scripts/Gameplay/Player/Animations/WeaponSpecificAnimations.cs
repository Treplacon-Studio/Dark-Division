using _6v6Shooter.Scripts.Gameplay.Player.Weapons;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Animations
{
    public class WeaponSpecificAnimations : MonoBehaviour
    {
        [SerializeField] [Tooltip("Reloading animation.")]
        private Animator anim;

        [SerializeField] private AnimationClip[] scarClips = new AnimationClip[6];
        [SerializeField] private AnimationClip[] m4Clips = new AnimationClip[6];
        [SerializeField] private AnimationClip[] dsrClips = new AnimationClip[6];

        public void ChangeAnimations(WeaponInfo.WeaponName n)
        {
            switch (n)
            {
                case WeaponInfo.WeaponName.ScarEnforcer557: 
                    ChangeWeaponSpecificAnimations(scarClips);
                    break;
                case WeaponInfo.WeaponName.M4A1Sentinel254: 
                    ChangeWeaponSpecificAnimations(m4Clips);
                    break;
                case WeaponInfo.WeaponName.Dsr50: 
                    ChangeWeaponSpecificAnimations(scarClips);
                    break;
            }
        }
        
        private void ChangeWeaponSpecificAnimations(AnimationClip[] clips)
        {
            var ac = anim.runtimeAnimatorController;
            for (var i = 0; i < ac.animationClips.Length; i++)
            {
                switch (ac.animationClips[i].name)
                {
                    case "AN_FPS_Inspect":
                        ac.animationClips[i] = clips[0];
                        break;
                    case "AN_FPS_Reload":
                        ac.animationClips[i] = clips[1];
                        break;
                    case "AN_FPS_ToADS":
                        ac.animationClips[i] = clips[2];
                        break;
                    case "AN_FPS_ShootHFR":
                        ac.animationClips[i] = clips[3];
                        break;
                    case "AN_FPS_ShootADS":
                        ac.animationClips[i] = clips[4];
                        break;
                    case "AN_FPS_TransitionADS":
                        ac.animationClips[i] = clips[5];
                        break;
                }
            }
        }
    }
}
