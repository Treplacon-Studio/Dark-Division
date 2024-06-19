using System;
using System.Collections;
using _6v6Shooter.Scripts.Gameplay.Player.Animations;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.PackageManager;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Reloading : MonoBehaviour
    {
        private PlayerAnimationController _pac;
        
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

                    if (_pac.InProgress(s, 0))
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
            
            var ac = _pac.anim.runtimeAnimatorController as AnimatorController;
            var animTime = 0f;
            if (ac != null)
            {
                var stateMachine = ac.layers[1].stateMachine;
                foreach (var state in stateMachine.states)
                {
                    if (state.state.name == "AN_FPS_Reload")
                    {
                        var clip = state.state.motion as AnimationClip;
                        if (clip == null) continue;
                        animTime = clip.length + 0.05f;
                        break;
                    }
                }
            }
                
            yield return new WaitForSeconds(animTime);
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
            var t = ActionsManager.Instance.Switching.WeaponComponent().GetMag().transform;
            _tWeaponMagSocket = new LocalTransformStructure(t);
            t.transform.parent = leftHand.transform;
        }

        private void OnReloadMagReturnEvent()
        {
            var t = ActionsManager.Instance.Switching.WeaponComponent().GetMag().transform;
            t.transform.parent = ActionsManager.Instance.Switching.WeaponComponent().GetMagSocket().transform;
            _tWeaponMagSocket.ApplyToTransform(t.transform);
        }
    }
}
