using UnityEngine;


public class Walking : MonoBehaviour
{
    [SerializeField] private PlayerNetworkController pnc;
    
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;
    
    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Walking = this;
    }

    public void Run(Vector2 input, bool grounded)
    {
        //Transition between running and idle while starting to slide and backwards.
        input = ActionsManager.GetInstance(pnc.GetInstanceID()).Crouching.TransitionToIdleWhenSliding(input);
        
        componentHolder.playerAnimationController.PlayWalkingAnimation(input, grounded);
    }
}