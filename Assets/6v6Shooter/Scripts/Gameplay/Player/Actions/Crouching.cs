using System.Linq;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class Crouching : MonoBehaviour
{
    [SerializeField]  [Tooltip("Player network controller component.")]
    private PlayerNetworkController pnc;
    
    [SerializeField] [Tooltip("Time that full crouch-walk or reversed transition takes.")]
    private float fCrouchTime = 0.0005f;

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
        var fCurrentTime = Time.time;
        
        //Reset transition when crouching started
        if (_bCrouching && !_bLastCrouching)
            _fTransitionStartTime = fCurrentTime;
        
        //Clamp normalized time to 01
        _fTransitionState = Mathf.Clamp01((fCurrentTime - _fTransitionStartTime) / fCrouchTime);
        
        //Set transition states in case of crouching activation
        if (_bCrouching)
            _fTransitionStateUnCrouched = _fTransitionState;
        else
        {
            if (_fTransitionStateUnCrouched > 0)
            {
                _fTransitionStateUnCrouched -= Time.deltaTime / fCrouchTime;
                _fTransitionStateUnCrouched = Mathf.Max(_fTransitionStateUnCrouched, 0);
            }
        }
        
        //Set proper state
        var fTransitionStateToApply = _bCrouching ? _fTransitionState : _fTransitionStateUnCrouched;
        ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder.playerAnimationController
            .SetCrouchingState(fTransitionStateToApply);
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