using System.Collections;
using UnityEngine;


public class Aiming : MonoBehaviour
{
    private PlayerAnimationController _pac;

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
        _pac = GetComponent<PlayerAnimationController>();
        _fpsCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        ActionsManager.Instance.Aiming = this;
    }

    public void Run(AimMode aimMode)
    {
        var canAim = true;
        foreach (var s in _pac.weaponActionsStates)
        {
            if (_pac.InProgress(s, 0))
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
                if (_pac.IsLocked()) return;
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
                if (_pac.IsLocked()) return;
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
        _pac.aimingLock = true;

        var animator = _pac.anim;
        var animTime = 0f;

        if (animator != null)
        {
            var overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            AnimationClip clip = null;
            
            foreach (var binding in overrideController.animationClips)
            {
                if (binding.name == "AN_FPS_ToAds")
                {
                    clip = binding;
                    break;
                }
            }

            if (clip != null)
            {
                animTime = clip.length + 0.05f;
            }
            else
            {
                Debug.LogError("Animation clip AN_FPS_ToAds not found.");
            }
        }
        else
        {
            Debug.LogError("Animator is null.");
        }

        yield return new WaitForSeconds(animTime);
        _pac.aimingLock = false;
    }

    private void ScopeZoom(bool zoomed)
    {
        if (_zoomCoroutine != null)
        {
            StopCoroutine(_zoomCoroutine);
        }

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
        _pac.PlayAimDownSightAnimation();
        ScopeZoom(true);
    }

    public void DisableScope()
    {
        _pac.PlayStopAimDownSightAnimation();
        ScopeZoom(false);
    }
}