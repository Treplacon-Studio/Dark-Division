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
    public void ReceiveBulletDirection(Rigidbody velocity)
    {
        Vector3 forward = gameObject.transform.forward;
        Vector3 bulletVelocity = velocity.velocity;

        float dotProduct = Vector3.Dot(forward, bulletVelocity);
        float angle = dotProduct / (bulletVelocity.magnitude * forward.magnitude);
        float radians = Mathf.Acos(angle);
        float degreeAngle = radians * Mathf.Rad2Deg;

        ShowHitIndicator(degreeAngle);
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
