using System.Collections;
using System.Linq;

using UnityEngine;


public class Stoeger22 : MonoBehaviour
{
    [SerializeField] [Tooltip("Left hand as starting holder for shells")]
    private GameObject leftHand;
    
    [SerializeField] [Tooltip("Two shells to be set in slots, 0 goes first.")]
    private GameObject[] visualShells = new GameObject[2];
    
    [SerializeField]  [Tooltip("Slots for shells, 0 is set at first.")]
    private GameObject[] shellsSlots = new GameObject[2];
    
    [SerializeField]  [Tooltip("Times in animations to un-parent shell from hand and put in slot.")]
    private float[] shellSetTimes = new float[2];
    
    private bool _lockReload;
    private Animator _anim;
    private PlayerNetworkController _pnc;
    
    private static readonly int PReload = Animator.StringToHash("pReload");

    private void Awake()
    {
        _pnc = PlayerUtils.FindComponentInParents<PlayerNetworkController>(gameObject);
        _anim = GetComponent<Animator>();
    }
    
    private void Update()
    {
        //Reset reloading parameter
        if (_anim.GetBool(PReload))
            _anim.SetBool(PReload, false);

        //Update network controller if was not previously set
        _pnc ??= PlayerUtils.FindComponentInParents<PlayerNetworkController>(gameObject);
        
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

    private IEnumerator ReloadAnimation()
    {
        if(shellSetTimes[0] > shellSetTimes[1])
            Debug.LogError("First shell put time cannot be higher than second.");
        
        //Show shells in player left hand
        foreach (var shell in visualShells)
            shell.SetActive(true);
        
        //Start animation
        _anim.SetBool(PReload, true);

        //Hide shells when put in slot
        yield return new WaitForSeconds(shellSetTimes[0]);
        visualShells[0].SetActive(false);
        yield return new WaitForSeconds(shellSetTimes[1]-shellSetTimes[0]);
        visualShells[1].SetActive(false);
    }
}