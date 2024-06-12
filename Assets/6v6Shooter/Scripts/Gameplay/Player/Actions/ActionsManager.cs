using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class ActionsManager : MonoBehaviour
    {
        [HideInInspector] public Reloading reloading;
        [HideInInspector] public Inspecting inspecting;
        [HideInInspector] public Aiming aiming;
        [HideInInspector] public Shooting shooting;
        
        [HideInInspector] public Walking walking;
        [HideInInspector] public Sprinting sprinting;
        [HideInInspector] public Jumping jumping;
        [HideInInspector] public Crouching crouching;
        

        private void Start()
        {
            reloading = GetComponent<Reloading>();
            inspecting = GetComponent<Inspecting>();
            aiming = GetComponent<Aiming>();
            shooting = GetComponent<Shooting>();
            
            walking = GetComponent<Walking>();
            sprinting = GetComponent<Sprinting>();
            jumping = GetComponent<Jumping>();
            crouching = GetComponent<Crouching>();
        }
    }
}
