using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class Reloading : MonoBehaviour
{
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;

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
        ActionsManager.Instance.Reloading = this;
    }

    public void Run()
    {
        if (Input.GetKeyDown(KeyCode.R) && !componentHolder.playerAnimationController.IsLocked())
        {
            StartCoroutine(LockTemporarily());

            var canReload = true;
            foreach (var s in componentHolder.playerAnimationController.weaponActionsStates)
            {
                if (s == componentHolder.playerAnimationController.weaponActionsStates[2] && componentHolder.playerAnimationController.aimingLock)
                {
                    ActionsManager.Instance.Aiming.DisableScope();
                    break;
                }

                if (componentHolder.playerAnimationController.InProgress(s, 0))
                {
                    canReload = false;
                    break;
                }
            }

            if (canReload)
                componentHolder.playerAnimationController.PlayReloadAnimation();
        }
    }

    private IEnumerator LockTemporarily()
    {
        var currentWeaponID = ActionsManager.Instance.Switching.GetCurrentWeaponID();
        
        componentHolder.playerAnimationController.reloadingLock = true;
        var animator = componentHolder.playerAnimationController.anim;
        var clip = PlayerUtils.GetClipByStateName(
                animator,  new AnimatorOverrideController(animator.runtimeAnimatorController), "AN_FPS_Reload");
        yield return new WaitForSeconds( clip.length + 0.05f);
        
        componentHolder.bulletPoolingManager.ResetAmmo(currentWeaponID);
        componentHolder.playerAnimationController.reloadingLock = false;
    }

    public void HandleReloadEvent(string eventName)
    {
        if (eventName == "reloadMagTake")
            OnReloadMagTakeEvent();
        else if (eventName == "reloadMagReturn")
            OnReloadMagReturnEvent();
    }

    private void OnReloadMagTakeEvent()
    {
        if (ActionsManager.Instance.Switching.WeaponComponent() is null)
            return;
        var t = ActionsManager.Instance.Switching.WeaponComponent().GetMag().transform;
        _tWeaponMagSocket = new LocalTransformStructure(t);
        t.transform.parent = leftHand.transform;
    }

    private void OnReloadMagReturnEvent()
    {
        if (ActionsManager.Instance.Switching.WeaponComponent() is null)
            return;
        var t = ActionsManager.Instance.Switching.WeaponComponent().GetMag().transform;
        t.transform.parent = ActionsManager.Instance.Switching.WeaponComponent().GetMagSocket().transform;
        _tWeaponMagSocket.ApplyToTransform(t.transform);
    }
}