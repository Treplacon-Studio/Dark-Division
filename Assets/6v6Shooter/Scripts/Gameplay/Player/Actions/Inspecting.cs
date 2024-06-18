using System.Collections;
using _6v6Shooter.Scripts.Gameplay.Player.Animations;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Actions
{
    public class Inspecting : MonoBehaviour
    {
        private PlayerAnimationController _pac;
        
        private void Awake()
        {
            _pac = GetComponent<PlayerAnimationController>();
            ActionsManager.Instance.Inspecting = this;
        }
        
        public void Run()
        {
            if (Input.GetKeyDown(KeyCode.I) && !_pac.IsLocked())
            {
                StartCoroutine(LockTemporarily());
                
                var canInspect = true;
                foreach (var s in _pac.weaponActionsStates)
                {
                    if (_pac.InProgress(s, 0))
                    {
                        canInspect = false;
                        break;
                    }
                }

                if (canInspect)
                    _pac.PlayInspectAnimation();
            }
        }
        

        private IEnumerator LockTemporarily()
        {
            _pac.inspectingLock = true;
            yield return new WaitForSeconds(1f);
            _pac.inspectingLock = false;
        }
    }
}
