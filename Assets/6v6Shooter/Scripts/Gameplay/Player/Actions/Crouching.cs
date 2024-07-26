using System.Linq;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class Crouching : MonoBehaviour
{
    [SerializeField]  [Tooltip("Player network controller component.")]
    private PlayerNetworkController pnc;
    
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;
    
    [SerializeField] [Tooltip("Difference between stand and crouch height.")]
    private float fHeightDelta;
    
    [SerializeField] [Tooltip("Speed of switching through stand and crouch states. Vector2(up, down).")]
    private Vector2 v2CrouchSpeed;
    
    [SerializeField] [Tooltip("Names of the stand-crouch states.")]
    private string sUpDownStateName, sDownUpStateName;

    private bool _bCrouching, _bLastCrouching;
    private bool _bDuringAnimation;
    private Animator _animator;
    
    private static readonly int PCrouching = Animator.StringToHash("pCrouching");

    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Crouching = this;
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Treat <c>Run</c> method as update. It runs in movement update.
    /// </summary>
    public void Run()
    {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
            _bCrouching = !_bCrouching;
        
        if (_bDuringAnimation)
            return;

        _animator ??= GetComponent<Animator>();
        _animator.SetBool(PCrouching, _bCrouching);

        return;
        var currentState = _animator.GetCurrentAnimatorStateInfo(2);
        var fNormalizedTime = currentState.normalizedTime % 1;
        
        if (_animator.IsInTransition(2))
        {
            if (_bCrouching)
                _animator.Play(currentState.fullPathHash, 2, 1 - fNormalizedTime);
            else
                _animator.Play(currentState.fullPathHash, 2, fNormalizedTime);
        }
        else
        {
            _animator.Play(currentState.fullPathHash, 2, fNormalizedTime);
        }

        SaveLastCrouchState();
    }
    
    /// <summary>
    /// Method <c>SaveLastCrouchState</c> updates last crouch state.
    /// Should be called in the end of code flow.
    /// </summary>
    private void SaveLastCrouchState()
    {
        _bLastCrouching = _bCrouching;
    }

    /// <summary>
    /// Getter method <c>IsCrouching</c>.
    /// </summary>
    /// <returns>
    /// Public info if player is now crouching.
    /// </returns>
    public bool IsCrouching()
    {
        return _bCrouching;
    }
}