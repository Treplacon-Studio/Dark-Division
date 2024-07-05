using UnityEngine;

public class DummyEnemy : MonoBehaviour, IEnemy
{
    [Header("Define enemy type")]
    [SerializeField] private EnemyType enemyType;
    private Vector3 initialPosition;

    [Header("DummyOnStick values")]
    private bool isDestroyed = false;
    [SerializeField] private float dummyOnStickHealth = 100;
    [SerializeField] private GameObject brokenDummy;

    [Header("DummyHitSheet values")]
    [SerializeField] private GameObject hitDecal;
    [SerializeField] private Transform decalParent;
    private bool isLerping = false;
    private Vector3 targetPosition;
    private float lerpDuration = 2f;
    private float lerpStartTime;

    [Header("DummyPatroler values")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float sinWaveMagnitude = 1f;
   

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        switch (enemyType)
        {
            case EnemyType.DummyOnStick:
                DummyOnStickLogic();
                break;

            case EnemyType.DummyHitSheet:
                DummyHitSheetLogic();
                break;

            case EnemyType.DummyPatroler:
                DummyPatrolerLogic();
                break;
        }
    }

    private void DummyOnStickLogic()
    {
        if (dummyOnStickHealth <= 0 && !isDestroyed)
        {
            isDestroyed = true; // Set the flag to true to prevent re-execution

            // Instantiate the broken dummy
            GameObject brokenInstance = Instantiate(brokenDummy, transform.position, transform.rotation);

            // Get all child objects of brokenInstance
            foreach (Transform child in brokenInstance.transform)
            {
                // Apply a random force to each child object
                Rigidbody childRigidbody = child.GetComponent<Rigidbody>();
                if (childRigidbody != null)
                {
                    float forceMagnitude = Random.Range(1f, 10);
                    Vector3 randomForce = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f)).normalized * forceMagnitude;
                    childRigidbody.AddForce(randomForce, ForceMode.Impulse);
                }
                else
                {
                    Debug.Log("No RB");
                }
            }

            // Destroy the original game object
            Destroy(gameObject);
        }
    }

    private void DummyHitSheetLogic()
    {
        // When certain button is pressed, initiate the lerping process
        if (Input.GetKeyDown(KeyCode.L))
        {
            isLerping = true;
            targetPosition = transform.position + Vector3.left * 10f;
            lerpStartTime = Time.time;
        }

        // Perform the lerping if isLerping is true
        if (isLerping)
        {
            // Calculate how far along the lerping process we are
            float lerpTimeElapsed = Time.time - lerpStartTime;
            float lerpPercentage = lerpTimeElapsed / lerpDuration;

            // Apply the lerp to move the object smoothly
            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpPercentage);

            // Check if lerping is complete
            if (lerpPercentage >= 1f)
                isLerping = false; // Stop lerping
        }
    }

    private void DummyPatrolerLogic()
    {
        float horizontalMovement = Mathf.Sin(Time.time * movementSpeed) * sinWaveMagnitude;
        transform.position = initialPosition + new Vector3(0f, 0f, horizontalMovement);
    }
}
