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
    
    [SerializeField] [Tooltip("Time that full crouch-walk or reversed transition takes.")]
    private float fCrouchTime = 1f;
    
    [SerializeField] [Tooltip("Speed of switching through stand and crouch states. Vector2(up, down).")]
    private Vector2 v2CrouchSpeed;
    
    [SerializeField] [Tooltip("Names of the stand-crouch states.")]
    private string sUpDownStateName, sDownUpStateName;

    private bool _bCrouching, _bLastCrouching;
    private float _fTransitionStartTime, _fTransitionState, _fTransitionStateUnCrouched;

    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Crouching = this;
        
        if (fCrouchTime <= 0)
            Debug.LogError($"Value fCrouchTime must be positive. Current value: {fCrouchTime}.");
    }

    /// <summary>
    /// Treat <c>Run</c> method as update. It runs in movement update.
    /// </summary>
    public void Run()
    {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
            _bCrouching = !_bCrouching;
        
        SetCrouchTransitionParameter();
        SaveLastCrouchState();
    }

    /// <summary>
    /// Controls crouch state between walking/idling and crouch walking/crouch idling.
    /// </summary>
    private void SetCrouchTransitionParameter()
    {
        //On crouching start set transition start time
        if (_bCrouching && !_bLastCrouching)
            _fTransitionStartTime = Time.time;
        
        //Clamped 01 value of transition state to set in animator.
        _fTransitionState = Mathf.Clamp01((Time.time - _fTransitionStartTime)/fCrouchTime);

        //If crouching set un-crouched state to same as crouched.
        if (_bCrouching)
            _fTransitionStateUnCrouched = _fTransitionState;
        
        //Decrease un-crouched state everytime (character keep getting up from crouch with crouch time speed).
        if(_fTransitionStateUnCrouched > 0)
            _fTransitionStateUnCrouched -= _fTransitionState * fCrouchTime/Time.deltaTime;
        
        //Apply state
        ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder.playerAnimationController
            .SetCrouchingState(_bCrouching ? _fTransitionState : _fTransitionStateUnCrouched);
    }
    
    /// <summary>
    /// Updates last crouch state.
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