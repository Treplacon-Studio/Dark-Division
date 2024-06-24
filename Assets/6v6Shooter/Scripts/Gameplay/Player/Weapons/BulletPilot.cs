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

    [SerializeField] [Tooltip("Player that owns the bullet.")]
    private GameObject bulletOwner;

    private Camera _playerCamera;
    private Rigidbody _rb;
    private HashSet<GameObject> _alreadyHitObjects = new();

    private void Awake()
    {
        var cameras = FindObjectsOfType<Camera>();
        foreach (var cam in cameras)
        {
            if (!cam.gameObject.name.Contains("Main Camera")) continue;
            _playerCamera = cam;
            break;
        }
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Invoke(nameof(Deactivate), fadeDuration);
        _rb.velocity = GetShootDirection() * initialSpeed;
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
        if (Physics.Raycast(transform.position, GetShootDirection(), out var hit, currentRayLength, hitLayers))
        {
            var hitObject = hit.collider.gameObject;
            if (!_alreadyHitObjects.Contains(hitObject))
            {
                hitObject.GetComponent<HealthController>().TakeDamage(10f);
                Debug.Log($"{bulletOwner.name} hits {hitObject.name}!");
                _alreadyHitObjects.Add(hitObject);
                Invoke(nameof(Deactivate), fadeDuration);
            }
        }
    }

    private Vector3 GetShootDirection()
    {
        var screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        var ray = _playerCamera.ScreenPointToRay(screenCenter);
        return Physics.Raycast(ray, out var hit)
            ? (hit.point - transform.position).normalized
            : ray.direction.normalized;
    }

    public void ResetHits()
    {
        _alreadyHitObjects = new HashSet<GameObject>();
    }

    public void SetOwner(GameObject owner)
    {
        bulletOwner = owner;
    }
}