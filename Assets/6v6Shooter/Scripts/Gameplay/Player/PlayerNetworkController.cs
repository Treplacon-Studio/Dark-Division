using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerNetworkController : MonoBehaviourPunCallbacks
{
    public bool PhotonViewIsMine()
    {
        return photonView.IsMine;
    }
}
