using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitIndicator : MonoBehaviour
{
    public GameObject hitIndicatorPrefab; // Prefab for the hit indicator
    public GameObject indicatorParent;    // Reference to the parent GameObject
    public float indicatorDuration = 1.0f; // How long the indicator stays on screen
    public float randomRadius = 45.0f; // Radius within which random impacts can occur

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            GenerateRandomImpact();
        }
    }

    public void ShowHitIndicator(Vector3 bulletPosition)
    {
        // Calculate direction from player to bullet
        Vector3 direction = bulletPosition - transform.position;
        direction.z = 0; // Ignore the z-axis, assuming 2D gameplay
        direction.Normalize();

        // Calculate the angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Spawn the indicator
        GameObject indicator = Instantiate(hitIndicatorPrefab, indicatorParent.transform);

        // Apply a random z rotation
        float randomZRotation = Random.Range(0f, 360f);
        indicator.transform.rotation = Quaternion.Euler(0, 0, randomZRotation);

        // Destroy the indicator after a short duration
        Destroy(indicator, indicatorDuration);
    }

    public void GenerateRandomImpact()
    {
        // Generate a random direction
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // Generate a random distance within the specified radius
        float randomDistance = Random.Range(0.5f, randomRadius);

        // Calculate the random impact position
        Vector3 randomImpactPosition = transform.position + (Vector3)randomDirection * randomDistance;

        // Call the ShowHitIndicator method with the generated position
        ShowHitIndicator(randomImpactPosition);
    }
}
