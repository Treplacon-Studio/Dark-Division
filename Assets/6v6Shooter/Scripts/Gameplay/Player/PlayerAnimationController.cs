using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

namespace _6v6Shooter.Scripts.Gameplay.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        //*----------------[IK (soon unused)]------------------
        [SerializeField] [Tooltip("Left hand inverse kinematic constraint.")]
        private TwoBoneIKConstraint leftHandIK;
        
        [SerializeField] [Tooltip("List of animations that needs left hand IK to be disabled.")]
        private string[] animationsWithoutLeftHandIK =
        {
            "AN_FPS_Scar_Reload",
            "AN_FPS_Scar_Inspect",
        };

        [SerializeField] [Tooltip("List of all weapon animation states names that cannot .")]
        public string[] weaponActionsStates =
        {
            "AN_FPS_Scar_Reload", //Reloading
            "AN_FPS_Scar_Inspect", //Inspecting
            "AN_FPS_Scar_ToAds" //Aiming
        };
        
        private bool _leftHandIKActive;

        private void HandleIK()
        {
            var currentState = anim.GetCurrentAnimatorStateInfo(0);
            if (!_leftHandIKActive)
            {
                foreach (var a in animationsWithoutLeftHandIK)
                {
                    if (!currentState.IsName(a)) 
                        continue;
                    
                    //Enable again just before animation end
                    //1.0 not always triggers because of float precision
                    if (currentState.normalizedTime >= 0.99f)
                        SwitchLeftHandIK(false);
                }
            }
        }
        
        private void SwitchLeftHandIK(bool state)
        {
            _leftHandIKActive = state;
            leftHandIK.weight = state ? 1f : 0f;
        }
        //-----------------[IK (soon unused)]-----------------*
        
        
        
        [SerializeField] [Tooltip("Character animator.")]
        private Animator anim;

        public bool reloadingLock;
        public bool inspectingLock;
        public bool aimingLock;
        
        void Update()
        { 
            //*----------------[IK (soon unused)]------------------
            HandleIK();
            //-----------------[IK (soon unused)]-----------------*
        }

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
        public void PlayShootAnimation()
        {
            Debug.Log("Shooting..");
            //Replace with shooting animation
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
        public void PlayJumpAnimation()
        {
            anim.SetTrigger("pJumping");
        }

        //Landing
        public void PlayLandFromAir(bool isLanding)
        {
            anim.SetBool("pLanding", isLanding);
        }
        
        //Inspecting
        public void PlayInspectAnimation()
        {
            SwitchLeftHandIK(false);
            anim.SetTrigger("pInspectingWeapon");
        }
        
        //Reloading
        public void PlayReloadAnimation()
        {
            SwitchLeftHandIK(false);
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
