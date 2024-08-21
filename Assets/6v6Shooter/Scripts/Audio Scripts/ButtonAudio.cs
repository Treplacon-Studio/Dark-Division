using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour
{
    public EventReference onClickSound;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogWarning("Button component missing.");
            return;
        }

        button.onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        if (AudioManager.Instance != null && !onClickSound.IsNull)
        {
            AudioManager.Instance.PlayOneShot(onClickSound, Vector3.zero);
        }
        else
        {
            Debug.LogWarning("AudioManager or onClickSound is not set.");
        }
    }
}
