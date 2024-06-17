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
                // Optionally apply a minimal force to avoid unrealistic behavior
                rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
            }
            
            // Apply damage to the player
            healthController.TakeDamage(healthController.startHealth);
        }
    }
}
