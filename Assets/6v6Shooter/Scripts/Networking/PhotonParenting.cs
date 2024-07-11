using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PhotonParenting : MonoBehaviourPunCallbacks
{
    public void Parent(GameObject parent)
    {
        photonView.RPC("SetParent", RpcTarget.AllBuffered, 
            GetComponent<PhotonView>().ViewID, parent.GetComponent<PhotonView>().ViewID);
    }
    
    [PunRPC]
    void SetParent(int objectViewID, int parentViewID)
    {
        var objectView = PhotonView.Find(objectViewID);
        var parentView = PhotonView.Find(parentViewID);

        if (objectView != null && parentView != null)
            objectView.transform.SetParent(parentView.transform);
    }
}
