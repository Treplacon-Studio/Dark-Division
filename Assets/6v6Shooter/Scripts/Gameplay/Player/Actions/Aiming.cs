using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Aiming : MonoBehaviour
    {
        [SerializeField] [Tooltip("Player animation controller.")]
        private PlayerAnimationController pac;
        
        public enum AimMode {Hold, Toggle}
        
        private bool _isAiming;
        
        public void Run(AimMode aimMode)
        {
            StartCoroutine(LockTemporarily());
            if (pac.IsLocked()) return;
            
            var canAim = true;
            foreach (var s in pac.weaponActionsStates)
            {
                if (pac.InProgress(s, 1))
                {
                    canAim = false;
                    break;
                }
            }
            if (aimMode == AimMode.Hold)
            {
                if (Input.GetMouseButtonDown(1) && canAim)
                {
                    pac.PlayAimDownSightAnimation();
                }

                if (Input.GetMouseButtonUp(1))
                {
                    pac.PlayStopAimDownSightAnimation();
                }
            }
            else if (aimMode == AimMode.Toggle)
            {
                var locked = false;
                if (Input.GetMouseButtonDown(1) && !_isAiming && canAim)
                {
                    _isAiming = true;
                    locked = true;
                    pac.PlayAimDownSightAnimation();
                }

                if (Input.GetMouseButtonDown(1) && _isAiming && !locked)
                {
                    _isAiming = false;
                    pac.PlayStopAimDownSightAnimation();
                }
            }
        }
        
        private IEnumerator LockTemporarily()
        {
            pac.aimingLock = true;
            yield return new WaitForSeconds(1f);
            pac.aimingLock = false;
        }
    }
}
