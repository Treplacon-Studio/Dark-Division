using DG.Tweening;
using UnityEngine;

public class DOTMaterialAnimator : MonoBehaviour
{
    [Header("Color Parameters")]
    public Color startColor = new Color(1f, 1f, 1f, 1f);  // Start color of the material (including alpha)
    public Color endColor = new Color(1f, 1f, 1f, 0f);    // End color of the material (including alpha)

    [Header("Scale Parameters")]
    public bool enableScaleAnimation = true; // Flag to enable or disable scale animation
    public Vector3 startScale = Vector3.one;  // Start scale of the object
    public Vector3 endScale = Vector3.zero;   // End scale of the object
    public AnimationCurve scaleCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f); // Custom curve for scaling

    [Header("Duration Parameters")]
    public bool loopAnimation = true;
    public float animationDuration = 1f;

    [Header("Easing Types")]
    public bool useCustomCurveForColor = false;  // Toggle to choose between custom curve and Ease for color
    public LoopType loopType = LoopType.Yoyo;
    public Ease colorEasing = Ease.Linear;
    public AnimationCurve colorCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f); // Custom curve for color easing

    [Header("References")]
    [SerializeField] private Renderer objectRenderer;  // Reference to the object's Renderer

    private Material objectMaterial;
    private Transform objectTransform;

    void OnEnable()
    {
        AnimateObject();
    }

    void OnDisable()
    {
        if (objectMaterial != null)
        {
            objectMaterial.DOKill();
        }
        if (objectTransform != null)
        {
            objectTransform.DOKill();
        }
    }

    void AnimateObject()
    {
        if (objectRenderer != null)
        {
            // Get the material and transform components
            objectMaterial = objectRenderer.material;
            objectTransform = transform;

            if (objectMaterial == null)
            {
                Debug.LogWarning("No Material found on " + gameObject.name);
                return;
            }

            // Set the initial color and scale
            objectMaterial.color = startColor;
            objectTransform.localScale = startScale;

            // Create the color animation
            Tweener colorTween;
            if (useCustomCurveForColor)
            {
                colorTween = objectMaterial.DOColor(endColor, animationDuration)
                                           .SetEase(colorCurve);  // Use the custom animation curve for color
            }
            else
            {
                colorTween = objectMaterial.DOColor(endColor, animationDuration)
                                           .SetEase(colorEasing);  // Use the predefined Ease type for color
            }

            // Create the scale animation if enabled
            Tweener scaleTween = null;
            if (enableScaleAnimation)
            {
                scaleTween = objectTransform.DOScale(endScale, animationDuration)
                                            .SetEase(scaleCurve);  // Use the custom curve for scaling
            }

            // Combine the tweens into a sequence if looping
            Sequence sequence = DOTween.Sequence();
            sequence.Append(colorTween);

            if (enableScaleAnimation)
            {
                sequence.Join(scaleTween);
            }

            if (loopAnimation)
            {
                sequence.SetLoops(-1, loopType);
            }

            sequence.Play();
        }
        else
        {
            Debug.LogWarning("No Renderer component found on " + gameObject.name);
        }
    }
}
