using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerKillFeedListener : MonoBehaviour, IOnEventCallback
{
    [SerializeField] private Transform killfeedContainer;
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    // This method is required by the IOnEventCallback interface
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 0) // Assuming event code 0 for kill feed
        {
            object[] data = (object[])photonEvent.CustomData;

            string victimName = (string)data[0];
            string killerName = (string)data[1];
            string weaponName = (string)data[2];

            Debug.Log($"OnEvent received with victim: {victimName}, killer: {killerName}, weapon: {weaponName}");

            // Call a method to update the local kill feed UI for this player
            UpdateKillFeed(victimName, killerName, weaponName);
        }
    }


    private void UpdateKillFeed(string victimName, string killerName, string weaponName)
    {
        Debug.Log($"Updating local kill feed with victim: {victimName}, killer: {killerName}, weapon: {weaponName}");
    
    // Assuming the KillFeedManager is a singleton
        KillFeedManager.Instance.UpdateKillFeedLocally(victimName, killerName, weaponName, killfeedContainer );
    }
}
