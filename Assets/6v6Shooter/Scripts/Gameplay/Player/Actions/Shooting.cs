using UnityEngine;


public class Shooting : MonoBehaviour
{
    [SerializeField] private PlayerNetworkController pnc;
    
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;

    private Transform _bulletStartPoint;
    private float _nextFireTime;

    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Shooting = this;
        if (ActionsManager.GetInstance(pnc.GetInstanceID())?.Switching is not null && ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent() is not null)
            _bulletStartPoint = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent().GetStartPoint().transform;
    }

    private void AutomaticFire()
    {
        if (ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent() is null)
            return;
        
        if (componentHolder.playerAnimationController.reloadingLock)
        {
            componentHolder.playerAnimationController.shootingLock = false;
            return;
        }

        var shootKeyClicked = Input.GetMouseButton(0);
        componentHolder.playerAnimationController.shootingLock = shootKeyClicked && Time.time >= _nextFireTime;
        componentHolder.playerAnimationController.StopShooting(!shootKeyClicked && Time.time >= _nextFireTime);

        if (!shootKeyClicked || !(Time.time >= _nextFireTime))
            return;
        
        var currentWeaponID = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.GetCurrentWeaponID();
        
        //No ammo
        if (componentHolder.bulletPoolingManager.GetAmmoPrimary() <= 0)
            return;

        var wc = ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent();
        wc.gameObject.GetComponent<Recoil>().StartRecoil(0.03f);
        _nextFireTime = Time.time + wc.Info().Stats().FireRate;
        _bulletStartPoint ??= ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent().GetStartPoint().transform;
       
        componentHolder.bulletPoolingManager.SpawnFromPool(currentWeaponID, _bulletStartPoint.transform);
        componentHolder.playerAnimationController.PlayShootAnimation(ActionsManager.GetInstance(pnc.GetInstanceID()).Aiming.IsAiming());
    }

    public void Run()
    {
        if (ActionsManager.GetInstance(pnc.GetInstanceID())?.Switching is not null && ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent() is not null)
            _bulletStartPoint ??= ActionsManager.GetInstance(pnc.GetInstanceID()).Switching.WeaponComponent().GetStartPoint().transform;
        AutomaticFire();
    }
}