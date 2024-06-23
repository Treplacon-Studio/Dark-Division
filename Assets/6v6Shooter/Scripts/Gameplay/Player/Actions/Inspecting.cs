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
        var clip = PlayerUtils.GetClipByStateName(
            animator,  new AnimatorOverrideController(animator.runtimeAnimatorController), "AN_FPS_Inspect");

        yield return new WaitForSeconds(clip.length + 0.05f);
        _pac.inspectingLock = false;
    }
}