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
            if (photonView.IsMine && targetDummy is true) {
                if (targetDummy is true)
                    photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
                else
                    StartCoroutine(Respawn());
            }
        }

        IEnumerator Respawn() {
            GameObject respawnText = GameObject.Find("RespawnText");

            float respawnTime = 8.0f;
            while (respawnTime > 0.0f) {
                yield return new WaitForSeconds(1.0f);
                respawnTime -= 1.0f;
                transform.GetComponent<PlayerMotor>().enabled = false;
                respawnText.GetComponent<Text>().text = "You are killed. Respawning at: " + respawnTime.ToString(".00");
            }

            respawnText.GetComponent<Text>().text = "";

            int randomPoint = Random.Range(-20, 20);
            transform.position = new Vector3(randomPoint, 0, randomPoint);
            transform.GetComponent<PlayerMotor>().enabled = true;

            photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void RegainHealth() {
            health = startHealth;
            healthBar.fillAmount = health / startHealth;
        }
    }
}
