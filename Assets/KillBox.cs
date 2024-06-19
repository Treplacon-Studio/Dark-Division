using UnityEngine;

public class KillBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        HealthController healthController = other.GetComponent<HealthController>();

        if (healthController != null)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Stop all movement
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            // Apply damage to the player
            healthController.TakeDamage(100f);
        }
    }
}
