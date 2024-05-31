using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineAnimationController : MonoBehaviour
{

    public Transform spineBone;

    // Update is called once per frame
    void Update()
    {
        // Example: Rotate the spine based on input or any other logic
        float spineRotationX = Mathf.Clamp(23, -30f, 75f);
        spineBone.localRotation = Quaternion.Euler(spineRotationX, 0, 0);
    }
}
