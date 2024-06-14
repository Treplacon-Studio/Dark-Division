using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start () 
    {
        mainCamera = Camera.main;
    }

    void Update() 
    {
        if (mainCamera != null)
        {
            Vector3 direction = mainCamera.transform.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(-direction);
        }
    }
}