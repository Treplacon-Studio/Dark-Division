using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraController : MonoBehaviour
{
    public Transform[] focusPoints;
    public float transitionSpeed = 2.0f;

    private Transform targetPoint;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (targetPoint != null)
        {
            transform.position = Vector3.Lerp(transform.position, targetPoint.position, Time.deltaTime * transitionSpeed);
            transform.LookAt(targetPoint);
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
    }
}