using System.Collections;

using UnityEngine;


public class FnFive : MonoBehaviour
{
    [SerializeField] [Tooltip("Delay to wait until cogh when reloading animation is started.")]
    private float coghDelay;

    [SerializeField] [Tooltip("Time after cogh starts to return to first position.")]
    private float coghHalf;

    [SerializeField] [Tooltip("Delay to wait until cogh when reloading animation is released.")]
    private float coghReleaseDelay;

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
    }
    private void Update()
    {
        if (_anim.GetBool("pCoghReload"))
            _anim.SetBool("pCoghReload", false);

        //Reloading cogh
        _pnc ??= PlayerUtils.FindComponentInParents<PlayerNetworkController>(gameObject);
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
        if (!_previousShootingLock && ActionsManager.GetInstance(_pnc.GetInstanceID()).ComponentHolder.playerAnimationController.shootingLock)
            _anim.SetTrigger("pCogh");

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
        _anim.SetBool("pCoghReload", true);
        yield return new WaitForSeconds(coghHalf);
        _anim.speed = 0f;
        yield return new WaitForSeconds(coghReleaseDelay - (coghDelay + coghHalf));
        _anim.speed = 1f;
    }
}