using System.Collections;
using UnityEngine;

/// <summary>
/// Class handles crouching and sliding features at once.
/// </summary>
public class Crouching : MonoBehaviour
{
    [SerializeField]  [Tooltip("Player network controller component.")]
    private PlayerNetworkController pnc;
    
    [SerializeField] [Tooltip("Time that full crouch-walk or reversed transition takes.")]
    private float fCrouchTime = 0.0005f;
    
    [SerializeField] [Tooltip("Time that slide works.")]
    private float fSlideTime = 3f;
    
    [SerializeField] [Tooltip("Time that slide gets max multiplier. Has to be lower than fSlideTime.")]
    private float fSlideHalfTime = 0.5f;
    
    [SerializeField] [Tooltip("Max speed multiplier that slide can achieve.")]
    private float fSlideMaxSpeedMultiplier = 2f;

    private bool _bCrouching, _bLastCrouching;
    private float _fTransitionStartTime, _fTransitionState, _fTransitionStateUnCrouched;
    private bool _bSliding;
    private Coroutine _cSlideSpeedOverTime;
    private float _fSlideMultiplier = 1f;

    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Crouching = this;
        
        if (fCrouchTime <= 0)
            Debug.LogError($"Value fCrouchTime must be positive. Current value: {fCrouchTime}.");
        
        if(fSlideTime <= fSlideHalfTime)
            Debug.LogError($"Slide time ({fSlideTime}) has to be longer than half time ({fSlideHalfTime}).");
    }

    /// <summary>
    /// Treat <c>Run</c> method as update. It runs in movement update.
    /// </summary>
    public void Run()
    {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            _bCrouching = !_bCrouching;
            
            //If player is sprinting and try to crouch, then slide will begin
            if (ActionsManager.GetInstance(pnc.GetInstanceID()).Sprinting.IsSprinting() && _bCrouching)
            {
                //Player slides not crouch
                _bCrouching = false;
                _cSlideSpeedOverTime = StartCoroutine(SlideSpeedOverTime());
            }
        }
        
        //Set slide info for animator
        ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder.
            playerAnimationController.PlaySlideAnimation(_bSliding);
        
        SetCrouchTransitionParameter();
        SaveLastCrouchState();
    }

    private IEnumerator SlideSpeedOverTime()
    {
        _bSliding = true;
    
        var fInitialTime = Time.time;
        var fTimeRemaining = fSlideTime - fSlideHalfTime;
        
        while (Time.time - fInitialTime < fSlideHalfTime)
        {
            var fElapsed = Time.time - fInitialTime;
            var fTime = fElapsed / fSlideHalfTime;
            _fSlideMultiplier = Mathf.Lerp(1.0f, fSlideMaxSpeedMultiplier, Mathf.Pow(fTime, 2));
            yield return null;
        }
        
        fInitialTime = Time.time;
        while (Time.time - fInitialTime < fTimeRemaining)
        {
            var fElapsed = Time.time - fInitialTime;
            var fTime = fElapsed / fTimeRemaining;
            _fSlideMultiplier = Mathf.Lerp(fSlideMaxSpeedMultiplier, 1.0f, 1 - Mathf.Pow(1-fTime, 2));
            yield return null;
        }
    
        _fSlideMultiplier = 1.0f; 
        _bSliding = false;
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
    /// Getter method <c>GetSlideSpeedMultiplier</c>.
    /// </summary>
    /// <returns>
    /// Multiplier for speed when sliding.
    /// </returns>
    public float GetSlideSpeedMultiplier()
    {
        return _fSlideMultiplier;
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
    
    /// <summary>
    /// Getter method <c>IsSliding</c>.
    /// </summary>
    /// <returns>
    /// Public info if player is now sliding.
    /// </returns>
    public bool IsSliding()
    {
        return _bSliding;
    }
}