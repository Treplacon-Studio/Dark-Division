using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerIdentifier : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI PlayerNameText;

    void Start()
    {
        SetPlayerUI();
    }

    public void SetPlayerUI()
    {
        if (!string.IsNullOrWhiteSpace(photonView.Owner.NickName) && PlayerNameText != null)
            PlayerNameText.text = photonView.Owner.NickName ?? "No name";
    }
}
