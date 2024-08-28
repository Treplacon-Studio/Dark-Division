using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DOTDecalAnimator : MonoBehaviour
{
    [Header("Opacity Parameters")]
    public bool enableOpacityAnimation = true;  // Flag to enable or disable opacity animation
    public float startOpacity = 1f;  // Start opacity of the decal
    public float endOpacity = 0f;    // End opacity of the decal
    public bool useCustomCurveForOpacity = false;  // Toggle to use custom curve for opacity animation
    public AnimationCurve opacityCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f); // Custom curve for opacity

    [Header("Scale Parameters")]
    public bool enableScaleAnimation = true; // Flag to enable or disable scale animation
    public Vector3 startScale = Vector3.one;  // Start scale of the decal
    public Vector3 endScale = Vector3.zero;   // End scale of the decal
    public AnimationCurve scaleCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f); // Custom curve for scaling

    [Header("Rotation Parameters")]
    public bool enableRandomZRotation = false; // Flag to enable or disable random rotation on Z-axis
    public float rotationRangeZ = 360f;        // Range of random rotation for the Z-axis

    [Header("Duration Parameters")]
    public bool loopAnimation = true;
    public float animationDuration = 1f;

    [Header("References")]
    [SerializeField] private DecalProjector decalProjector;  // Reference to the URP Decal Projector

    private Transform decalTransform;
    private Material decalMaterial;

    void OnEnable()
    {
        AnimateDecal();
    }

    void OnDisable()
    {
        if (decalMaterial != null)
        {
            decalMaterial.DOKill();
        }
        if (decalTransform != null)
        {
            decalTransform.DOKill();
        }
    }

    void AnimateDecal()
    {
        if (decalProjector != null)
        {
            // Get the material and transform components
            decalMaterial = decalProjector.material;
            decalTransform = transform;

            if (decalMaterial == null)
            {
                Debug.LogWarning("No Material found on Decal Projector of " + gameObject.name);
                return;
            }

            // Set the initial opacity and scale
            if (enableOpacityAnimation)
            {
                decalMaterial.SetFloat("_Opacity", startOpacity);
            }
            decalTransform.localScale = startScale;

            // Apply random rotation on the Z-axis if enabled
            if (enableRandomZRotation)
            {
                float randomZRotation = Random.Range(0f, rotationRangeZ);
                decalTransform.localRotation = Quaternion.Euler(0f, 0f, randomZRotation);
            }

            // Create the opacity animation if enabled
            Tweener opacityTween = null;
            if (enableOpacityAnimation)
            {
                if (useCustomCurveForOpacity)
                {
                    opacityTween = decalMaterial.DOFloat(endOpacity, "_Opacity", animationDuration)
                                                .SetEase(opacityCurve);  // Use the custom curve for opacity
                }
                else
                {
                    opacityTween = decalMaterial.DOFloat(endOpacity, "_Opacity", animationDuration)
                                                .SetEase(Ease.Linear);  // Use Linear easing if no custom curve is set
                }
            }

            // Create the scale animation if enabled
            Tweener scaleTween = null;
            if (enableScaleAnimation)
            {
                scaleTween = decalTransform.DOScale(endScale, animationDuration)
                                           .SetEase(scaleCurve);  // Use the custom curve for scaling
            }

            // Combine the tweens into a sequence if looping
            Sequence sequence = DOTween.Sequence();
            if (opacityTween != null)
            {
                sequence.Append(opacityTween);
            }

            if (scaleTween != null)
            {
                sequence.Join(scaleTween);
            }

            if (loopAnimation)
            {
                sequence.SetLoops(-1, LoopType.Yoyo);
            }

            sequence.Play();
        }
        else
        {
            Debug.LogWarning("No DecalProjector component found on " + gameObject.name);
        }
    }
}
