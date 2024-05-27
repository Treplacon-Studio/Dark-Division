using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class BulletController : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 4f;
    public float damage;
    public float raycastInterval = 0.1f; // Interval for the repeating raycast
    public float raycastLength = 1f; // Length of the raycast

    private Coroutine raycastCoroutine;
    private PhotonView pv;

    private void Awake() {
        pv = GetComponent<PhotonView>();
    }

    private void OnEnable() {
        raycastCoroutine = StartCoroutine(SpawnRaycast());
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
        if (raycastCoroutine != null)
        {
            StopCoroutine(raycastCoroutine);
        }
        CancelInvoke();
    }

    private IEnumerator SpawnRaycast()
    {
        while (true)
        {
            RaycastHit hit;
            Vector3 forward = transform.TransformDirection(Vector3.forward) * raycastLength;
            Debug.DrawRay(transform.position, forward, Color.red, raycastInterval);
            if (Physics.Raycast(transform.position, transform.forward, out hit, raycastLength))
            {
                if (hit.collider.CompareTag("Player") && !hit.collider.GetComponent<PhotonView>().IsMine)
                {
                    hit.collider.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, damage);
                    this.gameObject.SetActive(false);
                    Debug.Log("=== HIT PLAYER ===");
                } else if (hit.collider.CompareTag("TargetDummy")) {
                    hit.collider.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, damage);
                    Debug.Log("=== I AM HITTING A TARGET DUMMY ====");
                    this.gameObject.SetActive(false);
                } else if (hit.collider.CompareTag("Environment")) {
                    Debug.Log("=== I AM HITTING A TARGET ENVIRONMENT ====");
                    this.gameObject.SetActive(false);
                }
            }
            yield return new WaitForSeconds(raycastInterval);
        }
    }
}
