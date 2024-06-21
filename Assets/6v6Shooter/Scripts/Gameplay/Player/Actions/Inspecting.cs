using System.Collections;
using UnityEngine;


public class Inspecting : MonoBehaviour
{
    private PlayerAnimationController _pac;

    private void Awake()
    {
        _pac = GetComponent<PlayerAnimationController>();
        ActionsManager.Instance.Inspecting = this;
    }

    public void Run()
    {
        if (Input.GetKeyDown(KeyCode.I) && !_pac.IsLocked())
        {
            StartCoroutine(LockTemporarily());

            var canInspect = true;
            foreach (var s in _pac.weaponActionsStates)
            {
                if (_pac.InProgress(s, 0))
                {
                    canInspect = false;
                    break;
                }
            }

            if (canInspect)
                _pac.PlayInspectAnimation();
        }
    }


    private IEnumerator LockTemporarily()
    {
        _pac.inspectingLock = true;

        var animator = _pac.anim;
        var animTime = 0f;

        if (animator != null)
        {
            var overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            AnimationClip clip = null;
            
            foreach (var binding in overrideController.animationClips)
            {
                if (binding.name == "AN_FPS_Inspect")
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
                Debug.LogError("Animation clip AN_FPS_Inspect not found.");
            }
        }
        else
        {
            Debug.LogError("Animator is null.");
        }

        yield return new WaitForSeconds(animTime);
        _pac.inspectingLock = false;
    }
}