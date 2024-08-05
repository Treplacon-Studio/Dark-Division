using System.Collections;
using UnityEngine;

/// <summary>
/// Class handles aiming in two modes.
/// </summary>
public class Aiming : MonoBehaviour
{
    #region Base Properties
    
    [Header("Basic action setup.")]
    
    [SerializeField] [Tooltip("Network controller component.")]
    private PlayerNetworkController pnc;
    
    #endregion Base Properties

    #region Specific Properties

    [Header("Specific action setup.")] 
    
    [SerializeField] [Tooltip("AimMode")]
    private AimMode aimMode;
    
    [SerializeField] [Tooltip("Default FOV of the camera.")]
    private int iDefaultFOV;

    [SerializeField] [Tooltip("Scope multiplier, says how many times zoom the camera.")]
    private float fScopeMultiplier;

    [SerializeField] [Tooltip("Time to zoom and un-zoom.")]
    private float fAimTime;
    
    [SerializeField] [Tooltip("Additional aim lock time over the ADS animation time.")]
    private float fExtraAimLockTime;

    private enum AimMode
    {
        Hold,
        Toggle
    }

    private bool _bIsAiming;
    private Coroutine _cZoom;
    private Camera _camFPS;
    
    #endregion Specific Properties

    #region Base Methods
    
    private void Awake()
    {
        //Run action only if you are it's owner
        if (GetComponentInParent<PlayerNetworkController>().PhotonViewIsMine() is false)
            return;
        
        //Automatically find the proper camera
        var cameras = FindObjectsOfType<Camera>();
        foreach (var cam in cameras)
        {
            if (!cam.gameObject.name.Contains("Main Camera")) 
                continue;
            _camFPS = cam;
            break;
        }
        
        //Set instance to multi-singleton
        ActionsManager.GetInstance(pnc.GetInstanceID()).Aiming = this;
    }

    /// <summary>
    /// Called every frame method for action handle.
    /// </summary>
    public void Run()
    {
        var componentHolder = ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder;
        var bCanAnim = true;
        foreach (var s in ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder.playerAnimationController.weaponActionsStates)
        {
            if (componentHolder.playerAnimationController.InProgress(s, 0))
            {
                bCanAnim = false;
                break;
            }
        }

        //Handle hold aim mode 
        if (aimMode == AimMode.Hold)
        {
            if (Input.GetMouseButtonDown(1) && bCanAnim)
            {
                StartCoroutine(LockTemporarily());
                if (componentHolder.playerAnimationController.IsLocked()) 
                    return;
                _bIsAiming = true;
                EnableScope();
            }

            if (Input.GetMouseButtonUp(1))
            {
                _bIsAiming = false;
                DisableScope();
            }
        }
        
        //Handle toggle aim mode
        else if (aimMode == AimMode.Toggle)
        {
            var bLocked = false;
            if (Input.GetMouseButtonDown(1) && !_bIsAiming && bCanAnim)
            {
                StartCoroutine(LockTemporarily());
                if (componentHolder.playerAnimationController.IsLocked()) 
                    return;
                _bIsAiming = true;
                bLocked = true;
                EnableScope();
            }

            if (Input.GetMouseButtonDown(1) && _bIsAiming && !bLocked)
            {
                _bIsAiming = false;
                DisableScope();
            }
        }
        
        //Update info for animator if player is still aiming
        componentHolder.playerAnimationController.SetAimState(IsAiming());
    }
    
    #endregion Base Methods

    #region Specific Methods

    /// <summary>
    /// Set an aiming lock for ADS animation time and a little bit longer.
    /// </summary>
    private IEnumerator LockTemporarily()
    {
        var componentHolder = ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder;
        componentHolder.playerAnimationController.aimingLock = true;
        
        var currentWeapon = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent();
        var clip = GetComponent<AnimationClipsHolder>()
            .baseWeaponAnimations[(int)currentWeapon.Info().Name()].ads;
        
        yield return new WaitForSeconds(clip.length + fExtraAimLockTime);
        componentHolder.playerAnimationController.aimingLock = false;
    }

    /// <summary>
    /// Triggers zoom/un-zoom operation
    /// </summary>
    /// <param name="zoomed">Parameter that decide to zoom or un-zoom camera.</param>
    private void ScopeZoom(bool zoomed)
    {
        if (_cZoom != null)
            StopCoroutine(_cZoom);
        _cZoom = StartCoroutine(AnimateZoom(zoomed));
    }

    /// <summary>
    /// Applies async zooming transition in aim time.
    /// </summary>
    /// <param name="zoomed">Parameter that decide to zoom or un-zoom camera.</param>
    private IEnumerator AnimateZoom(bool zoomed)
    {
        var startFOV = _camFPS.fieldOfView;
        var endFOV = zoomed ? Mathf.Clamp(iDefaultFOV / fScopeMultiplier, 0, iDefaultFOV) : iDefaultFOV;
        var elapsedTime = 0f;

        //Make interpolated transition until aim time reached
        while (elapsedTime < fAimTime)
        {
            _camFPS.fieldOfView = Mathf.Lerp(startFOV, endFOV, elapsedTime / fAimTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Make sure that final fov not exceed the border
        _camFPS.fieldOfView = endFOV;
    }

    /// <summary>
    /// Enables scope on player camera.
    /// </summary>
    public void EnableScope()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder
            .playerAnimationController.PlayAimDownSightAnimation();
        ScopeZoom(true);
    }

    /// <summary>
    /// Disables scope on player camera.
    /// </summary>
    public void DisableScope()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder
            .playerAnimationController.PlayStopAimDownSightAnimation();
        ScopeZoom(false);
    }
    
    #endregion Specific Methods
    
    #region Accessors
    
    /// <summary>
    /// Getter method.
    /// </summary>
    /// <returns>Information if player is currently aiming.</returns>
    public bool IsAiming()
    {
        return _bIsAiming;
    }
    
    #endregion Accessors
}