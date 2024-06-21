using UnityEngine;
using Photon.Pun;
using System.IO;

public class PublicMatchManager : MonoBehaviourPunCallbacks
{
    private void Start() {
		//if (PhotonNetwork.IsMasterClient)
			SpawnPlayerTracker();
    }

    private void SpawnPlayerTracker() {
		Debug.Log("Player Tracker Spawned");
		GameObject newTracker = PhotonNetwork.Instantiate(Path.Combine("Gameplay", "PlayerTracker"), Vector3.zero, Quaternion.identity);
	}
}
