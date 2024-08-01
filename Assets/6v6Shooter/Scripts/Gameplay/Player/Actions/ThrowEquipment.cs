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

    private void Start()
    {
        canThrow = true;
    }

    private void Update() 
    {
        // Check if ThrowBtn1 is pressed and held
        if (Input.GetKeyDown(ThrowBtn1) && canThrow && EquipmentAmmo1 > 0)
        {
            Animator.SetTrigger("pThrow"); // Trigger the throw animation
            Throw(ObjectToThrow1);
            EquipmentAmmo1--;
        }

        // Check if ThrowBtn2 is pressed and held
        if (Input.GetKeyDown(ThrowBtn2) && canThrow && EquipmentAmmo2 > 0)
        {
            Animator.SetTrigger("pThrow"); // Trigger the throw animation
            Throw(ObjectToThrow2);
            EquipmentAmmo2--;
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
}
