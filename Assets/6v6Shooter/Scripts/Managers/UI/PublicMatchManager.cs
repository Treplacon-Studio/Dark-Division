using UnityEngine;
using Photon.Pun;
using System.IO;

public class PublicMatchManager : MonoBehaviour
{
	public static PublicMatchManager instance;

	void OnEnable() {
        if(instance == null) {
            instance = this;
        }
    }

    void Start() {
		if (PhotonNetwork.IsMasterClient) {
			SpawnPlayerTracker();
		}
    }

    private void SpawnPlayerTracker() {
		Debug.Log("Player Tracker Spawned");
		GameObject newTracker = PhotonNetwork.Instantiate(Path.Combine("Gameplay", "PlayerTracker"), Vector3.zero, Quaternion.identity);
	}
}
