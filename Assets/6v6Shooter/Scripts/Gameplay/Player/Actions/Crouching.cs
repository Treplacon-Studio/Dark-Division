using UnityEngine;


public class Crouching : MonoBehaviour
{
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;

    private void Awake()
    {
        ActionsManager.Instance.Crouching = this;
    }

    public void Run(bool isCrouching)
    {
        return;
    }
}