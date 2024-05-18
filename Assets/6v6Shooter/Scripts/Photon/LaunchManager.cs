using UnityEngine;
using Photon.Pun;
using UnityEditor.MemoryProfiler;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public GameObject UsernameCreationMenu;
    public GameObject ConnectionStatusPanel;

    #region Unity Methods

    void Start()
    {
        UsernameCreationMenu.SetActive(true);
        ConnectionStatusPanel.SetActive(false);
    }

    #endregion

    #region Photon Callbacks

    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            ConnectionStatusPanel.SetActive(true);
            UsernameCreationMenu.SetActive(false);
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + " connected to Photon Server");
    }

    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    #endregion
}
