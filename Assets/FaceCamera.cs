using UnityEngine;
using Photon.Pun;


public class FaceCamera : MonoBehaviourPun
{
    private Transform trans;
    private UnityEngine.Vector3 offset = new UnityEngine.Vector3(0, 180, 0);
     public PhotonView PhotonView;
     public GameObject NameConatiner;

    void Start()
    {
        if (PhotonView.IsMine is true)
            NameConatiner.SetActive(false);

       trans = GameObject.Find("Main Camera").GetComponent<Transform>();
    }

    void Update()
    {
        transform.LookAt(trans);
        transform.Rotate(offset);
    }
}
