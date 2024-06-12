using System.Collections;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Reloading : MonoBehaviour
    {
        [SerializeField] [Tooltip("Player animation controller.")]
        private PlayerAnimationController pac;
        
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
        
        public void Run()
        {
            if (Input.GetKeyDown(KeyCode.R) && !pac.IsLocked())
            {
                StartCoroutine(LockTemporarily());

                var canReload = true;
                foreach (var s in pac.weaponActionsStates)
                {
                    if (s == pac.weaponActionsStates[2])
                    {
                        pac.PlayStopAimDownSightAnimation();
                        break;
                    }

                    if (pac.InProgress(s, 1))
                    {
                        canReload = false;
                        break;
                    }
                }

                if (canReload)
                    pac.PlayReloadAnimation();
            }
        }

        private IEnumerator LockTemporarily()
        {
            pac.reloadingLock = true;
            yield return new WaitForSeconds(1f);
            pac.reloadingLock = false;
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
