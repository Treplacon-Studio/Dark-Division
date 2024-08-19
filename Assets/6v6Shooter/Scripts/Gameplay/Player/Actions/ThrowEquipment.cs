using System.Collections;
using System.Collections.Generic;
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

    [Header("SO references")]
    [SerializeField] private ThrowableSO lethalThrowableSO;
    [SerializeField] private ThrowableSO tacticalThrowableSO;

    private ThrowableSO currentThrowable;
    private GameObject lethalDummy;
    private GameObject tacticalDummy;
    private GameObject currentDummy;

    private bool canThrow = true;
    private bool isHolding = false;
    private bool isOverhandThrow = true;

    private void Start()
    {
        // Spawn a dummy for the lethal and tactical
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

            // Skip the currentDummy
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
            animator.SetTrigger("pFarThrow");
        }

        isHolding = false;
        ThrowAnimation(throwable);
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
        if (Input.GetKeyDown(ThrowStyleChange) && isHolding)
        {
            isOverhandThrow = !isOverhandThrow;
        }
    }

    private void ThrowAnimation(ThrowableSO throwable)
    {
        string triggerName = throwable.isThrowingKnife
            ? (isOverhandThrow ? "pThrowKnifeOverhand" : "pThrowKnifeUnderhand")
            : (isOverhandThrow ? "pFarThrow" : "pShortThrow");

        animator.SetTrigger(triggerName);
    }

    public void Throw()
    {
        canThrow = false;

        // Instantiate the throwable object (object pooling should be considered)
        GameObject throwableInstance = Instantiate(lethalThrowableSO.actualPrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody throwableRB = throwableInstance.GetComponent<Rigidbody>();

        // Apply throw force
        Vector3 forceDirection = transform.forward;
        Vector3 addForce = forceDirection * lethalThrowableSO.ThrowForce + transform.up * lethalThrowableSO.ThrowUpwardForce;
        throwableRB.AddForce(addForce, ForceMode.Impulse);

        StartCoroutine(ThrowCooldown(lethalThrowableSO.coolDownTime));
        if (currentDummy != null)
            currentDummy.SetActive(false);
        ReenableChildren();
    }

    private IEnumerator ThrowCooldown(float coolDownTime)
    {
        currentThrowable = null;
        yield return new WaitForSeconds(coolDownTime);
        canThrow = true;
    }
}
