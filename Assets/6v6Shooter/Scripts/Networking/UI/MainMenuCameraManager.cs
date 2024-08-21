using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraController : MonoBehaviour
{
    public static MainMenuCameraController instance;

    [Header("Menu transitions")]
    [SerializeField] private Transform[] transitionTargets;
    [SerializeField] private float transitionDuration = 1.0f;
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
        Vector3 initialPosition = lobbyCamera.transform.position;
        Quaternion initialRotation = lobbyCamera.transform.rotation;

        Vector3 targetPosition = target.position;
        Quaternion targetRotation = target.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;

            lobbyCamera.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            lobbyCamera.transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        lobbyCamera.transform.position = targetPosition;
        lobbyCamera.transform.rotation = targetRotation;
    }
}