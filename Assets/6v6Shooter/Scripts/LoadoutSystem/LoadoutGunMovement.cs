using UnityEngine;

public class LoadoutGunMovement : MonoBehaviour
{
     public float tiltSpeed = 100.0f;
    public float maxTiltAngleX = 45.0f;
    public float maxTiltAngleY = 45.0f;
    public float resetSpeed = 5.0f;

    private Quaternion initialRotation;
    private bool isTilting = false;

    void Start()
    {
        initialRotation = Quaternion.Euler(0, -90, 0);
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            TiltGun();
            isTilting = true;
        }
        else
        {
            if (isTilting)
                ResetGun();
        }
    }

    void TiltGun()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float tiltX = -mouseX * tiltSpeed * Time.deltaTime;
        float tiltZ = mouseY * tiltSpeed * Time.deltaTime;

        transform.Rotate(tiltX, 0, tiltZ, Space.Self);

        Vector3 currentRotation = transform.localEulerAngles;
        currentRotation.x = ClampAngle(currentRotation.x, initialRotation.eulerAngles.x - maxTiltAngleX, initialRotation.eulerAngles.x + maxTiltAngleX);
        currentRotation.z = ClampAngle(currentRotation.z, initialRotation.eulerAngles.z - maxTiltAngleY, initialRotation.eulerAngles.z + maxTiltAngleY);

        transform.localEulerAngles = new Vector3(currentRotation.x, transform.localEulerAngles.y, currentRotation.z);
    }

    void ResetGun()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, resetSpeed * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, initialRotation) < 0.1f)
        {
            transform.rotation = initialRotation; 
            isTilting = false;
        }

        Vector3 fixedRotation = transform.localEulerAngles;
        fixedRotation.y = initialRotation.eulerAngles.y;
        transform.localEulerAngles = fixedRotation;
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < 90 || angle > 270)
        {
            if (angle > 180) angle -= 360;
            if (max > 180) max -= 360;
            if (min > 180) min -= 360;
        }
        angle = Mathf.Clamp(angle, min, max);
        if (angle < 0) angle += 360;
        return angle;
    }
}
