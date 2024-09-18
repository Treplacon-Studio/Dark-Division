using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerKillFeedListener : MonoBehaviour, IOnEventCallback
{
    [SerializeField] private Transform killfeedContainer;

    private void Awake()
    {
        Debug.Log("PlayerKillFeedListener instance created.");
    }
    private void OnEnable()
    {
        Debug.Log("Adding callback target.");
        // PhotonNetwork.AddCallbackTarget(this);
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        Debug.Log("Removing callback target.");
        // PhotonNetwork.RemoveCallbackTarget(this);
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }


    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 0)
        {
            object[] data = (object[])photonEvent.CustomData;

            string victimName = (string)data[1];
            string killerName = (string)data[2];
            string weaponName = (string)data[3];

            UpdateKillFeed(victimName, killerName, weaponName);
        }
    }


    private void UpdateKillFeed(string victimName, string killerName, string weaponName)
    {
    
        KillFeedManager.Instance.UpdateKillFeedLocally(victimName, killerName, weaponName, killfeedContainer );
    }
}
