using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BulletPilot : MonoBehaviour
{
    [SerializeField] private float initialSpeed = 100f;
    [SerializeField] private float rayLengthFactor = 1f;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private GameObject bulletHolePrefab;  // Bullet hole decal prefab

    private Ray currentRay;
    private RaycastHit currentHit;
    private float bulletHoleDestroyDelay = 5f;

    private GameObject _bulletOwner;
    private Camera _playerCamera;
    private PhotonView _photonView;
    private PhotonView _bulletOwnerPhotonView;
    private Rigidbody _rb;
    private HashSet<GameObject> _alreadyHitObjects = new();
    private Recoil _recoil;
    private Vector3 _currentDirection;

    private PlayerNetworkController _pnc;

    public void LateAwake()
    {
        _pnc = PlayerUtils.FindComponentInParents<PlayerNetworkController>(gameObject);
        _bulletOwner = ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.bulletPoolingManager.player;
    }

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        var cameras = FindObjectsOfType<Camera>();
        foreach (var cam in cameras)
        {
            if (cam.gameObject.name.Contains("Main Camera"))
            {
                _playerCamera = cam;
                break;
            }
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

            PhotonView hitPhotonView = hitObject.GetComponent<PhotonView>();
            SpawnHitDecal(hit.point, hit.normal);  // Spawn bullet hole decal

            if (hitPhotonView != null)
            {
                if (hitObject.CompareTag("TargetDummy") == false)
                {
                    if (hitPhotonView.Owner == _bulletOwnerPhotonView.Owner)
                        return;
                }

                if (!_alreadyHitObjects.Contains(hitObject))
                {
                    if (!TargetIsOnSameTeam())
                    {
                        hitPhotonView.RPC("TakeDamage", RpcTarget.AllBuffered, 10f);
                        hitPhotonView.RPC("ReceiveBulletDirection", RpcTarget.AllBuffered, _rb.velocity);
                        BulletImpactManager.Instance?.TriggerHitFeedback(hit.point); // Trigger hit feedback
                    }

                    Debug.Log($"{_bulletOwner.name} hits {hitObject.name}!");
                    _alreadyHitObjects.Add(hitObject);
                    
                    Invoke(nameof(Deactivate), fadeDuration);
                }
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

        var finalDir = baseDir + _playerCamera.transform.rotation * _recoil.GetRecoilOffset();
        finalDir.Normalize();
        return finalDir;
    }

    private void SpawnHitDecal(Vector3 hitPoint, Vector3 hitNormal)
    {
        Debug.Log("Trying to spawn a decal");

        // Offset the hit point slightly away from the surface
        float decalOffset = -0.05f;
        Vector3 decalPosition = hitPoint + hitNormal * decalOffset;

        // Determine the rotation based on the hit normal
        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, hitNormal);

        // Instantiate the bullet hole decal
        GameObject bulletHole = Instantiate(bulletHolePrefab, decalPosition, rotation);
        bulletHole.transform.SetParent(null);

        // Apply a random rotation around the forward axis
        bulletHole.transform.Rotate(Vector3.forward, UnityEngine.Random.Range(0f, 360f));

        // Destroy the decal after a delay
        Destroy(bulletHole, bulletHoleDestroyDelay);
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

    public bool TargetIsOnSameTeam()
    {
        return false; // Implement your logic
    }
}
