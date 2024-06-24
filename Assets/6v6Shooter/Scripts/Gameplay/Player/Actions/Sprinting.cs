using UnityEngine;


public class Sprinting : MonoBehaviour
{
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;
    
    private void Awake()
    {
        ActionsManager.Instance.Sprinting = this;
    }

    public void Run()
    {
        componentHolder.playerAnimationController.PlaySprintAnimation(Input.GetKey(KeyCode.LeftShift));
    }
}