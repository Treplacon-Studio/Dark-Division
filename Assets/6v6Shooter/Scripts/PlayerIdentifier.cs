using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerIdentifier : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TextMeshProUGUI playerNameText;

    void Start()
    {
        SetPlayerUI();
    }

    public void SetPlayerUI() {
        if (playerNameText != null) {
            playerNameText.text = photonView.Owner.NickName;
        }
    }
}
