using System;
using FMODUnity;
using UnityEngine;

public class BulletImpactLogic : MonoBehaviour
{
    public GameObject hitmarkerPrefab;
    public Transform hitmarkerParent;
    public EventReference hitmarkerSound;

    private void OnEnable()
    {
        BulletImpactManager.Instance.RegisterHitFeedback(HandleImpact);
    }

    private void OnDisable()
    {
        BulletImpactManager.Instance.UnregisterHitFeedback(HandleImpact);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Keypad3))
        {
            HandleImpact(Vector3.zero);
        }
    }

    private void HandleImpact(Vector3 position)
    {
        if (AudioManager.Instance != null && !hitmarkerSound.IsNull)
        {
            AudioManager.Instance.PlayOneShot(hitmarkerSound, position);
        }

        float randomZRotation = UnityEngine.Random.Range(-25f, 25f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomZRotation);

        GameObject hitmarkerInstance = Instantiate(hitmarkerPrefab, hitmarkerParent);
        hitmarkerInstance.transform.localRotation = randomRotation;

        Destroy(hitmarkerInstance, 3f);
    }
}
