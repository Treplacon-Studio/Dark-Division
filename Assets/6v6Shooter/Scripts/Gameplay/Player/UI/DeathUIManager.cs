using UnityEngine;
using TMPro;
using System.Collections;

public class DeathUIManager : MonoBehaviour
{
    public GameObject deathCanvas;
    public TMP_Text deathMessage;
    public TMP_Text countdownText;

    void Start()
    {
        deathCanvas.SetActive(false);
    }

    public void ShowDeathUI(float countdown)
    {
        deathCanvas.SetActive(true);
        StartCoroutine(Countdown(countdown));
    }

    IEnumerator Countdown(float countdown)
    {
        while (countdown > 0)
        {
            countdownText.text = $"Respawn in {countdown} seconds";
            yield return new WaitForSeconds(1f);
            countdown--;
        }
        deathCanvas.SetActive(false);
    }
}
