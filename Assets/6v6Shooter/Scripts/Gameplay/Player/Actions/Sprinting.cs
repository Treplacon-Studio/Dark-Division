using UnityEngine;


public class Sprinting : MonoBehaviour
{
    [SerializeField] private PlayerNetworkController pnc;
    
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;

    private bool _bSprinting;
    
    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Sprinting = this;
    }

    /// <summary>
    /// Treat <c>Run</c> method as update. It runs in movement update.
    /// </summary>
    public void Run()
    {
        //Player cannot sprint when aiming
        _bSprinting = !ActionsManager.GetInstance(pnc.GetInstanceID()).Aiming.IsAiming() &&
                      Input.GetKey(KeyCode.LeftShift);
        
        componentHolder.playerAnimationController.PlaySprintAnimation(_bSprinting);
        
        /*
         Idle on slide is implemented inside walking script because this script corresponds
         to vertical and horizontal input.
         */
    }

    /// <summary>
    /// Returns information if player is currently sprinting.
    /// </summary>
    public bool IsSprinting()
    {
        return _bSprinting;
    }
}