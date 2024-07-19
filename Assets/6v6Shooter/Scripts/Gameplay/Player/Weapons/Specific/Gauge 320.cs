using System.Collections;
using System.Linq;

using UnityEngine;


public class Gauge320 : MonoBehaviour
{
    [SerializeField] [Tooltip("Time that weapon cogh has to wait for character cogh.")]
    private float coghDelay;
    
    [SerializeField] [Tooltip("Left hand as starting holder for shell")]
    private GameObject leftHand;
    
    [SerializeField] [Tooltip("Shell to be set in slot.")]
    private GameObject visualShell;
    
    [SerializeField]  [Tooltip("Slot for shell.")]
    private GameObject shellSlot;
    
    [SerializeField]  [Tooltip("Time in animation to un-parent shell from hand and put in slot.")]
    private float shellSetTime;
    
    private bool _previousShootingLock;
    private bool _lockReload;
    private Animator _anim;
    private PlayerNetworkController _pnc;
    
    private static readonly int PReload = Animator.StringToHash("pReload");
    private static readonly int PCogh = Animator.StringToHash("pCogh");

    private void Awake()
    {
        _pnc = PlayerUtils.FindComponentInParents<PlayerNetworkController>(gameObject);
        _anim = GetComponent<Animator>();
    }

    private void Start()
    {
        _pnc ??= PlayerUtils.FindComponentInParents<PlayerNetworkController>(gameObject);
        SetProperFireRate();
    }

    private void Update()
    {
        //Reset reloading parameter
        if (_anim.GetBool(PReload))
            _anim.SetBool(PReload, false);
        if (_anim.GetBool(PCogh))
            _anim.SetBool(PCogh, false);

        //Update network controller if was not previously set
        _pnc ??= PlayerUtils.FindComponentInParents<PlayerNetworkController>(gameObject);
        
        //Shooting cogh
        var ammoIn = ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.bulletPoolingManager
            .GetAmmoPrimary() > 0;
        if (_previousShootingLock && ammoIn && !ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder
                .playerAnimationController.shootingLock)
            StartCoroutine(CoghWithDelay());
        else
            _anim.SetBool(PCogh, false);

        //Unlock critical section
        _previousShootingLock = ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController.shootingLock;
        
        //Start reloading stuff
        if (ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController.reloadingLock)
        {
            if (!_lockReload)
            {
                StartCoroutine(ReloadAnimation());
                _lockReload = true;
            }
        }

        //Unlock critical section
        if (_lockReload && !ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController.reloadingLock)
            _lockReload = false;
    }
    
    void SetProperFireRate()
    {
        var animator = ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController.anim;
        var stateName = ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController.aimingLock ? "AN_FPS_Gauge320_CoghADS" : "AN_FPS_Gauge320_CoghHFR";
        
        if (animator == null)
        {
            Debug.LogError("Animator is null.");
            return;
        }
        
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
            Debug.LogError("Gauge 320 clip not found or empty in character animation.");
            return;
        }

        var clipLength = clip.length + 0.3f;
        
        var weapon = GetComponent<Weapon>();
        var stats = weapon.Info().Stats();
        stats.FireRate = clipLength;
        weapon.Info().UpdateStats(stats);
    }

    private IEnumerator CoghWithDelay()
    {
        yield return new WaitForSeconds(coghDelay);
        _anim.SetBool(PCogh, true);
    }
    
    private IEnumerator ReloadAnimation()
    {
        //Show shells in player left hand
        visualShell.SetActive(true);
        
        //Start animation
        _anim.SetBool(PReload, true);

        //Hide shells when put in slot
        yield return new WaitForSeconds(shellSetTime);
        visualShell.SetActive(false);
    }
}