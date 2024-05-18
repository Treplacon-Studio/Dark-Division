using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

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
