using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraController : MonoBehaviour
{
    public static MainMenuCameraController instance;

    public Transform[] focusPoints;
    public float transitionSpeed = 2.0f;
    public float rotationSpeed = 2.0f;

    private Transform targetPoint;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Awake()
    {
        if (instance == null) 
            instance = this;
    }

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (targetPoint != null)
        {
            transform.position = Vector3.Lerp(transform.position, targetPoint.position, Time.deltaTime * transitionSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetPoint.rotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void FocusOn(int index)
    {
        if (index >= 0 && index < focusPoints.Length)
            targetPoint = focusPoints[index];
    }

    public void ResetCamera()
    {
        targetPoint = null;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}