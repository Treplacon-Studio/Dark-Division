using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerIdentifier : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI playerNameText;

    void Start()
    {
        SetPlayerUI();
    }

    public void SetPlayerUI()
    {
        return;
        if (!string.IsNullOrWhiteSpace(photonView.Owner.NickName) && playerNameText != null)
            playerNameText.text = photonView.Owner.NickName ?? "No name";
    }
}
