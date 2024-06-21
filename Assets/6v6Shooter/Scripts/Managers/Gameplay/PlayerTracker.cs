using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class PlayerTracker : MonoBehaviour, IPunObservable {
	public static PlayerTracker instance { get; set; }

	public PhotonView pv { get; set; }

	private void OnEnable() {
		if (instance == null) {
			instance = this;
			pv = GetComponent<PhotonView>();
		}
	}

	private void Start() {
        // We spawn the player from here because the PlayerTracker absolutely needs to exist
        // on the client before the player is spawned so that they can be added to the tracker.
		string team = TeamManager.GetTeam(PhotonNetwork.NickName);
        PublicMatchSpawnManager.instance.SpawnPlayer(team);
	}

	[PunRPC]
	public void AddPlayer(int id) {
		PhotonView pv = PhotonView.Find(id);
	}

	[PunRPC]
	public void RemovePlayer(int id) {
		PhotonView pv = PhotonView.Find(id);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

	}
}