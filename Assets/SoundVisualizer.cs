using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
public class SoundVisualizer : MonoBehaviour
{
    public float minDistance = 5f; // Distance at which sound starts attenuating
    public float maxDistance = 20f; // Distance at which sound is inaudible
    public Color minDistanceColor = Color.green; // Color for minimum distance sphere
    public Color maxDistanceColor = Color.red; // Color for maximum distance sphere
    public float debugSphereRadius = 0.1f; // Radius of the debug spheres

    private FMODUnity.StudioEventEmitter emitter;

    private void Start()
    {
        // Get the FMOD emitter component
        emitter = GetComponent<FMODUnity.StudioEventEmitter>();

        // Optionally, you can set the minDistance and maxDistance values
        // according to the FMOD event settings or specific game requirements
    }

    private void OnDrawGizmos()
    {
        if (emitter == null)
            return;

        // Draw a sphere for the minimum distance
        Gizmos.color = minDistanceColor;
        Gizmos.DrawWireSphere(transform.position, minDistance);

        // Draw a sphere for the maximum distance
        Gizmos.color = maxDistanceColor;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

    private void OnDrawGizmosSelected()
    {
        if (emitter == null)
            return;

        // Draw filled spheres for visualization
        Gizmos.color = minDistanceColor;
        Gizmos.DrawSphere(transform.position, debugSphereRadius);

        Gizmos.color = maxDistanceColor;
        Gizmos.DrawSphere(transform.position, debugSphereRadius);
    }
}
