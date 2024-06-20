using System.Collections;
using System.Linq;
using _6v6Shooter.Scripts.Gameplay.Player.Actions;
using _6v6Shooter.Scripts.Gameplay.Player.Animations;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

namespace _6v6Shooter.Scripts.Gameplay.Player.Weapons.Specific
{
    public class Dsr50 : MonoBehaviour
    {
        [SerializeField] [Tooltip("Delay to wait until cogh when reloading animation is started.")]
        private float coghDelay;
        
        [SerializeField] [Tooltip("Time after cogh starts to return to first position.")]
        private float coghHalf;
        
        [SerializeField] [Tooltip("Delay to wait until cogh when reloading animation is released.")]
        private float coghReleaseDelay;
        
        private bool _lockReload;
        private bool _previousShootingLock;
        private PlayerAnimationController _pac;
        private Animator _anim;

        private void Start()
        {
            _pac = ActionsManager.Instance.Pac;
            _anim = GetComponent<Animator>();
            _previousShootingLock = _pac.shootingLock;
            if(_anim == null)
                Debug.LogError("Weapon animator not found.");
            if(_pac == null)
                Debug.LogError("Player animation controller not set in action manager.");
            SetProperFireRate();
        }

        void SetProperFireRate()
        {
            //Get character cogh animation length
            var sm = (ActionsManager.Instance.Pac.anim.runtimeAnimatorController as AnimatorController)?.layers[1].stateMachine;
            var stateName = ActionsManager.Instance.Pac.aimingLock ? "AN_FPS_CoghADS" : "AN_FPS_CoghHFR";
            var clipLength = (from s in sm.states where s.state.name == stateName select s.state.motion)
                .OfType<AnimationClip>().Select(clip => clip.length + 0.05f).FirstOrDefault();

            if (clipLength <= 0)
            {
                Debug.LogError("DSR-50 cogh clip not found or empty in character animation.");
                return;
            }

            //Update weapon fire rate
            var stats = GetComponent<Weapon>().Info().Stats();
            stats.FireRate = clipLength;
            GetComponent<Weapon>().Info().UpdateStats(stats);
        }
        
        private void Update()
        {
            if(_anim.GetBool("pCogh"))
                _anim.SetBool("pCogh", false);
            
            //Reloading cogh
            if (_pac.reloadingLock)
            {
                if (!_lockReload)
                {
                    StartCoroutine(RunCoghWithDelay());
                    _lockReload = true;
                }
            }
            
            if (_lockReload && (!_pac.reloadingLock))
                _lockReload = false;
            
            //Shooting cogh
            if (_previousShootingLock && !_pac.shootingLock)
                _anim.SetBool("pCogh", true);
            else
                _anim.SetBool("pCogh", false);
            
            _previousShootingLock = _pac.shootingLock;
        }

        //Cogh animations starts with delay, stops when hand not holding cogh and resume when hold again
        private IEnumerator RunCoghWithDelay()
        {
            if (coghDelay >  coghReleaseDelay)
            {
                Debug.LogError("Release time should be longer or equal cogh.");
                yield return null;
            }

            yield return new WaitForSeconds(coghDelay);
            _anim.SetBool("pCogh", true);
            yield return new WaitForSeconds(coghHalf);
            _anim.speed = 0f;
            yield return new WaitForSeconds(coghReleaseDelay - (coghDelay + coghHalf));
            _anim.speed = 1f;
        }
    }
}
