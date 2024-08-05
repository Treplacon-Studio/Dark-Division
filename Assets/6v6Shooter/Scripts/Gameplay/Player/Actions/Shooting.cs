using UnityEngine;
using Photon.Pun;

/// <summary>
/// Class handles shooting feature.
/// </summary>
[RequireComponent(typeof(AnimationClipsHolder))]
public class Shooting : MonoBehaviourPunCallbacks
{
    #region Base Parameters
    
    [Header("Basic action setup.")]
    
    [SerializeField] [Tooltip("Player network controller component.")]
    private PlayerNetworkController pnc;
    
    #endregion Base Parameters
    
    #region Specific Parameters

    private Transform _bulletStartPoint;
    private float _fNextFireTime;
    private float _fTransitionState, _fTransitionUnAimed, _fTransitionStartTime;
    private bool _bLastAiming, _bLastStopShooting;
    
    #endregion Specific Parameters

    #region Base Methods
    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Shooting = this;
        if (ActionsManager.GetInstance(pnc.GetInstanceID())?.Switching is not null && ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent() is not null)
            _bulletStartPoint = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent().GetStartPoint().transform;
    }
    
    /// <summary>
    /// Called every frame method for action handle.
    /// </summary>
    public void Run()
    {
        if (ActionsManager.GetInstance(pnc.GetInstanceID())?.Switching is not null && ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent() is not null)
            _bulletStartPoint ??= ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent().GetStartPoint().transform;
        AutomaticFire();
    }
    
    #endregion Base Methods

    #region Specific Methods
    
    /// <summary>
    /// Photon invoker.
    /// </summary>
    private void AutomaticFire()
    {
        if (pnc.PhotonViewIsMine())
        {
            SetAdsHrfTransitionParameter();
            photonView.RPC("RPC_AutomaticFire", RpcTarget.All);
        }
    }

    /// <summary>
    /// Pun RPC method that handles weapon shooting.
    /// </summary>
    [PunRPC]
    public void RPC_AutomaticFire()
    {
        var componentHolder = ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder;
        if (ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent() is null)
            return;
        
        if (componentHolder.playerAnimationController.reloadingLock)
        {
            componentHolder.playerAnimationController.shootingLock = false;
            return;
        }

        var shootKeyClicked = Input.GetMouseButton(0);
        var bStopShooting = !shootKeyClicked && Time.time >= _fNextFireTime;
        componentHolder.playerAnimationController.shootingLock = shootKeyClicked && Time.time >= _fNextFireTime;
        componentHolder.playerAnimationController.StopShooting(bStopShooting);
        
        //Trigger for animator to know that is returning to ADS/HFR from shooting.
        componentHolder.playerAnimationController.TriggerStopShooting(bStopShooting && !_bLastStopShooting);
        
        _bLastStopShooting = bStopShooting;
        
        if (!shootKeyClicked || !(Time.time >= _fNextFireTime))
            return;
        
        var currentWeaponID = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.GetCurrentWeaponID();
        
        //No ammo
        if (componentHolder.bulletPoolingManager.GetAmmoPrimary() <= 0)
            return;

        var wc = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent();
        wc.gameObject.GetComponent<Recoil>().StartRecoil(0.03f);
        _fNextFireTime = Time.time + wc.Info().Stats().FireRate;
        _bulletStartPoint ??= ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent().GetStartPoint().transform;
       
        componentHolder.bulletPoolingManager.SpawnFromPool(currentWeaponID, _bulletStartPoint.transform);
        componentHolder.playerAnimationController.PlayShootAnimation(ActionsManager.GetInstance(pnc.GetInstanceID()).Aiming.IsAiming());
    }

    /// <summary>
    /// Controls transition state between shooting ADS and HFR.
    /// </summary>
    private void SetAdsHrfTransitionParameter()
    {
        //On aiming start set transition start time
        var bIsAiming = ActionsManager.GetInstance(pnc.GetInstanceID()).Aiming.IsAiming();
        if (bIsAiming && !_bLastAiming)
            _fTransitionStartTime = Time.time;
        
        //Get normal HFR and ADS transition time.
        //TODO: Later with attachments shortening this time it will be replaced by weapon parameter.
        var animHolder = GetComponent<AnimationClipsHolder>();
        var currentWeapon = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent();
        var iWeaponId = (int)currentWeapon.Info().Name();
        var transitionAnimLen = animHolder.baseWeaponAnimations[iWeaponId].ads.length;
        
        //Clamped 01 value of transition state to set in animator.
        _fTransitionState = Mathf.Clamp01((Time.time - _fTransitionStartTime)/transitionAnimLen);

        //If aiming reset shooting transition
        if (bIsAiming)
            _fTransitionUnAimed = _fTransitionState;
        
        //If not aiming, weapon will go back to HFR slowly
        if(_fTransitionUnAimed > 0)
            _fTransitionUnAimed -= _fTransitionState * transitionAnimLen/Time.deltaTime;
        
        //Apply state in case of aiming or not
        ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder.playerAnimationController
            .SetShootingTransitionState(bIsAiming ? _fTransitionState : _fTransitionUnAimed);
        
        //Update aiming state for next frame.
        _bLastAiming = bIsAiming;
    }
    
    #endregion Specific Methods
}