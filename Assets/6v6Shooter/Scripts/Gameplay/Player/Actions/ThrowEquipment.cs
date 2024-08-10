using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowEquipment : MonoBehaviour
{
    [Header("References")]
    public Transform ThrowPoint;
    public GameObject ObjectToThrow1;
    public GameObject ObjectToThrow2;
    public GameObject StaticObjectToThrow1;
    public GameObject StaticObjectToThrow2; 
    public Animator Animator;
    [SerializeField] public GameObject Weapon;
    [SerializeField] public Transform HandTransform;

    [Header("Settings")]
    public int EquipmentAmmo1;
    public int EquipmentAmmo2;
    public float Cooldown;
    public float ThrowDelay = 0.3f;

    [Header("Throwing")]
    public KeyCode ThrowBtn1 = KeyCode.G;
    public KeyCode ThrowBtn2 = KeyCode.H;
    public KeyCode ShortThrowBtn = KeyCode.F;

    public float ThrowForce = 16;
    public float ThrowUpwardForce = 15;

    private bool canThrow;
    private bool isHolding;
    private bool isFrozen;
    private GameObject activeGrenade;

    private void Start()
    {
        canThrow = true;
    }

    private void Update()
{
    if (Input.GetKeyDown(ThrowBtn1) && canThrow && EquipmentAmmo1 > 0)
    {
        // Check if it's a knife or regular throw
        if (IsKnifeEquipped())
        {
            Animator.SetTrigger("pKnifeThrow"); // Set the knife throw trigger
            Invoke(nameof(FreezeAnimation), 0.1f); // Freeze only for knife throw
        }
        else
        {
            Animator.SetTrigger("pThrow"); // Set the regular throw trigger
        }

        isHolding = true;
        EquipmentAmmo1--;

        if (Weapon != null)
        {
            DisableWeaponChildren();
            EquipGrenade(StaticObjectToThrow1);
        }
    }

    if (Input.GetKeyUp(ThrowBtn1) && isHolding)
    {
        if (IsKnifeEquipped())
        {
            UnfreezeAnimation(); // Unfreeze only for knife throw
            Animator.SetTrigger("pKnifeThrow"); // Finish the knife throw animation
        }
        else
        {
            Animator.SetTrigger("pFarThrow"); // Transition to the second animation for regular throw
        }

        Invoke(nameof(ThrowPrimary), ThrowDelay);
        isHolding = false;
    }

    if (Input.GetKeyDown(ThrowBtn2) && canThrow && EquipmentAmmo2 > 0)
    {
        Animator.SetTrigger("pThrow"); // Regular throw, no freezing needed
        isHolding = true;
        EquipmentAmmo2--;

        if (Weapon != null)
        {
            DisableWeaponChildren();
            EquipGrenade(StaticObjectToThrow2);
        }
    }

    if (Input.GetKeyUp(ThrowBtn2) && isHolding)
    {
        Animator.SetTrigger("pFarThrow"); // Transition to the second animation for regular throw
        Invoke(nameof(ThrowSecondary), ThrowDelay);
        isHolding = false;
    }
}

    private void ThrowPrimary()
    {
        PerformThrow(ObjectToThrow1);
    }

    private void ThrowSecondary()
    {
        PerformThrow(ObjectToThrow2);
    }

    private void PerformThrow(GameObject throwable)
    {
        Throw(throwable);
        
        if (activeGrenade != null)
        {
            activeGrenade = null;
        }

        Invoke(nameof(ReEnableWeaponChildren), 0.5f);
    }

    private void Throw(GameObject objectToThrow)
    {
        canThrow = false;
        GameObject projectile = Instantiate(objectToThrow, ThrowPoint.position, ThrowPoint.rotation);
        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = ThrowPoint.transform.forward;
        Vector3 addForce = forceDirection * ThrowForce + ThrowPoint.transform.up * ThrowUpwardForce;
        projectileRB.AddForce(addForce, ForceMode.Impulse);

        if (activeGrenade != null)
        {
            Destroy(activeGrenade);
        }

        Invoke(nameof(ResetThrow), Cooldown);
    }

    private void ResetThrow()
    {
        canThrow = true;
    }

    private void DisableWeaponChildren()
    {
        foreach (Transform child in Weapon.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void ReEnableWeaponChildren()
    {
        foreach (Transform child in Weapon.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void EquipGrenade(GameObject grenadePrefab)
    {
        if (grenadePrefab != null)
        {
            activeGrenade = Instantiate(grenadePrefab, HandTransform);
        }
    }

    public void FreezeAnimation()
{
    if (isHolding && !isFrozen && IsKnifeEquipped())
    {
        Animator.speed = 0; // Pause the animation only for knife throw
        isFrozen = true;
    }
}

public void UnfreezeAnimation()
{
    if (IsKnifeEquipped())
    {
        Animator.speed = 1; // Resume the animation only for knife throw
        isFrozen = false;
    }
}

    private bool IsKnifeEquipped()
    {
        return ObjectToThrow1 != null && ObjectToThrow1.name == "ThrowKnifePrefab";
    }
}
