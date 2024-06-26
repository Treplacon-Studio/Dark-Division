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
        var animatorController = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
        if (animatorController != null)
        {
            return animatorController.layers.SelectMany(layer => layer.stateMachine.states)
                .Any(state => state.state.name == stateName && state.state.motion == clip);
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
