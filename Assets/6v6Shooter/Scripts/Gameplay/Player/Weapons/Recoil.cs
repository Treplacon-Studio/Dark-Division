using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Recoil : MonoBehaviour
{
    //original recoil variables
    public float recoil;
    public float maxRecoilX = -2.5f;
    public float defaultMaxY = 10f;
    public float maxRecoilY;
    public float recoilSpeed = 40f;
    public Quaternion originalRotation;
   
    //camera shaking variables
    public float cameraShakeIntensity = 5f;
    public float cameraShakeDuration = 0.1f;
    private float _shakeTimer;
    private Vector3 _cameraOriginalPosition;
    
    private CinemachineVirtualCamera _playerVirtualCamera;
    

    void Awake()
    {
        _playerVirtualCamera = ActionsManager.Instance.ComponentHolder.bulletPoolingManager
            .player.FindComponentInDescendants<CinemachineVirtualCamera>();
        cameraShakeIntensity *= 0.00001f;
    }
    
    void Start() {
        originalRotation = Quaternion.Euler(90, 0, 0);
        _cameraOriginalPosition = _playerVirtualCamera.transform.localPosition;
    }

    public void StartRecoil (float recoilParam)
    {
        recoil = recoilParam;
        maxRecoilY = Random.Range(-defaultMaxY, defaultMaxY);
        _shakeTimer = cameraShakeDuration;
    }

    void Recoiling ()
    {
        if (recoil > 0f) {
            var maxRecoil = Quaternion.Euler (90+maxRecoilX, maxRecoilY, 0f);
            transform.localRotation = Quaternion.Slerp (transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);
            recoil -= Time.deltaTime * recoilSpeed;
        } else {
            recoil = 0f;
            transform.localRotation = Quaternion.Slerp (transform.localRotation, originalRotation, Time.deltaTime * recoilSpeed);
        }
        
        if (_playerVirtualCamera != null && _shakeTimer > 0f)
        {
            var shakeX = Random.Range(-cameraShakeIntensity, cameraShakeIntensity);
            var shakeY = Random.Range(-cameraShakeIntensity, cameraShakeIntensity);
            _playerVirtualCamera.transform.localPosition = _cameraOriginalPosition + new Vector3(shakeX, shakeY, 0f);
            _shakeTimer -= Time.deltaTime;
        }
        else
        {
            _playerVirtualCamera.transform.localPosition = _cameraOriginalPosition;
        }
    }
    
    public Vector3 GetRecoilOffset()
    {
        var offsetRot = transform.localRotation * Quaternion.Inverse(originalRotation);
        var offset = offsetRot.eulerAngles;
        offset.x = Mathf.DeltaAngle(0, offset.x);
        offset.y = Mathf.DeltaAngle(0, offset.y);
        offset.z = Mathf.DeltaAngle(0, offset.z);
        return offset;
    }
    
    void Update ()
    {
        Recoiling ();
    }
}
