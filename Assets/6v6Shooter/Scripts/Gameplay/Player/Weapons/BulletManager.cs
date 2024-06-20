using System.Collections;
using _6v6Shooter.Scripts.Gameplay;
using UnityEngine;


public class BulletManager : MonoBehaviour
{
    [SerializeField] [Tooltip("Enables bullet trails (debug only).")]
    private bool enableDebugTrails;

    [SerializeField] [Tooltip("Initial speed of the bullet.")]
    private float initialSpeed = 100f;

    [SerializeField] [Tooltip("Factor for lenght of the ray.")]
    private float rayLengthFactor = 1f;

    [SerializeField] [Tooltip("Layers that will be affected by bullet.")]
    private LayerMask hitLayers;

    [SerializeField] [Tooltip("Time after the bullet disappears.")]
    private float fadeDuration = 0.3f;

    [SerializeField] [Tooltip("Renders trail behind the bullet.")]
    private TrailRenderer trailRenderer;

    [SerializeField] [Tooltip("Time of trail visibility.")]
    private float trailVisibilityTime = 0.005f;

    [SerializeField] [Tooltip("Trail width on start and on end of trail.")]
    private Vector2 trailWidth = new(0.003f, 0.002f);

    private Camera _playerCamera;

    [SerializeField] [Tooltip("Start point of the bullet.")]
    private GameObject startPoint;

    private Rigidbody _rb;

    private void Awake()
    {
        _playerCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
        trailRenderer.time = trailVisibilityTime;
        trailRenderer.startWidth = trailWidth.x;
        trailRenderer.endWidth = trailWidth.y;
        trailRenderer.enabled = enableDebugTrails;

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("bullet"), LayerMask.NameToLayer("bullet"));
    }

    private void OnEnable()
    {
        Invoke(nameof(Deactivate), fadeDuration);
        trailRenderer.enabled = enableDebugTrails;

        var screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        var ray = _playerCamera.ScreenPointToRay(screenCenter);
        var shootDirection = Physics.Raycast(ray, out var hit)
            ? (hit.point - transform.position).normalized
            : ray.direction.normalized;
        _rb.velocity = shootDirection * initialSpeed;
    }

    private void OnDisable()
    {
        trailRenderer.enabled = false;
        CancelInvoke();
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
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

        var screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        var ray = _playerCamera.ScreenPointToRay(screenCenter);
        var shootDirection = Physics.Raycast(ray, out var hit)
            ? (hit.point - transform.position).normalized
            : ray.direction.normalized;

        var currentRayLength = initialSpeed * rayLengthFactor;
        if (Physics.Raycast(transform.position, shootDirection, out hit, currentRayLength, hitLayers))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                hit.collider.gameObject.GetComponent<HealthController>().TakeDamage(30f);
            }

            Invoke(nameof(Deactivate), fadeDuration);
        }
    }
}