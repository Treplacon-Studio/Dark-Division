using System.Collections;

using UnityEngine;

/// <summary>
/// DSR50 specific stuff class.
/// </summary>
public class Dsr50 : MonoBehaviour
{
    [SerializeField] private float coghDelay;
    [SerializeField] private float coghHalf;
    [SerializeField] private float coghReleaseDelay;

    [SerializeField] private AnimationClip adsCoghClip;
    [SerializeField] private AnimationClip hfrCoghClip;
    
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
        var clip = ActionsManager.GetInstance(_pnc.GetInstanceID())
            .ComponentHolder.playerAnimationController.aimingLock ? adsCoghClip : hfrCoghClip;
        
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

        _pnc ??= PlayerUtils.FindComponentInParents<PlayerNetworkController>(gameObject);
        
        //Reloading cogh
        if (ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController.reloadingLock)
        {
            if (!_lockReload)
            {
                StartCoroutine(RunCoghWithDelay());
                _lockReload = true;
            }
        }

        if (_lockReload && (!ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController.reloadingLock))
            _lockReload = false;

        //Shooting cogh
        if (_previousShootingLock && !ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController.shootingLock)
            _anim.SetBool("pCogh", true);
        else
            _anim.SetBool("pCogh", false);

        _previousShootingLock = ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController.shootingLock;
    }

    //Cogh animations starts with delay, stops when hand not holding cogh and resume when hold again
    private IEnumerator RunCoghWithDelay()
    {
        if (coghDelay > coghReleaseDelay)
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