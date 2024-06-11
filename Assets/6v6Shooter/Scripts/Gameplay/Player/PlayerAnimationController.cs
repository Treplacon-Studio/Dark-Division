using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace _6v6Shooter.Scripts.Gameplay.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] [Tooltip("Left hand inverse kinematic constraint.")]
        private TwoBoneIKConstraint leftHandIK;
        
        [SerializeField] [Tooltip("Character animator.")]
        private Animator anim;

        [SerializeField] [Tooltip("List of animations that needs left hand IK to be disabled.")]
        private string[] animationsWithoutLeftHandIK =
        {
            "AN_FPS_Scar_Reload",
            "AN_FPS_Scar_Inspect",
        };
        
        //Reloading weapon stuff
        [SerializeField] [Tooltip("Weapon mag.")]
        private GameObject weaponMag;

        [SerializeField] [Tooltip("Weapon mag socket.")]
        private GameObject weaponMagSocket;
        
        [SerializeField] [Tooltip("Left hand.")]
        private GameObject leftHand;

        public struct LocalTransformStructure
        {
            public Vector3 LocalPosition;
            public Quaternion LocalRotation;
            public Vector3 LocalScale;

            public LocalTransformStructure(Transform transform)
            {
                LocalPosition = transform.localPosition;
                LocalRotation = transform.localRotation;
                LocalScale = transform.localScale;
            }

            public void ApplyToTransform(Transform targetTransform)
            {
                targetTransform.localPosition = LocalPosition;
                targetTransform.localRotation = LocalRotation;
                targetTransform.localScale = LocalScale;
            }
        }
        
        private LocalTransformStructure tWeaponMagSocket;
        
        
        private bool _leftHandIKActive;
        
        void Update()
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
                        SwitchLeftHandIK(true);
                }
            }
        }
        public void PlayAimDownSightAnimation()
        {
            anim.SetTrigger("aimSight");
        }

        public void PlayStopAimDownSightAnimation()
        {
            anim.SetTrigger("unaimSight");
        }

        public void PlayShootAnimation()
        {
            Debug.Log("Shooting..");
            //Replace with shooting animation
        }

        public void PlaySprintAnimation(bool isSprinting)
        {
            anim.SetBool("isSprinting", isSprinting);
        }

        public void PlayJumpAnimation()
        {
            anim.SetTrigger("jump");
        } 
        public void PlayInspectAnimation()
        {
            SwitchLeftHandIK(false);
            anim.SetTrigger("inspectWeapon");
        }

        public void PlayReloadAnimation()
        {
            SwitchLeftHandIK(false);
            anim.SetTrigger("reloadWeapon");
        }

        public void SetIsGroundedAnim(bool isGrounded)
        {
            anim.SetBool("isGrounded", isGrounded);
        }

        public void PlayWalkingAnimation(Vector2 movementInput)
        {
            anim.SetFloat("Horizontal", movementInput.x);
            anim.SetFloat("Vertical", movementInput.y);
        }

        private void SwitchLeftHandIK(bool state)
        {
            _leftHandIKActive = state;
            leftHandIK.weight = state ? 1f : 0f;
        }

        
        //Animation events
        
        public void HandleAnimationEvent(string eventName)
        {
            if(eventName == "reloadMagTake")
                OnReloadMagTakeEvent();
            else if (eventName == "reloadMagReturn")
                OnReloadMagReturnEvent();
        }

        private void OnReloadMagTakeEvent()
        {
            tWeaponMagSocket = new LocalTransformStructure(weaponMag.transform);
            weaponMag.transform.parent = leftHand.transform;
        }

        private void OnReloadMagReturnEvent()
        {
            weaponMag.transform.parent = weaponMagSocket.transform;
            tWeaponMagSocket.ApplyToTransform(weaponMag.transform);
        }
    }
}
