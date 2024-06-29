using UnityEngine;
using Photon.Pun;

[DefaultExecutionOrder(-32000)]
public class ComponentHolder : MonoBehaviour
{
    public PlayerAnimationController playerAnimationController;
    public BulletPoolingManager bulletPoolingManager;
    public WeaponSpecificAnimations weaponSpecificAnimations;
    public PhotonView PlayerPhotonView;

    private void Awake()
    {
        ActionsManager.Instance.ComponentHolder = this;
    }
}
