using System.Linq;
using UnityEngine;

public static class PlayerUtils
{
    public static AnimationClip GetClipByStateName(Animator animator, AnimatorOverrideController overrideController, string stateName)
    {
        var clips = overrideController.animationClips;
        return clips.FirstOrDefault(clip => IsClipUsedInState(animator, clip, stateName));
    }

    private static bool IsClipUsedInState(Animator animator, AnimationClip clip, string stateName)
    {
        if (animator == null || clip == null || string.IsNullOrEmpty(stateName))
            return false;

        var animatorController = animator.runtimeAnimatorController;
        if (animatorController is AnimatorOverrideController overrideController)
        {
            animatorController = overrideController.runtimeAnimatorController;
        }

        foreach (var layer in animatorController.animationClips)
        {
            if (layer.name == stateName && layer == clip)
            {
                return true;
            }
        }

        return false;
    }
    
    public static T FindComponentInDescendants<T>(this GameObject parentObject) where T : Component
    {
        if (parentObject == null)
            return null;
        
        var component = parentObject.GetComponent<T>();
        if (component != null)
            return component;
        
        foreach (Transform child in parentObject.transform)
        {
            component = FindComponentInDescendants<T>(child.gameObject);
            if (component != null)
                return component;
        }
        return null;
    }
}
