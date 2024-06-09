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
    public void PlayReloadAnimation()
    {
        SetIKInfluenceLeftHand(false);
        anim.SetTrigger("reloadWeapon");
    }

    public void SetIsGroundedAnim(bool isGrounded) => anim.SetBool("isGrounded", isGrounded);

    public void PlayWalkingAnimation(Vector2 movementInput)
    {
        anim.SetFloat("Horizontal", movementInput.x);
        anim.SetFloat("Vertical", movementInput.y);
    }

    private void SetIKInfluenceLeftHand(bool ik)
    {
        if (ik)
        {
            anim.SetIKPosition(AvatarIKGoal.LeftHand, Vector3.zero);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.identity);
        }
        else
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        }
    }
}
