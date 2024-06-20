using System.Collections;
using _6v6Shooter.Scripts.Gameplay.Player.Animations;
using UnityEditor.Animations;
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
            
            var ac = _pac.anim.runtimeAnimatorController as AnimatorController;
            var animtime = 0f;
            if (ac != null)
            {
                var stateMachine = ac.layers[1].stateMachine;
                foreach (var state in stateMachine.states)
                {
                    if (state.state.name == "AN_FPS_Inspect")
                    {
                        var clip = state.state.motion as AnimationClip;
                        if (clip == null) continue;
                        animtime = clip.length + 0.05f;
                        break;
                    }
                }
            }
                
            yield return new WaitForSeconds(animtime);
            _pac.inspectingLock = false;
        }
    }
}
