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
        _bulletStartPoint = ActionsManager.Instance.Switching.WeaponComponent().GetStartPoint().transform;
        if (_bulletStartPoint == null)
            Debug.LogError("Bullet start point not found.");
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

        var wi = ActionsManager.Instance.Switching.WeaponComponent().Info().Stats();
        _nextFireTime = Time.time + wi.FireRate;
        _bulletStartPoint ??= ActionsManager.Instance.Switching.WeaponComponent().GetStartPoint().transform;
        bpm.SpawnFromPool(wi.BType,
            _bulletStartPoint.transform.position,
            _bulletStartPoint.transform.rotation);
        _pac.PlayShootAnimation(ActionsManager.Instance.Aiming.IsAiming());
    }

    public void Run()
    {
        AutomaticFire();
    }
}