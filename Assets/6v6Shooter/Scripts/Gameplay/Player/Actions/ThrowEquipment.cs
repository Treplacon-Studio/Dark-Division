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
    private GameObject throwableHulster;

    private bool canThrow = true;
    private bool isHolding = false;
    private bool isFrozen = false;
    private bool isOverhandThrow = true;

    private void Start()
    {
        //Spawn a dummy for the lethal and tactical
        lethalDummy = Instantiate(lethalThrowableSO.dummyPrefab, throwableHulster.transform);
        lethalDummy.SetActive(false);
        tacticalDummy = Instantiate(tacticalThrowableSO.dummyPrefab, throwableHulster.transform);
        tacticalDummy.SetActive(false);
    }

    private void Update()
    {
        PrepareThrow(ThrowLethal, lethalThrowableSO);
        PrepareThrow(ThrowTactical, tacticalThrowableSO);
        ChangeThrowStyle();
    }

    private void PrepareThrow(KeyCode throwKey, ThrowableSO throwable)
    {
        if (Input.GetKey(throwKey))
        {
            if (canThrow && throwable.HasAmmo())
            {
                if (!isHolding)
                {
                    isHolding = true;
                    currentThrowable = throwable;

                    // Select the appropriate dummy based on the throwable type
                    currentDummy = throwable == lethalThrowableSO ? lethalDummy : tacticalDummy;

                    // Move the dummy to the weaponSocket
                    currentDummy.transform.SetParent(weaponSocket.transform);
                    currentDummy.transform.localPosition = Vector3.zero;
                    currentDummy.transform.localRotation = Quaternion.identity;
                    currentDummy.SetActive(true);

                    // Set the appropriate animation trigger
                    if (throwable.isThrowingKnife)
                    {
                        animator.SetTrigger("pPrepThrowKnife");
                    }
                    else
                    {
                        animator.SetTrigger("pPrepThrow");
                    }
                }

                if (Input.GetKeyUp(throwKey))
                {
                    ThrowAnimation(currentThrowable);
                    isFrozen = false;
                    isHolding = false;

                    currentDummy.SetActive(false);
                    currentDummy.transform.position = throwableHulster.transform.position;
                    currentDummy = null;
                }
            }
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
        {
            //Adjust trigger values Austyn
            string triggerName = throwable.isThrowingKnife
            ? (isOverhandThrow ? "pThrowKnifeOverhand" : "pThrowKnifeUnderhand")
            : (isOverhandThrow ? "pThrowOverhand" : "pThrowUnderhand");
            animator.SetTrigger(triggerName);
        }

        canThrow = false;
    }

    public void Throw()
    {
        //make sure to use object pooling
        GameObject throwableInstance = Instantiate(currentThrowable.actualPrefab, throwPoint.position, throwPoint.rotation);

        if (isOverhandThrow == true)
        {
            //Apply the old throwing (upward and forward force)
        }
        else
        {
            //Apply the old throwing (upward and forward force)
        }

        StartCoroutine(ThrowCooldown(currentThrowable.coolDownTime));
    }

    private IEnumerator ThrowCooldown(float coolDownTime)
    {
        currentThrowable = null;
        
        yield return new WaitForSeconds(coolDownTime);
        canThrow = true;
    }
}
