using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;


public class Aiming : MonoBehaviour
{
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;
    
    private Camera _fpsCamera;

    [SerializeField] [Tooltip("Default FOV of the camera.")]
    private int defaultFOV;

    [SerializeField] [Tooltip("Scope multiplier, says how many times zoom the camera.")]
    private float scopeMultiplier;

    [SerializeField] [Tooltip("Time to zoom and un-zoom.")]
    private float aimTime;

    public enum AimMode
    {
        Hold,
        Toggle
    }

    private bool _isAiming;
    private Coroutine _zoomCoroutine;

    private void Awake()
    {
        if (GetComponentInParent<PlayerNetworkController>().PhotonViewIsMine() is false)
            return;
            
        var cameras = FindObjectsOfType<Camera>();
        foreach (var cam in cameras)
        {
            if (!cam.gameObject.name.Contains("Main Camera")) continue;
            _fpsCamera = cam;
            break;
        }
        ActionsManager.Instance.Aiming = this;
    }

    public void Run(AimMode aimMode)
    {
        var canAim = true;
        foreach (var s in componentHolder.playerAnimationController.weaponActionsStates)
        {
            if (componentHolder.playerAnimationController.InProgress(s, 0))
            {
                canAim = false;
                break;
            }
        }

        if (aimMode == AimMode.Hold)
        {
            if (Input.GetMouseButtonDown(1) && canAim)
            {
                StartCoroutine(LockTemporarily());
                if (componentHolder.playerAnimationController.IsLocked()) return;
                _isAiming = true;
                EnableScope();
            }

            if (Input.GetMouseButtonUp(1))
            {
                _isAiming = false;
                DisableScope();
            }
        }
        else if (aimMode == AimMode.Toggle)
        {
            var locked = false;
            if (Input.GetMouseButtonDown(1) && !_isAiming && canAim)
            {
                StartCoroutine(LockTemporarily());
                if (componentHolder.playerAnimationController.IsLocked()) return;
                _isAiming = true;
                locked = true;
                EnableScope();
            }

            if (Input.GetMouseButtonDown(1) && _isAiming && !locked)
            {
                _isAiming = false;
                DisableScope();
            }
        }
    }

    public bool IsAiming()
    {
        return _isAiming;
    }

    private IEnumerator LockTemporarily()
    {
        componentHolder.playerAnimationController.aimingLock = true;
        var animator = componentHolder.playerAnimationController.anim;
        var clip = PlayerUtils.GetClipByStateName(
            animator,  new AnimatorOverrideController(animator.runtimeAnimatorController), "AN_FPS_ToAds");
        yield return new WaitForSeconds(clip.length + 0.05f);
        componentHolder.playerAnimationController.aimingLock = false;
    }

    private void ScopeZoom(bool zoomed)
    {
        if (_zoomCoroutine != null)
            StopCoroutine(_zoomCoroutine);
        _zoomCoroutine = StartCoroutine(AnimateZoom(zoomed));
    }

    private IEnumerator AnimateZoom(bool zoomed)
    {
        var startFOV = _fpsCamera.fieldOfView;
        var endFOV = zoomed ? Mathf.Clamp(defaultFOV / scopeMultiplier, 0, defaultFOV) : defaultFOV;
        var elapsedTime = 0f;

        while (elapsedTime < aimTime)
        {
            _fpsCamera.fieldOfView = Mathf.Lerp(startFOV, endFOV, elapsedTime / aimTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _fpsCamera.fieldOfView = endFOV;
    }

    public void EnableScope()
    {
        componentHolder.playerAnimationController.PlayAimDownSightAnimation();
        ScopeZoom(true);
    }

    public void DisableScope()
    {
        componentHolder.playerAnimationController.PlayStopAimDownSightAnimation();
        ScopeZoom(false);
    }
}