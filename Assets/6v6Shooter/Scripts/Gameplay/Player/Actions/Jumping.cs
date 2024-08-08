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

    [SerializeField] [Tooltip("Time that walking to hands idle in jump transition takes.")]
    private float fWalkIdleTransitionTime;
    
    private bool _bCanJump = true;
    private bool _bLock;
    
    private float _fTransitionStartTime, _fTransitionMultiplier;
    private bool _bWasGrounded;
    
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
        
        if(CanInvokeJump())
            componentHolder.playerAnimationController.PlayJumpAnimation(bLanding, bGrounded);
        else
            componentHolder.playerAnimationController.PlayJumpAnimation(true, false);

        SetJumpTransitionParameter();
        _bWasGrounded = bGrounded;
    }
    
    #endregion Base Methods
    
    #region Specific Methods
    
    /// <summary>
    /// Controls crouch (transitional) state between walking/idling and crouch walking/crouch idling.
    /// </summary>
    private void SetJumpTransitionParameter()
    {
        if (mc.IsGrounded() && _bWasGrounded)
        {
            _fTransitionMultiplier += Time.deltaTime / fWalkIdleTransitionTime;
            _fTransitionMultiplier = Mathf.Clamp01(_fTransitionMultiplier);
        }
        else if (!mc.IsGrounded() && !mc.IsLanding())
        {
            _fTransitionMultiplier -= Time.deltaTime / fWalkIdleTransitionTime;
            _fTransitionMultiplier = Mathf.Clamp01(_fTransitionMultiplier);
        }
    }
    
    /// <summary>
    /// Checks if player can invoke jump.
    /// </summary>
    /// <returns>Information if player can invoke jump.</returns>
    public bool CanInvokeJump()
    {
        var componentHolder = ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder;
        return !(componentHolder.playerAnimationController.inspectingLock ||
                 componentHolder.playerAnimationController.reloadingLock || 
                 ActionsManager.GetInstance(pnc.GetInstanceID()).Aiming.IsAiming());
    }

    /// <summary>
    /// If jumping, smoothly transits input from moving to idle and backwards.
    /// </summary>
    /// <param name="input">Entry input vector to be updated.</param>
    /// <returns>
    /// Updated input vector taking in mind jumping feature.
    /// </returns>
    public Vector2 TransitionToIdleWhenJumping(Vector2 input)
    {
        return input * _fTransitionMultiplier;
    }
    
    #endregion Specific Methods
    
    #region Accessors
    
    /// <summary>
    /// Getter method.
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