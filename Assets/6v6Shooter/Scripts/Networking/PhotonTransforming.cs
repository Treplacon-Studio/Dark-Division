using Photon.Pun;
using UnityEngine;

public class PhotonTransforming :  MonoBehaviourPun, IPunObservable
{
    [SerializeField] private bool position;
    [SerializeField] private bool rotation;
    [SerializeField] private bool scale;
    [SerializeField] private bool isLocal;
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (isLocal)
        {
            if (stream.IsWriting)
            {
                if (position)
                    stream.SendNext(transform.localPosition);
                if (rotation)
                    stream.SendNext(transform.localRotation);
                if (scale)
                    stream.SendNext(transform.localScale);
            }
            else
            {
                if (position)
                    transform.localPosition = (Vector3)stream.ReceiveNext();
                if (rotation)
                    transform.localRotation = (Quaternion)stream.ReceiveNext();
                if (scale)
                    transform.localScale = (Vector3)stream.ReceiveNext();
            }
        }
        else
        {
            if (stream.IsWriting)
            {
                if (position)
                    stream.SendNext(transform.position);
                if (rotation)
                    stream.SendNext(transform.rotation);
                if (scale)
                    stream.SendNext(transform.localScale);
            }
            else
            {
                if (position)
                    transform.position = (Vector3)stream.ReceiveNext();
                if (rotation)
                    transform.rotation = (Quaternion)stream.ReceiveNext();
                if (scale)
                    transform.localScale = (Vector3)stream.ReceiveNext();
            }
        }
    }
}
