using System.Collections.Generic;
using _6v6Shooter.Scripts.Gameplay;
using Photon.Pun;
using UnityEngine;


public class BulletPilot : MonoBehaviour
{
    [SerializeField] [Tooltip("Initial speed of the bullet.")]
    private float initialSpeed = 100f;

    [SerializeField] [Tooltip("Factor for lenght of the ray.")]
    private float rayLengthFactor = 1f;

    [SerializeField] [Tooltip("Layers that will be affected by bullet.")]
    private LayerMask hitLayers;

    [SerializeField] [Tooltip("Time after the bullet disappears.")]
    private float fadeDuration = 0.3f;
    
    private GameObject _bulletOwner;
    private Camera _playerCamera;
    private PhotonView _photonView;
    private PhotonView _bulletOwnerPhotonView;
    private Rigidbody _rb;
    private HashSet<GameObject> _alreadyHitObjects = new();
    private Recoil _recoil;
    private Vector3 _currentDirection;

    private void Awake()
    {
        _bulletOwner = ActionsManager.Instance.ComponentHolder.bulletPoolingManager.player;
        _photonView = GetComponent<PhotonView>();
        var cameras = FindObjectsOfType<Camera>();
        foreach (var cam in cameras)
        {
            if (!cam.gameObject.name.Contains("Main Camera")) continue;
            _playerCamera = cam;
            break;
        }
        _rb = GetComponent<Rigidbody>();
    }

    public void SetRecoil(Recoil recoil)
    {
        _recoil = recoil;
    }
    
    private void OnEnable()
    {
        Invoke(nameof(Deactivate), fadeDuration);
        _currentDirection = GetShootDirection();
        _rb.velocity = _currentDirection * initialSpeed;
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Deactivate()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    void Update()
    {
        ShootRay();
    }

    void ShootRay()
    {
        if (_playerCamera == null)
        {
            Debug.LogError("Camera not found.");
            return;
        }
        
        var currentRayLength = initialSpeed * rayLengthFactor;
        if (Physics.Raycast(transform.position, _currentDirection, out var hit, currentRayLength, hitLayers))
        {
            var hitObject = hit.collider.gameObject;

            //Get the PhotonView of the hit object
            PhotonView hitPhotonView = hitObject.GetComponent<PhotonView>();

            //Player cannot hit himself
            if (hitPhotonView != null && hitPhotonView.Owner == _bulletOwnerPhotonView.Owner)
                return;

            if (!_alreadyHitObjects.Contains(hitObject))
            {
                var healthController = hitObject.GetComponent<HealthController>();
                if (healthController != null)
                {
                    healthController.TakeDamage(10f);
                    Debug.Log($"{_bulletOwner.name} hits {hitObject.name}!");
                    _alreadyHitObjects.Add(hitObject);
                    Invoke(nameof(Deactivate), fadeDuration);
                }
                else
                    Debug.LogError("HealthController component not found on hit object.");
            }
        }
    }
    
    private Vector3 GetShootDirection()
    {
        var screenCenterPoint = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        var ray = _playerCamera.ScreenPointToRay(screenCenterPoint);
        var baseDir = ray.direction;
        
        if (_recoil == null)
            return baseDir;
        
        //Direction changes when recoiling.
        var finalDir = baseDir + _playerCamera.transform.rotation *  _recoil.GetRecoilOffset();
        finalDir.Normalize();
        return finalDir;
    }

    public void ResetHits()
    {
        _alreadyHitObjects = new HashSet<GameObject>();
    }

    public void SetOwner(GameObject owner)
    {
        _bulletOwner = owner;
    }

    public void SetOwnerPhotonView(PhotonView pv)
    {
        _bulletOwnerPhotonView = pv;
    }
}