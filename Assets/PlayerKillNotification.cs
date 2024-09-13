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

        GameObject xpInstance = Instantiate(xpPrefab, notificationSpawnPoint);

        Debug.Log("TriggerKillNotification: Starting notification animation.");
        StartCoroutine(ScaleAndFadeNotification(xpInstance, 3f));
    }

    private IEnumerator ScaleAndFadeNotification(GameObject xpInstance, float fadeDuration)
    {

        float scaleDuration = 0.2f;
        float targetScale = 1.5f;
        
        yield return StartCoroutine(ScaleObject(xpInstance, targetScale, scaleDuration));

        yield return StartCoroutine(FadeOutNotification(xpInstance, fadeDuration));
    }

    private IEnumerator ScaleObject(GameObject xpInstance, float targetScale, float duration)
    {

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

        timer = 0f;
        while (timer <= duration)
        {
            xpInstance.transform.localScale = Vector3.Lerp(targetScaleVector, originalScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        xpInstance.transform.localScale = originalScale;
    }

    private IEnumerator FadeOutNotification(GameObject xpInstance, float fadeDuration)
    {

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

        Destroy(xpInstance);
    }
}
