using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator anim;

    public void PlayAimDownSightAnimation() => anim.SetTrigger("aimSight");
    public void PlayStopAimDownSightAnimation() => anim.SetTrigger("unaimSight");
    public void PlayShootAnimation() => Debug.Log("Shooting.."); //Replace with shooting animation
    public void PlaySprintAnimation(bool isSprinting) => anim.SetBool("isSprinting", isSprinting);
    public void PlayJumpAnimation() => anim.SetTrigger("jump");
    public void PlayInspectAnimation() => anim.SetTrigger("inspectWeapon");
    public void PlayReloadAnimation() => anim.SetTrigger("reloadWeapon");
    public void SetIsGroundedAnim(bool isGrounded) => anim.SetBool("isGrounded", isGrounded);

    public void PlayWalkingAnimation(Vector2 movementInput)
    {
        anim.SetFloat("Horizontal", movementInput.x);
        anim.SetFloat("Vertical", movementInput.y);
    }
}
