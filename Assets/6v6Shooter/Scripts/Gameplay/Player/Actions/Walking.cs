using UnityEngine;

/// <summary>
/// Class handles idling, walking and running features.
/// </summary>
public class Walking : MonoBehaviour
{
    #region Base Parameters
    
    [Header("Basic action setup.")]
    
    [SerializeField] [Tooltip("Player network controller component.")]
    private PlayerNetworkController pnc;
    
    #endregion Base Parameters
    
    #region Specific Parameters

    [Header("Specific action setup.")] 
    
    [SerializeField] [Tooltip("Movement controller component.")]
    private MovementController mc;
    
    #endregion Specific Parameters
    
    
    #region Base Methods
    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Walking = this;
    }

    public void Run()
    {
        //Transition between running and idle while starting to slide and backwards.
        var v2Input = ActionsManager.GetInstance(pnc.GetInstanceID()).Crouching.TransitionToIdleWhenSliding(mc.GetInput());
        var bGrounded = mc.IsGrounded();
        
        var componentHolder = ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder;
        componentHolder.playerAnimationController.PlayWalkingAnimation(v2Input, bGrounded);
    }
    
    #endregion Base Methods
}