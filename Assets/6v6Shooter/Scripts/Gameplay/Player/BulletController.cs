using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class BulletController : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    public float damage;

    private void OnEnable()
    {
        Invoke("Deactivate", lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player") && !collision.gameObject.GetComponent<PhotonView>().IsMine) {
            collision.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, damage);
            Debug.Log("=== I AM HITTING A PLAYER ====");
            this.gameObject.SetActive(false);
        } else if (collision.gameObject.CompareTag("TargetDummy")) {
            collision.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, damage);
            Debug.Log("=== I AM HITTING A TARGET DUMMY ====");
            this.gameObject.SetActive(false);
        } else if (collision.gameObject.CompareTag("Environment")) {
             Debug.Log("=== I AM HITTING A TARGET ENVIRONMENT ====");
            this.gameObject.SetActive(false);
        } else {
            Debug.Log("=== I AM HITTING NOTHING ====");
        }
    }
}
