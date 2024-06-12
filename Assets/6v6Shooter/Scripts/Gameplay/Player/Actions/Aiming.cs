using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Aiming : MonoBehaviour
    {
        [SerializeField] [Tooltip("Player animation controller.")]
        private PlayerAnimationController pac;
        
        [SerializeField] [Tooltip("Camera for first person view.")]
        private Camera fpsCamera;

        [SerializeField] [Tooltip("Default FOV of the camera.")]
        private int defaultFOV;
        
        [SerializeField] [Tooltip("Scope multiplier, says how many times zoom the camera.")]
        private float scopeMultiplier;
        
        [SerializeField] [Tooltip("Time to zoom and un-zoom.")]
        private float aimTime;
        
        public enum AimMode {Hold, Toggle}
        private bool _isAiming;
        private Coroutine _zoomCoroutine;
        
        private void Awake()
        {
            ActionsManager.Instance.Aiming = this;
        }
        
        public void Run(AimMode aimMode)
        {
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
                    StartCoroutine(LockTemporarily());
                    if (pac.IsLocked()) return;
                    EnableScope();
                }

                if (Input.GetMouseButtonUp(1))
                {
                    DisableScope();
                }
            }
            else if (aimMode == AimMode.Toggle)
            {
                var locked = false;
                if (Input.GetMouseButtonDown(1) && !_isAiming && canAim)
                {
                    StartCoroutine(LockTemporarily());
                    if (pac.IsLocked()) return;
                    _isAiming = true;
                    locked = true;
                    EnableScope();
                }

                if (Input.GetMouseButtonDown(1) && _isAiming && !locked)
                {
                    _isAiming = false;
                    DisableScope();
                }
            }
        }
        
        private IEnumerator LockTemporarily()
        {
            pac.aimingLock = true;
            yield return new WaitForSeconds(1f);
            pac.aimingLock = false;
        }

        private void ScopeZoom(bool zoomed)
        {
            if (_zoomCoroutine != null)
            {
                StopCoroutine(_zoomCoroutine);
            }
            _zoomCoroutine = StartCoroutine(AnimateZoom(zoomed));
        }
        
        private IEnumerator AnimateZoom(bool zoomed)
        {
            var startFOV = fpsCamera.fieldOfView;
            var endFOV = zoomed ? Mathf.Clamp(defaultFOV / scopeMultiplier, 0, defaultFOV) : defaultFOV;
            var elapsedTime = 0f;

            while (elapsedTime < aimTime)
            {
                fpsCamera.fieldOfView = Mathf.Lerp(startFOV, endFOV, elapsedTime / aimTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            fpsCamera.fieldOfView = endFOV;
        }

        public void EnableScope()
        {
            pac.PlayAimDownSightAnimation();
            ScopeZoom(true);
        }

        public void DisableScope()
        {
            pac.PlayStopAimDownSightAnimation();
            ScopeZoom(false);
        }
    }
}
