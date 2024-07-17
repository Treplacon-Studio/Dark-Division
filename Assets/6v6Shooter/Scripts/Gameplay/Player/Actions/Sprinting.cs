using UnityEngine;


public class Sprinting : MonoBehaviour
{
    [SerializeField] private PlayerNetworkController pnc;
    
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;
    
    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Sprinting = this;
    }

    public void Run()
    {
        componentHolder.playerAnimationController.PlaySprintAnimation(Input.GetKey(KeyCode.LeftShift));
    }
}