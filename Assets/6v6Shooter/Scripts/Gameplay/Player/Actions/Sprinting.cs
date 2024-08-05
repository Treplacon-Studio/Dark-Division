using UnityEngine;

/// <summary>
/// Class handles sprinting feature.
/// </summary>
public class Sprinting : MonoBehaviour
{
    #region Base Parameters
    
    [SerializeField] 
    private PlayerNetworkController pnc;

    #endregion Base Parameters
    
    #region Specific Parameters
    
    private bool _bSprinting;
    
    #endregion Specific Parameters
    
    #region Base Methods
    
    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Sprinting = this;
    }

    /// <summary>
    /// Called every frame method for action handle.
    /// </summary>
    public void Run()
    {
        //Player cannot sprint when aiming
        _bSprinting = !ActionsManager.GetInstance(pnc.GetInstanceID()).Aiming.IsAiming() &&
                      Input.GetKey(KeyCode.LeftShift);
        
        var componentHolder = ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder;
        componentHolder.playerAnimationController.PlaySprintAnimation(_bSprinting);
        
        /*
         Idle on slide is implemented inside walking script because this script corresponds
         to vertical and horizontal input.
         */
    }
    
    #endregion Base Methods
    
    #region Accessors

    /// <summary>
    /// Returns information if player is currently sprinting.
    /// </summary>
    public bool IsSprinting()
    {
        return _bSprinting;
    }
    
    #endregion Accessors
}