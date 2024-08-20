using System.Collections;
using UnityEngine;

public class ThrowEquipment : MonoBehaviour
{
    public enum ThrowableType { Primary, Secondary }

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject weaponSocket;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwCooldown;

    [Header("Throwing")]
    [SerializeField] private KeyCode ThrowLethal = KeyCode.G;
    [SerializeField] private KeyCode ThrowTactical = KeyCode.H;
    [SerializeField] private KeyCode ThrowStyleChange = KeyCode.F;

    [Header("SO References")]
    [SerializeField] private ThrowableSO lethalThrowableSO;
    [SerializeField] private ThrowableSO tacticalThrowableSO;

    private ThrowableSO currentThrowable;
    private GameObject lethalDummy;
    private GameObject tacticalDummy;
    private GameObject currentDummy;

    private bool canThrow = true;
    private bool isHolding = false;
    private bool throwStyleFar = true;

    private void Start()
    {
        // Spawn a dummy for the lethal and tactical throwables
        lethalDummy = Instantiate(lethalThrowableSO.dummyPrefab);
        lethalDummy.SetActive(false);
        tacticalDummy = Instantiate(tacticalThrowableSO.dummyPrefab);
        tacticalDummy.SetActive(false);
    }

    private void Update()
    {
        HandleThrowing(ThrowLethal, lethalThrowableSO);
        HandleThrowing(ThrowTactical, tacticalThrowableSO);
        ChangeThrowStyle();
    }

    private void HandleThrowing(KeyCode throwKey, ThrowableSO throwable)
    {
        if (Input.GetKeyDown(throwKey) && canThrow && throwable.HasAmmo())
        {
            currentThrowable = throwable;

            // Set the appropriate animation trigger
            if (throwable.isThrowingKnife)
            {
                animator.SetTrigger("pPrepThrowKnife");
            }
            else
            {
                animator.SetTrigger("pThrow");
            }

            isHolding = true;

            // Select the appropriate dummy based on the throwable type
            currentDummy = throwable == lethalThrowableSO ? lethalDummy : tacticalDummy;

            // Move the dummy to the weaponSocket
            currentDummy.transform.SetParent(weaponSocket.transform);
            currentDummy.transform.localPosition = Vector3.zero;
            currentDummy.transform.localRotation = Quaternion.identity;
            currentDummy.transform.localScale = Vector3.one;
            currentDummy.SetActive(true);

            // Disable all other children in weaponSocket except for currentDummy
            for (int i = 0; i < weaponSocket.transform.childCount; i++)
            {
                Transform child = weaponSocket.transform.GetChild(i);

                if (child != currentDummy.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        // Handle the key release for throwing the object
        if (Input.GetKeyUp(throwKey) && isHolding)
        {
            if (throwable.isThrowingKnife)
            {
                animator.SetTrigger("pKnifeThrow");
            }
            else
            {
                animator.SetTrigger(throwStyleFar ? "pFarThrow" : "pShortThrow");
            }

            isHolding = false;
        }
    }

    public void ReenableChildren()
    {
        for (int i = 0; i < weaponSocket.transform.childCount; i++)
        {
            Transform child = weaponSocket.transform.GetChild(i);
            child.gameObject.SetActive(true);
        }
    }

    private void ChangeThrowStyle()
    {
        if (Input.GetKeyDown(ThrowStyleChange))
        {
            throwStyleFar = !throwStyleFar;
            Debug.Log("Throw style changed to " + (throwStyleFar ? "Far" : "Short") + ".");
        }
    }

    public void Throw()
    {
        canThrow = false;

        if (currentThrowable == null)
        {
            Debug.LogWarning("No throwable object is set.");
            return;
        }

        // Instantiate the throwable object (object pooling should be considered)
        GameObject throwableInstance = Instantiate(currentThrowable.actualPrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody throwableRB = throwableInstance.GetComponent<Rigidbody>();

        // Calculate the force based on the throw style
        Vector3 forceDirection = transform.forward;
        float forceMultiplier = throwStyleFar ? 1.0f : 0.5f; // Halve the force if throw style is short
        Vector3 addForce = forceDirection * currentThrowable.ThrowForce * forceMultiplier +
                           transform.up * currentThrowable.ThrowUpwardForce * forceMultiplier;

        throwableRB.AddForce(addForce, ForceMode.Impulse);

        StartCoroutine(ThrowCooldown(currentThrowable.coolDownTime));
        
        if (currentDummy != null)
            currentDummy.SetActive(false);

        StartCoroutine(DelayedReenableChildren());
    }

    // Slight delay to let the animation finish before reenabling
    private IEnumerator DelayedReenableChildren()
    {
        yield return new WaitForSeconds(0.2f);
        ReenableChildren();
    }

    private IEnumerator ThrowCooldown(float coolDownTime)
    {
        currentThrowable = null;
        yield return new WaitForSeconds(coolDownTime);
        canThrow = true;
    }
}
