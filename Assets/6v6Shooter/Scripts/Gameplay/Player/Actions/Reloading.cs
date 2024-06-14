using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Reloading : MonoBehaviour
    {
        private PlayerAnimationController _pac;
        
        [SerializeField] [Tooltip("Weapon mag.")]
        private GameObject weaponMag;

        [SerializeField] [Tooltip("Weapon mag socket.")]
        private GameObject weaponMagSocket;
        
        [SerializeField] [Tooltip("Left hand.")]
        private GameObject leftHand;
        
        private LocalTransformStructure _tWeaponMagSocket;
        
        private readonly struct LocalTransformStructure
        {
            private readonly Vector3 _localPosition;
            private readonly Quaternion _localRotation;
            private readonly Vector3 _localScale;

            public LocalTransformStructure(Transform transform)
            {
                _localPosition = transform.localPosition;
                _localRotation = transform.localRotation;
                _localScale = transform.localScale;
            }

            public void ApplyToTransform(Transform targetTransform)
            {
                targetTransform.localPosition = _localPosition;
                targetTransform.localRotation = _localRotation;
                targetTransform.localScale = _localScale;
            }
        }
        
        private void Awake()
        {
            _pac = GetComponent<PlayerAnimationController>();
            ActionsManager.Instance.Reloading = this;
        }
        
        public void Run()
        {
            if (Input.GetKeyDown(KeyCode.R) && !_pac.IsLocked())
            {
                StartCoroutine(LockTemporarily());

                var canReload = true;
                foreach (var s in _pac.weaponActionsStates)
                {
                    if (s == _pac.weaponActionsStates[2] && _pac.aimingLock)
                    {
                        ActionsManager.Instance.Aiming.DisableScope();
                        break;
                    }

                    if (_pac.InProgress(s, 1))
                    {
                        canReload = false;
                        break;
                    }
                }

                if (canReload)
                    _pac.PlayReloadAnimation();
            }
        }

        private IEnumerator LockTemporarily()
        {
            _pac.reloadingLock = true;
            yield return new WaitForSeconds(1f);
            _pac.reloadingLock = false;
        }
        
        public void HandleReloadEvent(string eventName)
        {
            if(eventName == "reloadMagTake")
                OnReloadMagTakeEvent();
            else if (eventName == "reloadMagReturn")
                OnReloadMagReturnEvent();
        }
        
        private void OnReloadMagTakeEvent()
        {
            _tWeaponMagSocket = new LocalTransformStructure(weaponMag.transform);
            weaponMag.transform.parent = leftHand.transform;
        }

        private void OnReloadMagReturnEvent()
        {
            weaponMag.transform.parent = weaponMagSocket.transform;
            _tWeaponMagSocket.ApplyToTransform(weaponMag.transform);
        }
    }
}
