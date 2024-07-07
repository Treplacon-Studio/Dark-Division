using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TargetDummy :  MonoBehaviourPun, IPunObservable
{

    public bool isMoveableTarget;

    public float speed = 1f;
    public float distance = 5f;

    private Vector3 startPosition;
    private float randomOffset;

    void Start()
    {
        startPosition = transform.position;
        randomOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        if (isMoveableTarget && photonView.IsMine)
            MoveTarget();
    }

    private void MoveTarget()
    {
        float movement = Mathf.PingPong(Time.time * speed + randomOffset, distance * 2) - distance;
        Vector3 localPosition = transform.localPosition;
        localPosition.z = movement;
        transform.localPosition = localPosition;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //Send data to other clients
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            //Receive data from other clients
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
