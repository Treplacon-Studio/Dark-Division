using System.Collections;
using UnityEngine;
using Photon.Pun;


[RequireComponent(typeof(AnimationClipsHolder))]
public class Shooting : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerNetworkController pnc;
    
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;

    private Transform _bulletStartPoint;
    private float _fNextFireTime;
    private float _fTransitionState, _fTransitionUnAimed, _fTransitionStartTime;
    private bool _bLastAiming, _bLastStopShooting;

    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Shooting = this;
        if (ActionsManager.GetInstance(pnc.GetInstanceID())?.Switching is not null && ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent() is not null)
            _bulletStartPoint = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent().GetStartPoint().transform;
    }

    /// <summary>
    /// Photon <c>RPC_AutomaticFire</c> invoker.
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
    /// Method <c>SetAdsHrfTransitionParameter</c> controls transition state between shooting ADS and HFR.
    /// </summary>
    private void SetAdsHrfTransitionParameter()
    {
        //On aiming start set transition start time
        var isAiming = ActionsManager.GetInstance(pnc.GetInstanceID()).Aiming.IsAiming();
        if (isAiming && !_bLastAiming)
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
        if (isAiming)
            _fTransitionUnAimed = _fTransitionState;
        
        //If not aiming, weapon will go back to HFR slowly
        if(_fTransitionUnAimed > 0)
            _fTransitionUnAimed -= _fTransitionState * transitionAnimLen/Time.deltaTime;
        
        //Apply state in case of aiming or not
        ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder.playerAnimationController
            .SetShootingTransitionState(isAiming ? _fTransitionState : _fTransitionUnAimed);
        
        //Update aiming state for next frame.
        _bLastAiming = isAiming;
    }
    
    /// <summary>
    /// Treat <c>Run</c> method as update. It runs in movement update.
    /// </summary>
    public void Run()
    {
        if (ActionsManager.GetInstance(pnc.GetInstanceID())?.Switching is not null && ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent() is not null)
            _bulletStartPoint ??= ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent().GetStartPoint().transform;
        AutomaticFire();
    }
}