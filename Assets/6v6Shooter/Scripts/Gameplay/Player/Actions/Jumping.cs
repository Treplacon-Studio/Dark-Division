using UnityEngine;

/// <summary>
/// Class handles jumping and vaulting features at once.
/// </summary>
public class Jumping : MonoBehaviour
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
    
    private bool _bCanJump = true;
    private bool _bLock;
    
    #endregion Specific Parameters
    
    #region Base Methods
    
    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Jumping = this;
    }

    /// <summary>
    /// Called every frame method for action handle.
    /// </summary>
    public void Run()
    {
        var bLanding = mc.IsLanding();
        var bGrounded = mc.IsGrounded();
        var componentHolder = ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder;
        componentHolder.playerAnimationController.PlayJumpAnimation(bLanding, bGrounded);
    }
    
    #endregion Base Methods
    
    #region Accessors
    
    /// <summary>
    /// Getter method .
    /// </summary>
    /// <returns>
    /// Information if player can now jump.
    /// </returns>
    public bool CanJump()
    {
        return _bCanJump;
    }

    /// <summary>
    /// Getter method.
    /// </summary>
    /// <returns>
    /// Information if player just triggered the jump.
    /// </returns>
    public bool JumpTriggered()
    {
        return Input.GetButton("Jump");
    }
    
    #endregion Accessors
   
}