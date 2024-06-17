using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    // Damage value set to be greater than the player's max health to ensure it kills
    public float lethalDamage = 1000f;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger has a HealthController component
        HealthController healthController = other.GetComponent<HealthController>();
        if (healthController != null)
        {
            // Call TakeDamage with lethal damage
            healthController.TakeDamage(lethalDamage);
        }
    }
}
