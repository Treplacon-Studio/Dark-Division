using UnityEngine;

public class AdsTransitionBehaviour : StateMachineBehaviour
{
    [SerializeField] [Tooltip("If script is attached on inverted transition.")]
    private bool bInverted;
    
    private static readonly int PShootingState = Animator.StringToHash("pShootingState");
    private static readonly int PStopShootingTrigger = Animator.StringToHash("pStopShootingTrigger");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var fShootingTransitionState = animator.GetFloat(PShootingState);
        
        //If transition is going from shooting, adjust time from the last shooting transition state
        if (animator.GetBool(PStopShootingTrigger))
            animator.Play(stateInfo.fullPathHash, layerIndex, fShootingTransitionState);
    }
}
