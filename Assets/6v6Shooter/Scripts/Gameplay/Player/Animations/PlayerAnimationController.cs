using UnityEngine;

[RequireComponent(typeof(ComponentHolder))]
[RequireComponent(typeof(Jumping))]
[RequireComponent(typeof(Walking))]
[RequireComponent(typeof(Sprinting))]
[RequireComponent(typeof(Aiming))]
[RequireComponent(typeof(Reloading))]
[RequireComponent(typeof(Inspecting))]
[RequireComponent(typeof(Crouching))]
[RequireComponent(typeof(Shooting))]
[RequireComponent(typeof(Switching))]
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] [Tooltip("List of all weapon animation states names that cannot .")]
    public string[] weaponActionsStates =
    {
        "AN_FPS_Scar_Reload", //Reloading
        "AN_FPS_Scar_Inspect", //Inspecting
        "AN_FPS_Scar_ToAds" //Aiming
    };

    [SerializeField] [Tooltip("Character animator.")]
    public Animator anim;

    public bool reloadingLock;
    public bool inspectingLock;
    public bool aimingLock;
    public bool shootingLock;

    public bool InProgress(string stateName, int layerIndex)
    {
        var currentState = anim.GetCurrentAnimatorStateInfo(layerIndex);
        return currentState.IsName(stateName);
    }

    public bool IsLocked()
    {
        return reloadingLock || inspectingLock;
    }

    //Aiming
    public void PlayAimDownSightAnimation()
    {
        anim.SetTrigger("pAimSight");
        anim.ResetTrigger("pUnaimSight");
    }

    public void PlayStopAimDownSightAnimation()
    {
        anim.SetTrigger("pUnaimSight");
        anim.ResetTrigger("pAimSight");
    }

    public void SetAimState(bool aiming)
    {
        anim.SetBool("pAiming", aiming);
    }

    //Shooting
    public void PlayShootAnimation(bool isAiming)
    {
        anim.SetTrigger(isAiming ? "pShootingADS" : "pShooting");
    }

    public void StopShooting(bool stop)
    {
        anim.SetBool("pStopShooting", stop);
    }

    public void TriggerStopShooting(bool set)
    {
        if(set)
            anim.SetTrigger("pStopShootingTrigger");
        else
            anim.ResetTrigger("pStopShootingTrigger");
    }

    /// <summary>
    /// Sets shooting transition to animator as blending of HFR and ADS.
    /// </summary>
    public void SetShootingTransitionState(float ts)
    {
        anim.SetFloat("pShootingState", ts);
    }

    //Crouching
    public void SetCrouchingState(float state)
    {
        anim.SetFloat("pCrouchState", state);
    }

    //Sprinting
    public void PlaySprintAnimation(bool isSprinting)
    {
        anim.SetBool("pSprinting", isSprinting);
    }
    
    //Sliding
    public void PlaySlideAnimation(bool slide)
    {
        anim.SetBool("pSliding", slide);
    }

    //Jumping
    public bool PlayJumpAnimation(bool landing, bool grounded)
    {
        if (grounded && !landing)
        {
            anim.SetBool("pJumping", false);
            anim.SetBool("pLanding", false);
        }
        else if (!grounded && !landing)
        {
            anim.SetBool("pJumping", true);
            anim.SetBool("pLanding", false);
        }
        else if (!grounded)
        {
            anim.SetBool("pJumping", false);
            anim.SetBool("pLanding", true);
        }

        return !(anim.GetBool("pJumping") || anim.GetBool("pLanding"));
    }

    //Inspecting
    public void PlayInspectAnimation()
    {
        anim.SetTrigger("pInspectingWeapon");
    }

    //Reloading
    public void PlayReloadAnimation()
    {
        anim.SetTrigger("pReloadingWeapon");
    }

    //Walking
    public void PlayWalkingAnimation(Vector2 movementInput, bool isGrounded)
    {
        anim.SetFloat("pHorizontal", movementInput.x);
        anim.SetFloat("pVertical", movementInput.y);
        anim.SetBool("pGrounded", isGrounded);
    }
}