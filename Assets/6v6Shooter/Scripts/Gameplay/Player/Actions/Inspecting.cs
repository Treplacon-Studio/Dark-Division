using System.Collections;
using UnityEngine;

/// <summary>
/// Class handles weapon inspecting.
/// </summary>
public class Inspecting : MonoBehaviour
{
    #region Base Parameters
    
    [Header("Basic action setup.")]
    
    [SerializeField] [Tooltip("Player network controller component.")]
    private PlayerNetworkController pnc;
    
    #endregion Base Parameters

    #region Specific Parameters
    
    [Header("Specific action setup.")]
    
    [SerializeField] [Tooltip("Additional inspect lock time over animation time.")]
    private float fExtraInspectLockTime;
    
    private bool _locker;
    
    #endregion Specific Parameters
    
    #region Base Methods
    
    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Inspecting = this;
    }

    /// <summary>
    /// Called every frame method for action handle.
    /// </summary>
    public void Run()
    {
        var componentHolder = ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder;
        if (Input.GetKeyDown(KeyCode.I) && !componentHolder.playerAnimationController.IsLocked())
        {
            StartCoroutine(LockTemporarily());

            var fCanInspect = true;
            foreach (var s in componentHolder.playerAnimationController.weaponActionsStates)
            {
                if (componentHolder.playerAnimationController.InProgress(s, 0))
                {
                    fCanInspect = false;
                    break;
                }
            }

            if (fCanInspect)
            {
                _locker = true;
                componentHolder.playerAnimationController.PlayInspectAnimation();
            }
        }
    }
    
    #endregion Base Methods
    
    #region Specific Methods
    
    /// <summary>
    /// Set an inspecting lock for inspect animation time and a little bit longer.
    /// </summary>
    private IEnumerator LockTemporarily()
    {
        while (!_locker)
            yield return null;
        
        var componentHolder = ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder;
        componentHolder.playerAnimationController.inspectingLock = true;
        
        var currentWeapon = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent();
        var clip = GetComponent<AnimationClipsHolder>()
            .baseWeaponAnimations[(int)currentWeapon.Info().Name()].inspect;

        if(clip is null)
            Debug.LogError("Inspecting clip has not been attached.");
        
        yield return new WaitForSeconds(clip!.length + fExtraInspectLockTime);
        
        componentHolder.playerAnimationController.inspectingLock = false;
        _locker = false;
    }
    
    #endregion Specific Methods
}