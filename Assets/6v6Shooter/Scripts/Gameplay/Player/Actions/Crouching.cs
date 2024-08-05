using System.Collections;
using UnityEngine;

/// <summary>
/// Class handles crouching and sliding features at once.
/// </summary>
public class Crouching : MonoBehaviour
{
    #region Base Properties
    
    [Header("Basic action setup.")]
    
    [SerializeField]  [Tooltip("Player network controller component.")]
    private PlayerNetworkController pnc;
    
    #endregion Base Properties
    
    #region Specific Properties
    
    [Header("Specific action setup.")]
    
    [SerializeField] [Tooltip("Time that full crouch-walk or reversed transition takes.")]
    private float fCrouchTime = 0.0005f;
    
    [SerializeField] [Tooltip("Time that slide works.")]
    private float fSlideTime = 3f;
    
    [SerializeField] [Tooltip("Time that hands go from run to idle when sliding and backwards.")]
    private float fIdleTransitionTime = 1f;
    
    [SerializeField] [Tooltip("Time that slide gets max multiplier. Has to be lower than fSlideTime.")]
    private float fSlideHalfTime = 0.5f;
    
    [SerializeField] [Tooltip("Max speed multiplier that slide can achieve.")]
    private float fSlideMaxSpeedMultiplier = 2f;

    private bool _bCrouching, _bLastCrouching;
    private float _fTransitionStartTime, _fTransitionState, _fTransitionStateUnCrouched;
    private bool _bSliding, _bLastSliding, _bLastUpdateIdle;
    private Coroutine _cSlideSpeedOverTime, _cUpdateSlideIdleMultiplierInTime;
    private float _fSlideMultiplier = 1f;
    private float _fIdleInSlideMultiplier = 1f;
    
    #endregion Specific Properties

    #region Base Methods
    
    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Crouching = this;
        
        if (fCrouchTime <= 0)
            Debug.LogError($"Value fCrouchTime must be positive. Current value: {fCrouchTime}.");
        
        if(fSlideTime <= fSlideHalfTime)
            Debug.LogError($"Slide time ({fSlideTime}) has to be longer than half time ({fSlideHalfTime}).");
    }

    /// <summary>
    /// Called every frame method for action handle.
    /// </summary>
    public void Run()
    {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            //Handle slide cancel
            if (_bSliding)
            {
                if(_cSlideSpeedOverTime is not null)
                    StopCoroutine(_cSlideSpeedOverTime);
                _bSliding = false;
            }
            
            //Handle crouching
            else
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
        }
        
        //Set slide info for animator
        ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder.
            playerAnimationController.PlaySlideAnimation(_bSliding);
        
        SetCrouchTransitionParameter();
        UpdateIdleMultiplier();
        SaveLastStates();
    }
    
    #endregion Base Methods
    
    #region Specific Methods

    /// <summary>
    /// Async square function speed transition method for sliding.
    /// </summary>
    private IEnumerator SlideSpeedOverTime()
    {
        _bSliding = true;
    
        var fInitialTime = Time.time;
        var fTimeRemaining = fSlideTime - fSlideHalfTime;
        
        //Handle slide acceleration
        while (Time.time - fInitialTime < fSlideHalfTime)
        {
            var fElapsed = Time.time - fInitialTime;
            var fTime = fElapsed / fSlideHalfTime;
            _fSlideMultiplier = Mathf.Lerp(1.0f, fSlideMaxSpeedMultiplier, Mathf.Pow(fTime, 2));
            yield return null;
        }
        
        fInitialTime = Time.time;
        
        //Handle slide deacceleration
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
    /// Controls crouch (transitional) state between walking/idling and crouch walking/crouch idling.
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
    /// If sliding, smoothly transits input from moving to idle and backwards.
    /// </summary>
    /// <param name="input">Entry input vector to be updated.</param>
    /// <returns>
    /// Updated input vector taking in mind sliding feature.
    /// </returns>
    public Vector2 TransitionToIdleWhenSliding(Vector2 input)
    {
        return input * _fIdleInSlideMultiplier;
    }

    /// <summary>
    /// Controls idle while sliding.
    /// </summary>
    private void UpdateIdleMultiplier()
    {
        //If sliding started run coroutine that will decrease multiplier in time
        _cUpdateSlideIdleMultiplierInTime ??= _bSliding switch
        {
            true when !_bLastSliding => StartCoroutine(UpdateSlideIdleMultiplierInTime(false)),
            false when _bLastSliding => StartCoroutine(UpdateSlideIdleMultiplierInTime(true)),
            _ => _cUpdateSlideIdleMultiplierInTime
        };
    }
    
    /// <summary>
    /// Changes idle multiplier in time.
    /// </summary>
    /// <param name="increasing">Indicates whether the slide multiplier should increase or decrease.</param>
    private IEnumerator UpdateSlideIdleMultiplierInTime(bool increasing)
    {
        _bLastUpdateIdle = increasing;
        
        var fTargetMultiplier = increasing ? 1f : 0f;
        var fElapsedTime = 0f;
        
        while (fElapsedTime < fIdleTransitionTime)
        {
            fElapsedTime += Time.deltaTime;
            _fIdleInSlideMultiplier = Mathf.Lerp(_fIdleInSlideMultiplier, fTargetMultiplier, fElapsedTime / fIdleTransitionTime);
            yield return null;
        }
        
        _fIdleInSlideMultiplier = Mathf.Clamp01(fTargetMultiplier);
        _cUpdateSlideIdleMultiplierInTime = null;
    }
    
    /// <summary>
    /// Updates last crouch and slide state.
    /// </summary>
    private void SaveLastStates()
    {
        _bLastCrouching = _bCrouching;
        _bLastSliding = _bSliding;
    }
    
    #endregion Specific Methods
    
    #region Accessors

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
    /// Getter method.
    /// </summary>
    /// <returns>
    /// Information if player is now crouching.
    /// </returns>
    public bool IsCrouching()
    {
        return _bCrouching;
    }
    
    /// <summary>
    /// Getter method.
    /// </summary>
    /// <returns>
    /// Information if player is now sliding.
    /// </returns>
    public bool IsSliding()
    {
        return _bSliding;
    }
    
    #endregion Accessors
}