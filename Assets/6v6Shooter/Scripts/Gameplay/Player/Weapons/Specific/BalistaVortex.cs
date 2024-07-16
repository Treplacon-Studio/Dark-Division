using System.Collections;
using System.Linq;

using UnityEngine;


public class BalistaVortex : MonoBehaviour
{
    [SerializeField] [Tooltip("Delay to wait until cogh/reload when reloading animation is started.")]
    private float coghDelay, reloadDelay;

    [SerializeField] [Tooltip("Time after cogh/reload starts to return to first position.")]
    private float coghHalf, reloadHalf;

    [SerializeField] [Tooltip("Delay to wait until cogh/reload when reloading animation is released.")]
    private float coghReleaseDelay, reloadReleaseDelay;

    private bool _lockReload;
    private bool _previousShootingLock;
    private Animator _anim;

    private PlayerNetworkController _pnc;

    private void Awake()
    {
        _pnc = PlayerUtils.FindComponentInParents<PlayerNetworkController>(gameObject);
    }
    
    private void Start()
    {
        _anim = GetComponent<Animator>();
        _pnc ??= PlayerUtils.FindComponentInParents<PlayerNetworkController>(gameObject);
        _previousShootingLock = ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController.shootingLock;
        
        if (_anim == null)
            Debug.LogError("Weapon animator not found.");
        if (ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController == null)
            Debug.LogError("Player animation controller not set in action manager.");
        
        SetProperFireRate();
    }

    void SetProperFireRate()
    {
        var animator = ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController.anim;
        var stateName = ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController.aimingLock ? "AN_FPS_Balista_CoghADS" : "AN_FPS_Balista_CoghHFR";
        
        if (animator == null)
            Debug.LogError("Animator is null.");
        
        var overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        AnimationClip clip = null;
        
        foreach (var binding in overrideController.animationClips)
        {
            if (binding.name == stateName)
            {
                clip = binding;
                break;
            }
        }

        if (clip == null)
        {
            Debug.LogError("Balista cogh clip not found or empty in character animation.");
            return;
        }

        var clipLength = clip.length + 0.05f;
        
        var weapon = GetComponent<Weapon>();
        var stats = weapon.Info().Stats();
        stats.FireRate = clipLength;
        weapon.Info().UpdateStats(stats);
    }

    private void Update()
    {
        if (_anim.GetBool("pCogh"))
            _anim.SetBool("pCogh", false);
        if (_anim.GetBool("pReload"))
            _anim.SetBool("pReload", false);

        _pnc ??= PlayerUtils.FindComponentInParents<PlayerNetworkController>(gameObject);
        
        //Reloading cogh
        if (ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController.reloadingLock)
        {
            if (!_lockReload)
            {
                StartCoroutine(RunReloadWithDelay());
                _lockReload = true;
            }
        }

        if (_lockReload && (!ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController.reloadingLock))
            _lockReload = false;

        //Shooting cogh
        if (_previousShootingLock && !ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController.shootingLock)
            StartCoroutine(RunCoghWithDelay());

        _previousShootingLock = ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController.shootingLock;
    }
    
    private IEnumerator RunReloadWithDelay()
    {
        if (reloadDelay > reloadReleaseDelay)
            Debug.LogError("Release time should be longer or equal reload.");

        //Start animation after cogh delay time
        yield return new WaitForSeconds(reloadDelay);
        _anim.SetBool("pReload", true);
        
        //Stop animation in cogh half-time
        yield return new WaitForSeconds(reloadHalf);
        _anim.speed = 0f;
        
        //Resume animation at coght release delay time
        yield return new WaitForSeconds(reloadReleaseDelay - (reloadDelay + reloadHalf));
        _anim.speed = 1f;
    }
    
    private IEnumerator RunCoghWithDelay()
    {
        if (coghDelay > coghReleaseDelay)
            Debug.LogError("Release time should be longer or equal cogh.");

        //Start animation after cogh delay time
        yield return new WaitForSeconds(coghDelay);
        _anim.SetBool("pCogh", true);
        
        //Stop animation in cogh half-time
        yield return new WaitForSeconds(coghHalf);
        _anim.speed = 0f;
        
        //Resume animation at coght release delay time
        yield return new WaitForSeconds(coghReleaseDelay - (coghDelay + coghHalf));
        _anim.speed = 1f;
    }
}