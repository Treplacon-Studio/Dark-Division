using UnityEngine;


public class Shooting : MonoBehaviour
{
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;

    private Transform _bulletStartPoint;
    private float _nextFireTime;

    private void Awake()
    {
        ActionsManager.Instance.Shooting = this;
        if (ActionsManager.Instance?.Switching is not null && ActionsManager.Instance.Switching.WeaponComponent() is not null)
            _bulletStartPoint = ActionsManager.Instance.Switching.WeaponComponent().GetStartPoint().transform;
    }

    private void AutomaticFire()
    {
        if (ActionsManager.Instance.Switching.WeaponComponent() is null)
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
        
        var currentWeaponID = ActionsManager.Instance.Switching.GetCurrentWeaponID();
        
        //No ammo
        if (componentHolder.bulletPoolingManager.GetAmmoPrimary() <= 0)
            return;

        var wc = ActionsManager.Instance.Switching.WeaponComponent();
        _nextFireTime = Time.time + wc.Info().Stats().FireRate;
        _bulletStartPoint ??= ActionsManager.Instance.Switching.WeaponComponent().GetStartPoint().transform;
        componentHolder.bulletPoolingManager.SpawnFromPool(currentWeaponID, _bulletStartPoint.transform.position, _bulletStartPoint.transform.rotation);
        
        componentHolder.playerAnimationController.PlayShootAnimation(ActionsManager.Instance.Aiming.IsAiming());
    }

    public void Run()
    {
        if (ActionsManager.Instance?.Switching is not null && ActionsManager.Instance.Switching.WeaponComponent() is not null)
            _bulletStartPoint ??= ActionsManager.Instance.Switching.WeaponComponent().GetStartPoint().transform;
        AutomaticFire();
    }
}