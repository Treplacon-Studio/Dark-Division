using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour
{
    public string onClickSound;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (!string.IsNullOrEmpty(onClickSound))
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void PlayClickSound()
    {
        if (!string.IsNullOrEmpty(onClickSound))
        {
            AudioManager.PlayOneShot(onClickSound, Vector3.zero);
        }
    }
}
