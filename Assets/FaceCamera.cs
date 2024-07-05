using UnityEngine;
using Photon.Pun;


public class FaceCamera : MonoBehaviourPun
{
    private Transform trans;
    private UnityEngine.Vector3 offset = new UnityEngine.Vector3(0, 180, 0);

    void Start()
    {
       trans = GameObject.Find("Main Camera").GetComponent<Transform>();
    }

    void Update()
    {
        transform.LookAt(trans);
        transform.Rotate(offset);
    }
}
