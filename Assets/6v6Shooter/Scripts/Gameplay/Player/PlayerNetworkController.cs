using UnityEngine;
using Photon.Pun;

public class PlayerNetworkController : MonoBehaviourPunCallbacks
{
    private int _photonPlayerID;

    void Awake()
    {
        _photonPlayerID = photonView.Owner.ActorNumber;
    }

    public int GetPlayerPhotonId()
    {
        return _photonPlayerID;
    }

    public bool PhotonViewIsMine()
    {
        return photonView.IsMine;
    }
}
