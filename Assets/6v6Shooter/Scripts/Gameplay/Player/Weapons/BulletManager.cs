using System.Collections;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Weapons
{
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
        private Vector2 trailWidth = new Vector2(0.003f, 0.002f);
        
        [SerializeField] [Tooltip("Camera of the player")]
        private Camera playerCamera;
        
        [SerializeField] [Tooltip("Start point of the bullet.")]
        private GameObject startPoint;
        
        private Rigidbody _rb;

        private void Awake()
        {
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
            var ray = playerCamera.ScreenPointToRay(screenCenter);
            var shootDirection = Physics.Raycast(ray, out var hit) ? 
                (hit.point - transform.position).normalized : ray.direction.normalized;
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

        // void Update()
        // {
        //     //ShootRay();
        // }
        //
        // void ShootRay()
        // {
        //     if (playerCamera == null)
        //     {
        //         Debug.LogError("Camera not found.");
        //         return;
        //     }
        //
        //     RaycastHit hit;
        //     
        //     var screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        //     var ray = playerCamera.ScreenPointToRay(screenCenter);
        //     var shootDirection = ray.direction.normalized;
        //
        //     var currentRayLength = initialSpeed * rayLengthFactor;
        //
        //     if (Physics.Raycast(transform.position, shootDirection, out hit, currentRayLength, hitLayers))
        //     {
        //         Debug.Log("Object was hit by bullet: " + hit.collider.gameObject.name);
        //
        //       
        //         
        //         Invoke(nameof(Deactivate), fadeDuration);
        //     }
        // }
    }
}
