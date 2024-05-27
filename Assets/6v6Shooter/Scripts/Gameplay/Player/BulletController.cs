using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class BulletController : MonoBehaviour
{
    public float speed = 20f;
    public float damage;
    public float rayLengthFactor = 1f; // Factor to multiply with bullet speed to determine ray length
    public LayerMask hitLayers;
    public float fadeDuration = 2f; // Fade duration in seconds

    private LineRenderer lineRenderer;
    private Coroutine raycastCoroutine;
    private PhotonView pv;

    private void Awake()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        lineRenderer.positionCount = 2;
        pv = GetComponent<PhotonView>();
    }

    void Start()
    {
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.SetPosition(0, transform.position);
        StartCoroutine(DestroyAfterDelay(3f));
    }

    void Update()
    {
        // Move the bullet forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Shoot ray towards the center of the screen
        ShootRay();
    }

    void ShootRay()
    {
        RaycastHit hit;

        // Calculate direction towards the center of the screen
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        Vector3 shootDirection = ray.direction.normalized;

        float currentRayLength = speed * rayLengthFactor;
        //Debug.DrawRay(transform.position, shootDirection * currentRayLength, Color.red);

        if (Physics.Raycast(transform.position, shootDirection, out hit, currentRayLength, hitLayers))
        {
            DrawPath(hit.point);
            Debug.Log("Hit an object: " + hit.collider.gameObject.name);

            HealthController objectsHealth = hit.collider.GetComponent<HealthController>();
            if (objectsHealth != null)
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

            StartCoroutine(DestroyAfterDelay(0.01f));
        }
        else
        {
            DrawPath(transform.position + shootDirection * currentRayLength);
            StartCoroutine(FadeOut());
        }
    }

    void DrawPath(Vector3 endPosition)
    {
        Vector3 currentPosition = transform.position;
        Vector3 nextPosition = Vector3.MoveTowards(currentPosition, endPosition, speed * Time.deltaTime);
        lineRenderer.SetPosition(1, nextPosition);
    }

    public void SetDirection(Vector3 direction, float bulletSpeed)
    {
        speed = bulletSpeed;
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color startColor = lineRenderer.startColor;
        Color endColor = lineRenderer.endColor;
        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            endColor.a = Mathf.Lerp(1f, 0f, t);
            lineRenderer.endColor = Color.white;
            lineRenderer.startColor = endColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
