using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowEquipment : MonoBehaviour
{
    [Header("References")]
    public Transform Camera;
    public GameObject ObjectToThrow1;
    public GameObject ObjectToThrow2;
    public GameObject StaticObjectToThrow1; // Reference to static grenade prefab
    public GameObject StaticObjectToThrow2; // Reference to static grenade prefab
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

    public float ThrowForce;
    public float ThrowUpwardForce;

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
            Animator.SetTrigger("pThrow");
            isHolding = true;
            isFrozen = false;
            EquipmentAmmo1--;

            if (Weapon != null)
            {
                DisableWeaponChildren();
                EquipGrenade(StaticObjectToThrow1); // Use static prefab
            }
        }

        if (Input.GetKeyUp(ThrowBtn1) && isHolding)
        {
            Animator.SetTrigger("pFarThrow");
            Invoke(nameof(ThrowPrimary), ThrowDelay);
            isHolding = false;
            isFrozen = false;
        }

        if (Input.GetKeyDown(ThrowBtn2) && canThrow && EquipmentAmmo2 > 0)
        {
            Animator.SetTrigger("pThrow");
            isHolding = true;
            isFrozen = false;
            EquipmentAmmo2--;

            if (Weapon != null)
            {
                DisableWeaponChildren();
                EquipGrenade(StaticObjectToThrow2); // Use static prefab
            }
        }

        if (Input.GetKeyUp(ThrowBtn2) && isHolding)
        {
            Animator.SetTrigger("pFarThrow");
            Invoke(nameof(ThrowSecondary), ThrowDelay);
            isHolding = false;
            isFrozen = false;
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
        Invoke(nameof(ReEnableWeaponChildren), 0.5f);
    }

    private void Throw(GameObject objectToThrow)
    {
        canThrow = false;
        GameObject projectile = Instantiate(objectToThrow, Camera.position, Camera.rotation);
        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = Camera.transform.forward;
        Vector3 addForce = forceDirection * ThrowForce + Camera.transform.up * ThrowUpwardForce;
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
            activeGrenade = Instantiate(grenadePrefab, HandTransform.position, HandTransform.rotation, HandTransform);
            activeGrenade.transform.localScale = Vector3.one; // Ensure the grenade is scaled correctly
        }
    }

    public void FreezeAnimation()
    {
        if (isHolding && !isFrozen)
        {
            Animator.speed = 0;
            isFrozen = true;
        }
    }

    public void UnfreezeAnimation()
    {
        Animator.speed = 1;
    }
}
