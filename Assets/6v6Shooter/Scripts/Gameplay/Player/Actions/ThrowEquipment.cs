using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowEquipment : MonoBehaviour
{
    [Header("References")]
    public Transform Camera;
    public GameObject ObjectToThrow1;
    public GameObject ObjectToThrow2;
    public Animator Animator;  // Add reference to the Animator
     public GameObject Weapon;

    [Header("Settings")]
    public int EquipmentAmmo1;
    public int EquipmentAmmo2;
    public float Cooldown;

    [Header("Throwing")]
    public KeyCode ThrowBtn1 = KeyCode.G;
    public KeyCode ThrowBtn2 = KeyCode.H;
    public float ThrowForce;
    public float ThrowUpwardForce;

    private bool canThrow;
     private bool isHolding;

    private void Start()
    {
        canThrow = true;
    }

    private void Update()
    {
        // Start throwing process
        if (Input.GetKeyDown(ThrowBtn1) && canThrow && EquipmentAmmo1 > 0)
        {
            Animator.SetTrigger("pThrow"); // Trigger the prep throw animation
            isHolding = true;
            EquipmentAmmo1--;

            if (Weapon != null)
            {
                Weapon.SetActive(false); // Disable the weapon during throw
            }
        }

        // Hold the throw
        if (Input.GetKey(ThrowBtn1) && isHolding)
        {
            Animator.SetBool("isHolding", true); // Set the bool to transition to the hold animation
        }

        // Release the throw
        if (Input.GetKeyUp(ThrowBtn1) && isHolding)
        {
            Animator.SetBool("isHolding", false); // Unset the hold bool
            Animator.SetTrigger("pReleaseThrow"); // Trigger the release animation
            Throw(ObjectToThrow1);
            isHolding = false;

            Invoke(nameof(ReEnableWeapon), 0.5f); // Re-enable the weapon after a short delay (adjust as needed)
        }
    }

    private void Throw(GameObject objectToThrow)
    {
        canThrow = false;
        GameObject projectile = Instantiate(objectToThrow, Camera.position, Camera.rotation);
        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();

        // Calculate force direction based on camera's forward direction
        Vector3 forceDirection = Camera.transform.forward;

        // Add force to the projectile
        Vector3 addForce = forceDirection * ThrowForce + Camera.transform.up * ThrowUpwardForce;
        projectileRB.AddForce(addForce, ForceMode.Impulse);

        Invoke(nameof(ResetThrow), Cooldown);
    }

    private void ResetThrow()
    {
        canThrow = true;
    }

     private void ReEnableWeapon()
    {
        if (Weapon != null)
        {
            Weapon.SetActive(true); // Re-enable the weapon after the throw
        }
    }
}
