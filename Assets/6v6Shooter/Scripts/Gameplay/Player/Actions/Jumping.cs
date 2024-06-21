using UnityEngine;

public class Jumping : MonoBehaviour
{
    private PlayerAnimationController _pac;
    private bool _canJump = true;
    private bool _lock;

    private void Awake()
    {
        _pac = GetComponent<PlayerAnimationController>();
        ActionsManager.Instance.Jumping = this;
    }

    public void Run(bool isLanding, bool isGrounded)
    {
        _pac.PlayJumpAnimation(isLanding, isGrounded);
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