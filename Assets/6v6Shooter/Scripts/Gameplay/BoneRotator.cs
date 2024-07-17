using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class BoneRotator : MonoBehaviourPunCallbacks
{
    public float rotationSpeed = 100f; // Speed of rotation
    [SerializeField] private InputAction lookAction;

    private Vector3 currentEulerAngles;

    void Start()
    {
        currentEulerAngles = transform.localEulerAngles;
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
        if (photonView.IsMine is false)
            return;

        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        float lookY = lookInput.y;

        currentEulerAngles.x -= lookY * rotationSpeed * Time.deltaTime;

        // Clamping the X rotation for looking up and down
        currentEulerAngles.x = Mathf.Clamp(currentEulerAngles.x, -90f, 90f);

        transform.localEulerAngles = currentEulerAngles;
    }

    public void AddRotation(float rotation)
    {
        currentEulerAngles.x -= rotation;
        currentEulerAngles.x = Mathf.Clamp(currentEulerAngles.x, -90f, 90f);
    }
}
