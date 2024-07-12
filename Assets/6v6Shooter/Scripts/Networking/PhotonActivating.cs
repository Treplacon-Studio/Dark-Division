using Photon.Pun;

public class PhotonActivating : MonoBehaviourPun
{
    public void SetActive(bool active)
    {
        photonView.RPC("RPC_SetActiveState", RpcTarget.All, active);
    }

    [PunRPC]
    void RPC_SetActiveState(bool state)
    {
        gameObject.SetActive(state);
    }
}
