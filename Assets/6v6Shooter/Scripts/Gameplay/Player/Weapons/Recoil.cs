using UnityEngine;

public class Recoil : MonoBehaviour
{
    public float recoil;
    public float maxRecoilX = -30f;
    public float maxRecoilY = 3f;
    public float recoilSpeed = 50f;

    public Quaternion _originalRotation;

    void Start() {
        _originalRotation = Quaternion.Euler(90, 0, 0);
    }

    public void StartRecoil (float recoilParam, float maxX, float maxY, float recoilSpeedParam)
    {
        recoil = recoilParam;
        maxRecoilX = maxX;
        recoilSpeed = recoilSpeedParam;
        maxRecoilY = Random.Range(-maxY, maxY);
    }

    void Recoiling ()
    {
        if (recoil > 0f) {
            var maxRecoil = Quaternion.Euler (90+maxRecoilX, maxRecoilY, 0f);
            transform.localRotation = Quaternion.Slerp (transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);
            recoil -= Time.deltaTime * recoilSpeed;
        } else {
            recoil = 0f;
            transform.localRotation = Quaternion.Slerp (transform.localRotation, _originalRotation, Time.deltaTime * recoilSpeed);
        }
    }
    
    public Vector3 GetRecoilOffset()
    {
        var offsetRot = transform.localRotation * Quaternion.Inverse(_originalRotation);
        var offset = offsetRot.eulerAngles;
        offset.x = Mathf.DeltaAngle(0, offset.x);
        offset.y = Mathf.DeltaAngle(0, offset.y);
        offset.z = Mathf.DeltaAngle(0, offset.z);
        return offset;
    }
    
    void Update ()
    {
        Recoiling ();
    }
}
