using System.Linq;
using Unity.VisualScripting;
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
    
    public static T FindComponentInParents<T>(GameObject child) where T : Component
    {
        if (child == null)
            return null;
        var component = child.GetComponent<T>();
        
        if (component != null)
            return component;
        
        var parentTransform = child.transform.parent;
       
        while (parentTransform != null)
        {
            component = parentTransform.GetComponentInParent<T>(true);
            if (component != null)
                return component;

            parentTransform = parentTransform.parent;
        }
        return null;
    }
}
