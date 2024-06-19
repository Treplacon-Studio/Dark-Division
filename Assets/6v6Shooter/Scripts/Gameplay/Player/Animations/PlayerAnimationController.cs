using System.Collections;
using _6v6Shooter.Scripts.Gameplay.Player.Actions;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Animations
{
    [RequireComponent(typeof(Jumping))]
    [RequireComponent(typeof(Walking))]
    [RequireComponent(typeof(Sprinting))]
    [RequireComponent(typeof(Aiming))]
    [RequireComponent(typeof(Reloading))]
    [RequireComponent(typeof(Inspecting))]
    [RequireComponent(typeof(Crouching))]
    [RequireComponent(typeof(Shooting))]
    [RequireComponent(typeof(Switching))]
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] [Tooltip("List of all weapon animation states names that cannot .")]
        public string[] weaponActionsStates =
        {
            "AN_FPS_Scar_Reload", //Reloading
            "AN_FPS_Scar_Inspect", //Inspecting
            "AN_FPS_Scar_ToAds" //Aiming
        };
        
        [SerializeField] [Tooltip("Character animator.")]
        public Animator anim;

        public bool reloadingLock;
        public bool inspectingLock;
        public bool aimingLock;
        
        public bool InProgress(string stateName, int layerIndex)
        {
            var currentState = anim.GetCurrentAnimatorStateInfo(layerIndex);
            return currentState.IsName(stateName);
        }

        public bool IsLocked()
        {
            return reloadingLock || inspectingLock;
        }
        
        //Aiming
        public void PlayAimDownSightAnimation()
        {
            anim.SetTrigger("pAimSight");
            anim.ResetTrigger("pUnaimSight");
        }

        public void PlayStopAimDownSightAnimation()
        {
            anim.SetTrigger("pUnaimSight");
            anim.ResetTrigger("pAimSight");
        }
        
        //Shooting
        public void PlayShootAnimation(bool isAiming)
        {
            anim.SetTrigger(isAiming ? "pShootingADS" : "pShooting");
        }

        //Crouching
        public void PlayCrouchAnimation(bool isCrouching)
        {
            anim.SetBool("pCrouching", isCrouching);
        }
        
        //Sprinting
        public void PlaySprintAnimation(bool isSprinting)
        {
            anim.SetBool("pSprinting", isSprinting);
        }

        //Jumping
        public bool PlayJumpAnimation(bool landing, bool grounded)
        {
            if (grounded && !landing)
            {
                anim.SetBool("pJumping", false);
                anim.SetBool("pLanding", false);
            }
            else if (!grounded && !landing)
            {
                anim.SetBool("pJumping", true);
                anim.SetBool("pLanding", false);
            }
            else if (!grounded)
            {
                anim.SetBool("pJumping", false);
                anim.SetBool("pLanding", true);
            }
            
            return !(anim.GetBool("pJumping") || anim.GetBool("pLanding"));
        }
        
        //Inspecting
        public void PlayInspectAnimation()
        {
            anim.SetTrigger("pInspectingWeapon");
        }
        
        //Reloading
        public void PlayReloadAnimation()
        {
            anim.SetTrigger("pReloadingWeapon");
        }
        
        //Walking
        public void PlayWalkingAnimation(Vector2 movementInput, bool isGrounded)
        {
            anim.SetFloat("pHorizontal", movementInput.x);
            anim.SetFloat("pVertical", movementInput.y);
            anim.SetBool("pGrounded", isGrounded);
        }
    }
}
