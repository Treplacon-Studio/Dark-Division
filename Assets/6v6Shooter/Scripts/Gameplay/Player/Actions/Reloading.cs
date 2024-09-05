using System.Collections;
using _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem;
using _6v6Shooter.Scripts.Audio_Scripts.NewAudioSystem.Helpers;
using UnityEngine;

/// <summary>
/// Class handles reloading feature.
/// </summary>
public class Reloading : MonoBehaviour
{
    #region Base Properties
    
    [Header("Basic action setup.")]
    
    [SerializeField] [Tooltip("Player network controller component.")]
    private PlayerNetworkController pnc;

    #endregion Base Properties
    
    #region Specific Properties
    
    [Header("Specific action setup.")]
    
    [SerializeField] [Tooltip("Left hand.")]
    private GameObject leftHand;

    private LocalTransformStructure _tWeaponMagSocket;

    private bool _locker;
    
    #endregion Specific Properties

    #region Structures 
    
    /// <summary>
    /// Special structure for keep transform elements localy.
    /// </summary>
    private readonly struct LocalTransformStructure
    {
        private readonly Vector3 _v3LocalPosition;
        private readonly Quaternion _v3LocalRotation;
        private readonly Vector3 _v3LocalScale;

        public LocalTransformStructure(Transform tTransform)
        {
            _v3LocalPosition = tTransform.localPosition;
            _v3LocalRotation = tTransform.localRotation;
            _v3LocalScale = tTransform.localScale;
        }

        public void ApplyToTransform(Transform tTargetTransform)
        {
            tTargetTransform.localPosition = _v3LocalPosition;
            tTargetTransform.localRotation = _v3LocalRotation;
            tTargetTransform.localScale = _v3LocalScale;
        }
    }
    
    #endregion Structures
    
    #region Base Methods
    
    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Reloading = this;
    }

    /// <summary>
    /// Called every frame method for action handle.
    /// </summary>
    public void Run()
    {
        var componentHolder = ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder;
        if (Input.GetKeyDown(KeyCode.R) && !componentHolder.playerAnimationController.IsLocked())
        {
            StartCoroutine(LockTemporarily());
            var bCanReload = true;
            foreach (var s in componentHolder.playerAnimationController.weaponActionsStates)
            {
                if (s == componentHolder.playerAnimationController.weaponActionsStates[2] && componentHolder.playerAnimationController.aimingLock)
                {
                    ActionsManager.GetInstance(pnc.GetInstanceID()).Aiming.DisableScope();
                    break;
                }

                if (componentHolder.playerAnimationController.InProgress(s, 0))
                {
                    bCanReload = false;
                    break;
                }
            }

            if (bCanReload)
            {
                _locker = true;
                componentHolder.playerAnimationController.PlayReloadAnimation();
                
                var currentWeaponID = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.GetCurrentWeaponID();
                SoundEventBuilder.Create()
                    .WithEventType(SoundEvent.Type.Reload)
                    .WithId(currentWeaponID)
                    .PlayOneShotAttached(gameObject);
            }
        }
    }
    
    #endregion Base Methods
    
    #region Specific Methods
    
    /// <summary>
    /// Set an reloading lock for reload animation time and a little bit longer.
    /// </summary>
    private IEnumerator LockTemporarily()
    {
        while (!_locker)
            yield return null;
        
        var componentHolder = ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder;
        var iCurrentWeaponID = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.GetCurrentWeaponID();
        
        componentHolder.playerAnimationController.reloadingLock = true;
        var currentWeapon = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent();
        var clip = GetComponent<AnimationClipsHolder>()
            .baseWeaponAnimations[(int)currentWeapon.Info().Name()].reload;
        
        if(clip is null)
            Debug.LogError("Reloading clip has not been attached.");
        
        yield return new WaitForSeconds(clip!.length - 0.5f);

        componentHolder.bulletPoolingManager.ResetAmmo(iCurrentWeaponID);
        componentHolder.playerAnimationController.reloadingLock = false;

        _locker = false;
    }

    /// <summary>
    /// Change parenting settings for mag while reloading.
    /// </summary>
    /// <param name="sEventName">String deciding to attach mag to player hand or un-attach.</param>
    public void HandleReloadEvent(string sEventName)
    {
        if (sEventName == "reloadMagTake")
            OnReloadMagTakeEvent();
        else if (sEventName == "reloadMagReturn")
            OnReloadMagReturnEvent();
    }

    /// <summary>
    /// Attach a mag from weapon to player hand.
    /// </summary>
    private void OnReloadMagTakeEvent()
    {
        if (ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent() is null)
            return;
        var t = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent().GetMag().transform;
        _tWeaponMagSocket = new LocalTransformStructure(t);
        t.transform.parent = leftHand.transform;
    }

    /// <summary>
    /// Attach a mag from player hand to weapon.
    /// </summary>
    private void OnReloadMagReturnEvent()
    {
        if (ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent() is null)
            return;
        var t = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent().GetMag().transform;
        t.transform.parent = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent().GetMagSocket().transform;
        _tWeaponMagSocket.ApplyToTransform(t.transform);
    }
    
    #endregion Specific Methods
}