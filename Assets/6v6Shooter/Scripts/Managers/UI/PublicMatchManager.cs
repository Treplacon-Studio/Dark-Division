using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PublicMatchManager : MonoBehaviourPunCallbacks
{
    void Start() {
		if (PhotonNetwork.IsMasterClient) {
			SpawnPlayerTracker();
		}
    }

    void SpawnPlayerTracker() {
		Debug.Log("Player Tracker Spawned");
		GameObject newTracker = PhotonNetwork.Instantiate(Path.Combine("Gameplay", "PlayerTracker"), Vector3.zero, Quaternion.identity);
	}
}
