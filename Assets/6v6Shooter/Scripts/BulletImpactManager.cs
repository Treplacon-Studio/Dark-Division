using System;
using UnityEngine;

public class BulletImpactManager : MonoBehaviour
{
    public static BulletImpactManager Instance { get; private set; }

    public Action<Vector3> OnHitFeedback;

    private void Awake()
    {
        // Ensure Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Method to register for hit feedback
    public void RegisterHitFeedback(Action<Vector3> callback)
    {
        OnHitFeedback += callback;
    }

    // Method to unregister from hit feedback
    public void UnregisterHitFeedback(Action<Vector3> callback)
    {
        OnHitFeedback -= callback;
    }

    // Method to trigger hit feedback
    public void TriggerHitFeedback(Vector3 position)
    {
        OnHitFeedback?.Invoke(position);
    }
}
