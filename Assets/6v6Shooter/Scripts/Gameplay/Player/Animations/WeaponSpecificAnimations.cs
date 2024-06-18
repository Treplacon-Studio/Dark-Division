using _6v6Shooter.Scripts.Gameplay.Player.Weapons;
using UnityEditor.Animations;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Animations
{
    public class WeaponSpecificAnimations : MonoBehaviour
    {
        [SerializeField] [Tooltip("Reloading animation.")]
        private Animator anim;

        [SerializeField] private AnimationClip[] scarClips = new AnimationClip[7];
        [SerializeField] private AnimationClip[] m4Clips = new AnimationClip[7];
        [SerializeField] private AnimationClip[] dsrClips = new AnimationClip[7];

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
                    ChangeWeaponSpecificAnimations(dsrClips);
                    break;
            }
        }
        
        private void ChangeWeaponSpecificAnimations(AnimationClip[] clips)
        {
            var ac = anim.runtimeAnimatorController as AnimatorController;
            if (ac != null)
            {
                var stateMachine = ac.layers[1].stateMachine;
                ChangeClipsInStateMachine(stateMachine, clips);
            }

            anim.runtimeAnimatorController = ac;
        }

        private void ChangeClipsInStateMachine(AnimatorStateMachine stateMachine, AnimationClip[] clips)
        {
            foreach (var state in stateMachine.states)
            {
                switch (state.state.name)
                {
                    case "AN_FPS_Inspect":
                        state.state.motion = clips[0];
                        break;
                    case "AN_FPS_Reload":
                        state.state.motion = clips[1];
                        break;
                    case "AN_FPS_ToAds":
                        state.state.motion = clips[2];
                        break;
                    case "AN_FPS_ShootHFR":
                        state.state.motion = clips[3];
                        break;
                    case "AN_FPS_ShootADS":
                        state.state.motion = clips[4];
                        break;
                    case "AN_FPS_TransitionADS":
                        state.state.motion = clips[5];
                        break;
                    case "AN_FPS_Default":
                        state.state.motion = clips[6];
                        break;
                }
            }
            
            foreach (var subStateMachine in stateMachine.stateMachines)
                ChangeClipsInStateMachine(subStateMachine.stateMachine, clips);
        }
        
    
    }
}
