using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DOTImageAnimator : MonoBehaviour
{
    [Header("Color Parameters")]
    public Color startColor = new Color(1f, 1f, 1f, 1f);  // Start color of the image (including alpha)
    public Color endColor = new Color(1f, 1f, 1f, 0f);    // End color of the image (including alpha)

    [Header("Scale Parameters")]
    public bool enableScaleAnimation = true; // Flag to enable or disable scale animation
    public Vector3 startScale = Vector3.one;  // Start scale of the object
    public Vector3 endScale = Vector3.zero;   // End scale of the object
    public AnimationCurve scaleCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f); // Custom curve for scaling

    [Header("Duration Parameters")]
    public bool loopAnimation = true;
    public float fadeDuration = 1f;

    [Header("Easing Types")]
    public bool useCustomCurveForColor = false;  // Toggle to choose between custom curve and Ease for color
    public LoopType loopType = LoopType.Yoyo;
    public Ease colorEasing = Ease.Linear;
    public AnimationCurve colorCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f); // Custom curve for color easing

    [Header("References")]
    [SerializeField] private Image uiImage;

    private RectTransform rectTransform;

    void OnEnable()
    {
        AnimateObject();
    }

    void OnDisable()
    {
        if (uiImage != null)
        {
            uiImage.DOKill();
        }
        if (rectTransform != null)
        {
            rectTransform.DOKill();
        }
    }

    void AnimateObject()
    {
        if (uiImage != null)
        {
            // Get the RectTransform component for scaling
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                Debug.LogWarning("No RectTransform component found on " + gameObject.name);
                return;
            }

            // Set the initial color and scale
            uiImage.color = startColor;
            rectTransform.localScale = startScale;

            // Create the color animation
            Tweener colorTween;
            if (useCustomCurveForColor)
            {
                colorTween = uiImage.DOColor(endColor, fadeDuration)
                                   .SetEase(colorCurve);  // Use the custom animation curve for color
            }
            else
            {
                colorTween = uiImage.DOColor(endColor, fadeDuration)
                                   .SetEase(colorEasing);  // Use the predefined Ease type for color
            }

            // Create the scale animation if enabled
            Tweener scaleTween = null;
            if (enableScaleAnimation)
            {
                scaleTween = rectTransform.DOScale(endScale, fadeDuration)
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
            Debug.LogWarning("No Image component found on " + gameObject.name);
        }
    }
}
