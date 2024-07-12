using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class DummyHealthController : MonoBehaviourPunCallbacks
{
    [SerializeField] private Image healthBar;
    public float health;
    public float startHealth = 100;

    void Start()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.fillAmount = health / startHealth;

        if (health <= 0f)
            Die();
    }

    void Die()
    {
        if (photonView != null && photonView.IsMine)
        {
            photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
        }
        else
        {
            RegainHealth();
        }
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
    }
}
