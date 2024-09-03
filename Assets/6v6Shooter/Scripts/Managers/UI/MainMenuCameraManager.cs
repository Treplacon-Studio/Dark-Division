using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraController : MonoBehaviour
{
    public static MainMenuCameraController instance;

    [Header("Menu transitions")]
    [SerializeField] private Transform[] transitionTargets;    // Targets to move to
    [SerializeField] private Transform intermediatePoint;      // Intermediate point
    [SerializeField] private float transitionDuration = 1.0f;  // Total duration of the transition
    [SerializeField] private Camera lobbyCamera;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void FocusOn(int index)
    {
        if (index >= 0 && index < transitionTargets.Length)
            StartCoroutine(TransitionCoroutine(transitionTargets[index]));
    }

    private IEnumerator TransitionCoroutine(Transform target)
    {
        Debug.Log("Yep");
        Vector3 initialPosition = lobbyCamera.transform.position;
        Quaternion initialRotation = lobbyCamera.transform.rotation;

        Vector3 intermediatePosition = intermediatePoint.position;
        Quaternion intermediateRotation = intermediatePoint.rotation;

        Vector3 targetPosition = target.position;
        Quaternion targetRotation = target.rotation;

        float halfDuration = transitionDuration / 2f;  // Split duration into two parts

        // Move from initial position to intermediate point
        yield return LerpCamera(initialPosition, initialRotation, intermediatePosition, intermediateRotation, halfDuration);

        // Move from intermediate point to target position
        yield return LerpCamera(intermediatePosition, intermediateRotation, targetPosition, targetRotation, halfDuration);
    }

    private IEnumerator LerpCamera(Vector3 startPosition, Quaternion startRotation, Vector3 endPosition, Quaternion endRotation, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            lobbyCamera.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            lobbyCamera.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        lobbyCamera.transform.position = endPosition;
        lobbyCamera.transform.rotation = endRotation;
    }
}
