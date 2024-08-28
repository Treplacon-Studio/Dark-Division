using System;
using FMODUnity;
using UnityEngine;

public class BulletImpactLogic : MonoBehaviour
{
    public GameObject hitmarkerPrefab;            // Hitmarker UI prefab
    public Transform hitmarkerParent;             // Parent transform for the hitmarker
    public EventReference hitmarkerSound;         // Sound to play when a hit occurs

    private void OnEnable()
    {
        BulletImpactManager.Instance.RegisterHitFeedback(HandleImpact);
    }

    private void OnDisable()
    {
        BulletImpactManager.Instance.UnregisterHitFeedback(HandleImpact);
    }

    private void HandleImpact(Vector3 position)
    {
        // Play hitmarker sound
        if (AudioManager.Instance != null && !hitmarkerSound.IsNull)
        {
            AudioManager.Instance.PlayOneShot(hitmarkerSound, position);
        }

        // Instantiate hitmarker with random rotation
        float randomZRotation = UnityEngine.Random.Range(-25f, 25f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomZRotation);

        GameObject hitmarkerInstance = Instantiate(hitmarkerPrefab, hitmarkerParent);
        hitmarkerInstance.transform.localRotation = randomRotation;

        // Destroy hitmarker after 3 seconds
        Destroy(hitmarkerInstance, 3f);
    }
}
