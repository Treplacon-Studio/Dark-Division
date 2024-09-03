using System.Collections;
using UnityEngine;

public class PlayerKillNotification : MonoBehaviour
{
    public GameObject xpPrefab;
    public Transform notificationSpawnPoint;

    public void TriggerKillNotification()
    {
        if (xpPrefab == null)
        {
            Debug.LogError("XP Prefab is not assigned!");
            return;
        }

        if (notificationSpawnPoint == null)
        {
            Debug.LogError("Notification Spawn Point is not assigned!");
            return;
        }

        Debug.Log("TriggerKillNotification: XP Prefab and Spawn Point are set.");

        GameObject xpInstance = Instantiate(xpPrefab, notificationSpawnPoint);
        if (xpInstance != null)
        {
            Debug.Log("XP Instance successfully instantiated.");
        }
        else
        {
            Debug.LogError("Failed to instantiate XP Instance.");
        }

        Debug.Log("TriggerKillNotification: Starting notification animation.");
        StartCoroutine(ScaleAndFadeNotification(xpInstance, 3f));
    }

    private IEnumerator ScaleAndFadeNotification(GameObject xpInstance, float fadeDuration)
    {
        Debug.Log("ScaleAndFadeNotification: Starting scaling and fading.");

        float scaleDuration = 0.2f;
        float targetScale = 1.5f;
        
        yield return StartCoroutine(ScaleObject(xpInstance, targetScale, scaleDuration));

        yield return StartCoroutine(FadeOutNotification(xpInstance, fadeDuration));
    }

    private IEnumerator ScaleObject(GameObject xpInstance, float targetScale, float duration)
    {
        // Debug.Log("ScaleObject: Scaling up to " + targetScale);

        Vector3 originalScale = xpInstance.transform.localScale;
        Vector3 targetScaleVector = originalScale * targetScale;

        float timer = 0f;

        while (timer <= duration)
        {
            xpInstance.transform.localScale = Vector3.Lerp(originalScale, targetScaleVector, timer / duration);
            timer += Time.deltaTime;
            yield return null; 
        }

        xpInstance.transform.localScale = targetScaleVector;

        // Debug.Log("ScaleObject: Scale up completed. Now scaling down.");

        timer = 0f;
        while (timer <= duration)
        {
            xpInstance.transform.localScale = Vector3.Lerp(targetScaleVector, originalScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        xpInstance.transform.localScale = originalScale;
        // Debug.Log("ScaleObject: Scaling completed.");
    }

    private IEnumerator FadeOutNotification(GameObject xpInstance, float fadeDuration)
    {
        Debug.Log("FadeOutNotification: Starting fade out.");

        CanvasGroup canvasGroup = xpInstance.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogWarning("FadeOutNotification: No CanvasGroup found, adding one.");
            canvasGroup = xpInstance.AddComponent<CanvasGroup>();
        }

        float fadeSpeed = 1f / fadeDuration;

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        canvasGroup.alpha = 0;
        Debug.Log("FadeOutNotification: Fading completed.");

        Destroy(xpInstance);
        Debug.Log("FadeOutNotification: Notification destroyed.");
    }
}
