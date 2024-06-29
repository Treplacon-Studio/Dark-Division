using System.Collections;
using UnityEngine;


public class Inspecting : MonoBehaviour
{
    [SerializeField] private PlayerNetworkController pnc;
    
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;
    
    [SerializeField] [Tooltip("Clips for specific weapon animations.")]
    private WeaponAnimation[] clips;

    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Inspecting = this;
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
        
        AnimationClip clip = null;
        var currentWeapon = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent();
        foreach(var elem in clips)
            if (currentWeapon != null && elem.name == currentWeapon.Info().Name())
                clip = elem.clip;

        yield return new WaitForSeconds(clip.length + 0.05f);
        componentHolder.playerAnimationController.inspectingLock = false;
    }
}