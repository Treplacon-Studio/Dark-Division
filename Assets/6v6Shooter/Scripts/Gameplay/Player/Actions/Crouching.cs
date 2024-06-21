using UnityEngine;


public class Crouching : MonoBehaviour
{
    private PlayerAnimationController _pac;

    private void Awake()
    {
        _pac = GetComponent<PlayerAnimationController>();
        ActionsManager.Instance.Crouching = this;
    }

    public void Run(bool isCrouching)
    {
        return;
    }
}