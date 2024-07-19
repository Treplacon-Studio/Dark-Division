using Cinemachine;
using UnityEngine;

public class WeaponLerpToCameraPosition : MonoBehaviour
{
    public CinemachineVirtualCamera VirtualCamera; // Reference to the Cinemachine virtual camera
    public float LerpSpeed = 5f;                   // Speed of the lerp movement
    public float ViewDistance = 1f;                // Distance to move the gun closer to the camera

    private Vector3 _originalPosition;
    private bool _isLerping = false;
    private Vector3 _targetPosition;
    private bool _isDisplayed = false;

    void Start()
    {
        _originalPosition = transform.position;
    }

    void Update()
    {
        if (_isLerping)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * LerpSpeed);
            if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
            {
                _isLerping = false;
                _isDisplayed = true;
            }
        }
    }

    public void LerpToCamera()
    {
        if (VirtualCamera != null)
        {
            Transform cameraTransform = VirtualCamera.transform;
            _targetPosition = cameraTransform.position + cameraTransform.forward * ViewDistance;
            _isLerping = true;
            _isDisplayed = false;
        }
        else
        {
            Debug.LogWarning("VirtualCamera is not assigned.");
        }
    }

    public void ResetPosition()
    {
        transform.position = _originalPosition;
        _isLerping = false;
        _isDisplayed = false;
    }

    public bool IsDisplayed()
    {
        return _isDisplayed;
    }
}
