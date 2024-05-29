using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class ScoreBoard : MonoBehaviour
{
    public TMP_Text username;
    public TMP_Text playerDeaths;
    public TMP_Text playerKills;

    public void Initialize()
    {
        username.text = PhotonNetwork.NickName;
    }
}
