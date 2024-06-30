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
        public bool isDead = false;

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
                isDead = true;
                if (targetDummy is true)
                    photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
                else
                {
                    if (!isDead)
                    {
                        TeamDeathmatchManager.instance.GetComponent<PhotonView>().RPC("AddPointForTeam", RpcTarget.AllBuffered);
                        StartCoroutine(Respawn());
                    }
                }
            }
        }

        IEnumerator Respawn() {
            yield return new WaitForSeconds(1.0f);
            photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void RegainHealth() {
            health = startHealth;
            healthBar.fillAmount = health / startHealth;
        }
    }
}
