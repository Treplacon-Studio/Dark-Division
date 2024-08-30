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
        //If we are swapping weapons you cannot shoot
        var componentHolder = ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder;
        
        //Wait until weapon is loaded
        if (ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent() is null)
            return;
        
        //Cannot shoot when reloading
        if (componentHolder.playerAnimationController.reloadingLock)
        {
            //Release shooting lock, player is not shooting now, because he is reloading
            componentHolder.playerAnimationController.shootingLock = false;
            return;
        }

        //Get shoot key and check if clicked
        var shootKeyClicked = Input.GetMouseButton(0);

        //Check if delay between shoots is over
        var bDelayBetweenShootsIsOver = Time.time >= _fNextFireTime;
        
        //Set stop shooting boolean if player not clicking shoot key but can shoot (delay between shoots is over)
        var bStopShooting = !shootKeyClicked && bDelayBetweenShootsIsOver;

        //Set variable that says if player has any ammo in mag
        var bHasAmmo = componentHolder.bulletPoolingManager.GetAmmoPrimary() > 0;

        //We lock action for shooting when player click shooting key and can shoot (delay between shoots is over)
        componentHolder.playerAnimationController.shootingLock = shootKeyClicked && bDelayBetweenShootsIsOver;
        
        //Inform animator to change stop shooting parameter
        componentHolder.playerAnimationController.StopShooting(bStopShooting);
        
        //Trigger for animator to know that is returning to ADS/HFR from shooting.
        componentHolder.playerAnimationController.TriggerStopShooting(bStopShooting && !_bLastStopShooting);
        
        //Save value of the bStopShooting variable from last frame
        _bLastStopShooting = bStopShooting;
        
        //Player not clicked shooting button or delay between shots is not over yet - shoot won't happen
        if (!shootKeyClicked || !bDelayBetweenShootsIsOver || !bHasAmmo)
            return;
        
        //Get a weapon of current weapon ID (current - the one we have in hands)
        var currentWeaponID = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.GetCurrentWeaponID();
        
        //We take a weapon to variable
        var wc = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent();
        
        //We will be shooting in next lines - we start recoil
        wc.gameObject.GetComponent<Recoil>().StartRecoil(0.03f);
        
        //Calculate next time when player will be able to shoot
        _fNextFireTime = Time.time + wc.Info().Stats().FireRate;
        
        //Get the transform place where the bullet will start
        _bulletStartPoint ??= ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent().GetStartPoint().transform;
        
        //Spawn bullet from pool
        componentHolder.bulletPoolingManager.SpawnFromPool(currentWeaponID, _bulletStartPoint.transform);
        
        //Tell animator to show the proper shoot animation
        componentHolder.playerAnimationController.PlayShootAnimation();
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
        var ts = bIsAiming ? _fTransitionState : _fTransitionUnAimed;
        ts = Mathf.Clamp01(ts);
        ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder.playerAnimationController
            .SetShootingTransitionState(ts);
        
        //Update aiming state for next frame.
        _bLastAiming = bIsAiming;
    }
    
    #endregion Specific Methods
}