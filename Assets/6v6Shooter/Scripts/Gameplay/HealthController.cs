using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace _6v6Shooter.Scripts.Gameplay
{
    public class HealthController : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        Image healthBar;

        public float health;
        public float startHealth = 100;

        public bool targetDummy;

        void Start() {
            health = startHealth;
            healthBar.fillAmount = health / startHealth;
        }

        [PunRPC]
        public void TakeDamage(float damage) {
            health -= damage;
            healthBar.fillAmount = health / startHealth;    
        
            if (health <= 0f)
                Die();
        }

        void Die() {
            if (photonView.IsMine) {
                if (targetDummy is true)
                    photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
                else
                {
                    Team? team = TeamManager.GetTeam(PhotonNetwork.LocalPlayer);
                    Debug.Log("AddPointForTeam HIT");
                    Debug.Log(PhotonNetwork.LocalPlayer);
                    Debug.Log(team);
                    TeamDeathmatchManager.instance.GetComponent<PhotonView>().RPC("AddPointForTeam", RpcTarget.AllBuffered, team);
                    StartCoroutine(Respawn());
                }
            }
        }

        IEnumerator Respawn() {
            photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
            yield return new WaitForSeconds(1.0f);
        }

        [PunRPC]
        public void RegainHealth() {
            health = startHealth;
            healthBar.fillAmount = health / startHealth;
        }
    }
}
