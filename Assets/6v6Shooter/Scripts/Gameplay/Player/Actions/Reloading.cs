using System.Collections;
using UnityEngine;


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

        var animator = _pac.anim;
        var animTime = 0f;

        if (animator != null)
        {
            var overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            AnimationClip clip = null;
            
            foreach (var binding in overrideController.animationClips)
            {
                if (binding.name == "AN_FPS_Reload")
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
                Debug.LogError("Animation clip AN_FPS_Reload not found.");
            }
        }
        else
        {
            Debug.LogError("Animator is null.");
        }

        yield return new WaitForSeconds(animTime);
        _pac.reloadingLock = false;
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