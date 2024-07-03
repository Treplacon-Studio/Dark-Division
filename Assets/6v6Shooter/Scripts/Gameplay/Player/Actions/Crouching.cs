using UnityEngine;


public class Crouching : MonoBehaviour
{
    [SerializeField] private PlayerNetworkController pnc;
    
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;

    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Crouching = this;
    }

    public void Run(bool isCrouching)
    {
        return;
    }
}