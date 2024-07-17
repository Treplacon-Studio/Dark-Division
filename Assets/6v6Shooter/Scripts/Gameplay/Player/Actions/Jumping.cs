using UnityEngine;

public class Jumping : MonoBehaviour
{
    [SerializeField] private PlayerNetworkController pnc;
    
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;
    
    private bool _canJump = true;
    private bool _lock;

    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Jumping = this;
    }

    public void Run(bool isLanding, bool isGrounded)
    {
        componentHolder.playerAnimationController.PlayJumpAnimation(isLanding, isGrounded);
    }

    public bool CanJump()
    {
        return _canJump;
    }

    public bool JumpTriggered()
    {
        return Input.GetButton("Jump");
    }
}