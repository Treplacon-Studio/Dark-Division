using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitmarkerController : MonoBehaviour
{
    public enum HitResult
    {
        DamageObject,
        DamageCharacter,
        KillObject,
        KillCharacter
    }

    public enum HitType
    {
        Headshot,
        Normal
    }

    public Image headshotHitmarker;
    public Image bodyshotHitmarker;
    public Image wallHitmarker;

    private Animator headshotAnimator;
    private Animator bodyshotAnimator;
    private Animator wallHitAnimator;

    private void Awake()
    {
        headshotAnimator = headshotHitmarker.GetComponent<Animator>();
        bodyshotAnimator = bodyshotHitmarker.GetComponent<Animator>();
        wallHitAnimator = wallHitmarker.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // ShootingScript.OnHit += ShowHitmarker;
    }

    private void OnDisable()
    {
        // ShootingScript.OnHit -= ShowHitmarker;
    }

    private void ShowHitmarker(HitResult hitResult, HitType hitType)
    {
        // Determine the target hitmarker and animator
        Image targetHitmarker = null;
        Animator targetAnimator = null;
        string triggerName = "TriggerDamage";

        // Set the appropriate targetHitmarker and targetAnimator based on hitResult and hitType
        switch (hitResult)
        {
            case HitResult.DamageCharacter:
            case HitResult.KillCharacter:
                targetHitmarker = bodyshotHitmarker;
                targetAnimator = hitType == HitType.Headshot ? headshotAnimator : bodyshotAnimator;
                break;

            case HitResult.DamageObject:
            case HitResult.KillObject:
                targetHitmarker = wallHitmarker;
                targetAnimator = wallHitAnimator;
                break;
        }

        // Apply random rotation and play animation if targets are set
        if (targetHitmarker != null && targetAnimator != null)
        {
            SetRandomRotation(targetHitmarker.transform);
            PlayHitmarkerAnimation(targetAnimator, triggerName);
        }
    }

    private void SetRandomRotation(Transform hitmarkerTransform)
    {
        float randomZRotation = Random.Range(-10f, 10f);
        hitmarkerTransform.rotation = Quaternion.Euler(0f, 0f, randomZRotation);
    }

    private void PlayHitmarkerAnimation(Animator animator, string shotType)
    {
        animator.SetTrigger(shotType);
    }
}
