using System.Collections;
using System.Collections.Generic;
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
}
