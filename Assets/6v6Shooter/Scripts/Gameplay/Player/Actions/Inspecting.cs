using System.Collections;
using UnityEngine;


public class Inspecting : MonoBehaviour
{
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;

    private void Awake()
    {
        ActionsManager.Instance.Inspecting = this;
    }

    public void Run()
    {
        if (Input.GetKeyDown(KeyCode.I) && !componentHolder.playerAnimationController.IsLocked())
        {
            StartCoroutine(LockTemporarily());

            var canInspect = true;
            foreach (var s in componentHolder.playerAnimationController.weaponActionsStates)
            {
                if (componentHolder.playerAnimationController.InProgress(s, 0))
                {
                    canInspect = false;
                    break;
                }
            }

            if (canInspect)
                componentHolder.playerAnimationController.PlayInspectAnimation();
        }
    }


    private IEnumerator LockTemporarily()
    {
        componentHolder.playerAnimationController.inspectingLock = true;
        
        var animator = componentHolder.playerAnimationController.anim;
        var clip = PlayerUtils.GetClipByStateName(
            animator,  new AnimatorOverrideController(animator.runtimeAnimatorController), "AN_FPS_Inspect");

        yield return new WaitForSeconds(clip.length + 0.05f);
        componentHolder.playerAnimationController.inspectingLock = false;
    }
}