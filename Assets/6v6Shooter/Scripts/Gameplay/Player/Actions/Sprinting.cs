using UnityEngine;


public class Sprinting : MonoBehaviour
{
    private PlayerAnimationController _pac;

    private void Awake()
    {
        _pac = GetComponent<PlayerAnimationController>();
        ActionsManager.Instance.Sprinting = this;
    }

    public void Run()
    {
        _pac.PlaySprintAnimation(Input.GetKey(KeyCode.LeftShift));
    }
}