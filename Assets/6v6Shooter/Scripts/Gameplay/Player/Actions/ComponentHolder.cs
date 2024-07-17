using UnityEngine;
using Photon.Pun;

[DefaultExecutionOrder(-32000)]
public class ComponentHolder : MonoBehaviour
{
    [SerializeField] private PlayerNetworkController pnc;
    
    public PlayerAnimationController playerAnimationController;
    public BulletPoolingManager bulletPoolingManager;
    public WeaponSpecificAnimations weaponSpecificAnimations;
    public PhotonView PlayerPhotonView;

    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder = this;
    }
}
