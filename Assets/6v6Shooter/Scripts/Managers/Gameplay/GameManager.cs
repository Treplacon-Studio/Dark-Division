using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject LoadingScreenCanvas;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        CloseLoadingScreen();
    }

    public void OpenLoadingScreen() => LoadingScreenCanvas.SetActive(true);
    public void CloseLoadingScreen() => LoadingScreenCanvas.SetActive(false);
}
