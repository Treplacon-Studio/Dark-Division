using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Recoil : MonoBehaviour
{
    public float recoil;
    
    //original recoil variables
    [Header("Non ADS recoil")][Space(10f)]
    public float verticalRecoilLimit = -2.5f;
    public float horizontalRecoilLimit = 10f;
    public float randomHorizontalRecoil;
    public float recoilSpeed = 40f;
    public float cameraRecoilPower = 0.005f;
    public float cameraShakeIntensity = 5f;
    public float cameraShakeDuration = 0.1f;
    
    [Header("ADS recoil")][Space(10f)]
    public float verticalRecoilLimitAds = -2.5f;
    public float horizontalRecoilLimitAds = 10f;
    public float randomHorizontalRecoilAds;
    public float recoilSpeedAds = 40f;
    public float cameraRecoilPowerAds = 0.005f;
    public float cameraShakeIntensityAds = 5f;
    public float cameraShakeDurationAds = 0.1f;
    
    //camera shaking variables
    private Quaternion _originalRotation;
    private float _shakeTimer;
    private Vector3 _cameraOriginalPosition;
    private CinemachineVirtualCamera _playerVirtualCamera;
    private BoneRotator _playerBoneRotator;

    private PlayerNetworkController _pnc;
    
    void Start() {
        _pnc ??= PlayerUtils.FindComponentInParents<PlayerNetworkController>(gameObject);
        _playerVirtualCamera = ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.bulletPoolingManager
            .player.FindComponentInDescendants<CinemachineVirtualCamera>();
        _playerBoneRotator = ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.bulletPoolingManager
            .player.FindComponentInDescendants<BoneRotator>();
        cameraShakeIntensity *= 0.00001f;
        cameraShakeIntensityAds *= 0.00001f;
        
        _originalRotation = Quaternion.Euler(90, 0, 0);
        _cameraOriginalPosition = _playerVirtualCamera.transform.localPosition;
    }

    public void StartRecoil (float recoilParam)
    {
        recoil = recoilParam;
        randomHorizontalRecoil = Random.Range(-horizontalRecoilLimit, horizontalRecoilLimit);
        randomHorizontalRecoilAds = Random.Range(-horizontalRecoilLimitAds, horizontalRecoilLimitAds);
        _shakeTimer = ActionsManager.GetInstance(_pnc.GetInstanceID()).Aiming.IsAiming() ? cameraShakeDurationAds : cameraShakeDuration;
    }

    void Recoiling ()
    {
        if (recoil > 0f) {
            var maxRecoil = Quaternion.Euler (90 + verticalRecoilLimit, randomHorizontalRecoil, 0f);
            transform.localRotation = Quaternion.Slerp (transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);
            recoil -= Time.deltaTime * recoilSpeed;
        } else {
            recoil = 0f;
            transform.localRotation = Quaternion.Slerp (transform.localRotation, _originalRotation, Time.deltaTime * recoilSpeed);
        }
        
        if (_playerVirtualCamera != null && _shakeTimer > 0f)
        {
            var shakeX = Random.Range(-cameraShakeIntensity, cameraShakeIntensity);
            var shakeY = Random.Range(-cameraShakeIntensity, cameraShakeIntensity);
            _playerVirtualCamera.transform.localPosition = _cameraOriginalPosition + new Vector3(shakeX, shakeY, 0f);
            _shakeTimer -= Time.deltaTime;
            _playerBoneRotator.AddRotation(cameraRecoilPower);
        }
        else
        {
            _playerVirtualCamera.transform.localPosition = _cameraOriginalPosition;
        }
    }
    
    void RecoilingAds()
    {
        if (recoil > 0f) {
            var maxRecoil = Quaternion.Euler (90+ verticalRecoilLimitAds, randomHorizontalRecoilAds, 0f);
            transform.localRotation = Quaternion.Slerp (transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeedAds);
            recoil -= Time.deltaTime * recoilSpeedAds;
        } else {
            recoil = 0f;
            transform.localRotation = Quaternion.Slerp (transform.localRotation, _originalRotation, Time.deltaTime * recoilSpeedAds);
        }
        
        if (_playerVirtualCamera != null && _shakeTimer > 0f)
        {
            var shakeX = Random.Range(-cameraShakeIntensityAds, cameraShakeIntensityAds);
            var shakeY = Random.Range(-cameraShakeIntensityAds, cameraShakeIntensityAds);
            _playerVirtualCamera.transform.localPosition = _cameraOriginalPosition + new Vector3(shakeX, shakeY, 0f);
            _shakeTimer -= Time.deltaTime;
            _playerBoneRotator.AddRotation(cameraRecoilPowerAds);
        }
        else
        {
            _playerVirtualCamera.transform.localPosition = _cameraOriginalPosition;
        }
    }
    
    public Vector3 GetRecoilOffset()
    {
        var offsetRot = transform.localRotation * Quaternion.Inverse(_originalRotation);
        var offset = offsetRot.eulerAngles;
        offset.x = Mathf.DeltaAngle(0, offset.x);
        offset.y = Mathf.DeltaAngle(0, offset.y);
        offset.z = Mathf.DeltaAngle(0, offset.z);
        return offset;
    }
    
    void Update ()
    {
        _pnc ??= PlayerUtils.FindComponentInParents<PlayerNetworkController>(gameObject);
        if (ActionsManager.GetInstance(_pnc.GetInstanceID()).Aiming is null)
            return;
        
        if (ActionsManager.GetInstance(_pnc.GetInstanceID()).Aiming.IsAiming())
            RecoilingAds();
        else Recoiling();
    }
}
