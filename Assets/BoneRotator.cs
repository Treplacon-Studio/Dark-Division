using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoneRotator : MonoBehaviour
{
    private Transform bone; // The bone to control
    public float rotationSpeed = 100f; // Speed of rotation
    [SerializeField] private InputAction lookAction;

    private Vector3 currentEulerAngles;

    void Start()
    {
        bone = this.transform;
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
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        float lookY = lookInput.y;

        currentEulerAngles.x -= lookY * rotationSpeed * Time.deltaTime;

        // Clamping the X rotation for looking up and down
        currentEulerAngles.x = Mathf.Clamp(currentEulerAngles.x, -90f, 90f);

        bone.localEulerAngles = currentEulerAngles;
    }
}
