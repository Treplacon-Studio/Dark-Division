using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class BoneRotator : MonoBehaviour
{
    private Transform bone; // The bone to control
    public float rotationSpeed = 100f; // Speed of rotation
    [SerializeField] private InputAction lookAction;

    public PhotonView PhotonView;

    private Vector3 currentEulerAngles;

    void Start()
    {
        bone = transform;
        currentEulerAngles = bone.localEulerAngles;
    }

    void OnEnable()
    {
        lookAction.Enable();
    }

    void OnDisable()
    {
        lookAction.Disable();
    }

    void LateUpdate()
    {
        if (PhotonView.IsMine is false)
            return;

        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        float lookY = lookInput.y;

        currentEulerAngles.x -= lookY * rotationSpeed * Time.deltaTime;

        // Clamping the X rotation for looking up and down
        currentEulerAngles.x = Mathf.Clamp(currentEulerAngles.x, -90f, 90f);

        bone.localEulerAngles = currentEulerAngles;
    }

    public void AddRotation(float rotation)
    {
        currentEulerAngles.x -= rotation;
        currentEulerAngles.x = Mathf.Clamp(currentEulerAngles.x, -90f, 90f);
    }
}
