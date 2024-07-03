using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class Reloading : MonoBehaviour
{
    [SerializeField] private PlayerNetworkController pnc;
    
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;

    [SerializeField] [Tooltip("Left hand.")]
    private GameObject leftHand;

    [SerializeField] [Tooltip("Clips for specific weapon animations.")]
    private WeaponAnimation[] clips;

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
        ActionsManager.GetInstance(pnc.GetInstanceID()).Reloading = this;
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
                    ActionsManager.GetInstance(pnc.GetInstanceID()).Aiming.DisableScope();
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
        var currentWeaponID = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.GetCurrentWeaponID();
        
        componentHolder.playerAnimationController.reloadingLock = true;
        var animator = componentHolder.playerAnimationController.anim;
        AnimationClip clip = null;
        var currentWeapon = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent();
        foreach(var elem in clips)
            if (currentWeapon != null && elem.name == currentWeapon.Info().Name())
                clip = elem.clip;
        
        if(clip is null)
            Debug.LogError("Reloading clip has not been attached.");
        
        yield return new WaitForSeconds(clip!.length + 0.05f);

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
        if (ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent() is null)
            return;
        var t = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent().GetMag().transform;
        _tWeaponMagSocket = new LocalTransformStructure(t);
        t.transform.parent = leftHand.transform;
    }

    private void OnReloadMagReturnEvent()
    {
        if (ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent() is null)
            return;
        var t = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent().GetMag().transform;
        t.transform.parent = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent().GetMagSocket().transform;
        _tWeaponMagSocket.ApplyToTransform(t.transform);
    }
}