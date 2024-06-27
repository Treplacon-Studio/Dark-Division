using System;
using System.Collections;
using System.Collections.Generic;
using _6v6Shooter.Scripts.Gameplay;
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
    private Rigidbody _rb;
    private HashSet<GameObject> _alreadyHitObjects = new();
    private Recoil _recoil;
    private Vector3 _currentDirection;

    private void Awake()
    {
        _bulletOwner = ActionsManager.Instance.ComponentHolder.bulletPoolingManager.player;
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
            
            //Player cannot hit himself
            if (hit.collider.gameObject == _bulletOwner)
                return;
            
            if (!_alreadyHitObjects.Contains(hitObject))
            {
                hitObject.GetComponent<HealthController>().TakeDamage(10f);
                Debug.Log($"{_bulletOwner.name} hits {hitObject.name}!");
                _alreadyHitObjects.Add(hitObject);
                Invoke(nameof(Deactivate), fadeDuration);
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
}