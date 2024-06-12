using System.Collections;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Inspecting : MonoBehaviour
    {
        [SerializeField] [Tooltip("Player animation controller.")]
        private PlayerAnimationController pac;
        
        
        public void Run()
        {
            if (Input.GetKeyDown(KeyCode.I) && !pac.IsLocked())
            {
                StartCoroutine(LockTemporarily());
                
                var canInspect = true;
                foreach (var s in pac.weaponActionsStates)
                {
                    if (pac.InProgress(s, 1))
                    {
                        canInspect = false;
                        break;
                    }
                }

                if (canInspect)
                    pac.PlayInspectAnimation();
            }
        }
        

        private IEnumerator LockTemporarily()
        {
            pac.inspectingLock = true;
            yield return new WaitForSeconds(1f);
            pac.inspectingLock = false;
        }
    }
}
