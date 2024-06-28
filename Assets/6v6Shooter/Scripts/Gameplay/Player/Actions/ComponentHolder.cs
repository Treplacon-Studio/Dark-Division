using UnityEngine;

[DefaultExecutionOrder(-32000)]
public class ComponentHolder : MonoBehaviour
{
    public PlayerAnimationController playerAnimationController;
    public BulletPoolingManager bulletPoolingManager;
    public WeaponSpecificAnimations weaponSpecificAnimations;

    private void Awake()
    {
        ActionsManager.Instance.ComponentHolder = this;
    }
}
