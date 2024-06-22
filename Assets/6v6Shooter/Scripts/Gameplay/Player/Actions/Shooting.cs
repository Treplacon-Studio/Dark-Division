using UnityEngine;


public class Shooting : MonoBehaviour
{
    [SerializeField] [Tooltip("Bullet pool manager component to shot bullets.")]
    private BulletPoolingManager bpm;

    private Transform _bulletStartPoint;
    private PlayerAnimationController _pac;
    private float _nextFireTime;

    private void Awake()
    {
        _pac = GetComponent<PlayerAnimationController>();
        ActionsManager.Instance.Shooting = this;
        if (ActionsManager.Instance?.Switching is not null)
            _bulletStartPoint = ActionsManager.Instance.Switching.WeaponComponent().GetStartPoint().transform;
    }

    private void AutomaticFire()
    {
        if (_pac.reloadingLock)
        {
            _pac.shootingLock = false;
            return;
        }

        var shootKeyClicked = Input.GetMouseButton(0);
        _pac.shootingLock = shootKeyClicked && Time.time >= _nextFireTime;

        if (!shootKeyClicked || !(Time.time >= _nextFireTime))
            return;

        var wc = ActionsManager.Instance.Switching.WeaponComponent();
        _nextFireTime = Time.time + wc.Info().Stats().FireRate;
        _bulletStartPoint ??= ActionsManager.Instance.Switching.WeaponComponent().GetStartPoint().transform;
        bpm.SpawnFromPool(wc.GetMag().GetComponent<Mag>().ammoType,
            _bulletStartPoint.transform.position,
            _bulletStartPoint.transform.rotation);
        _pac.PlayShootAnimation(ActionsManager.Instance.Aiming.IsAiming());
    }

    public void Run()
    {
        if (ActionsManager.Instance?.Switching is not null)
            _bulletStartPoint ??= ActionsManager.Instance.Switching.WeaponComponent().GetStartPoint().transform;
        AutomaticFire();
    }
}