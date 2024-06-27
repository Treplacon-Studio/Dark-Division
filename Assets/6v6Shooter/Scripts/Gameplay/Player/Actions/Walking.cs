using UnityEngine;


public class Walking : MonoBehaviour
{
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;
    
    private void Awake()
    {
        ActionsManager.Instance.Walking = this;
    }

    public void Run(Vector2 input, bool grounded)
    {
        componentHolder.playerAnimationController.PlayWalkingAnimation(input, grounded);
    }
}