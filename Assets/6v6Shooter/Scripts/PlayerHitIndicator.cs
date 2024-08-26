using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitIndicator : MonoBehaviourPunCallbacks
{
    public GameObject hitIndicatorPrefab; // Prefab for the hit indicator
    public GameObject indicatorParent;    // Reference to the parent GameObject
    public float indicatorDuration = 1.0f; // How long the indicator stays on screen
    public float randomRadius = 45.0f; // Radius within which random impacts can occur

    [PunRPC]
    public void ReceiveBulletDirection(Vector3 velocity)
    {
        Vector3 forward = gameObject.transform.forward;
        Vector3 bulletVelocity = velocity.normalized;
        float dotProduct = Vector3.Dot(forward, bulletVelocity);
        Vector3 crossProduct = Vector3.Cross(forward, bulletVelocity);

        float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        if (crossProduct.y > 0)
            angle = 360f - angle;

        if (crossProduct.z < 0)
            angle = -angle;

        ShowHitIndicator(180 - angle);
    }

    public void ShowHitIndicator(float bulletAngle)
    {
        // Spawn the indicator
        GameObject indicator = Instantiate(hitIndicatorPrefab, indicatorParent.transform);
        indicator.transform.rotation = Quaternion.Euler(0, 0, bulletAngle);

        // Destroy the indicator after a short duration
        Destroy(indicator, indicatorDuration);
    }
}
