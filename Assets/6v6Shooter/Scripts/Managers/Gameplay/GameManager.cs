using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject LoadingScreenCanvas;
    public Image loadingBar;
    public Image BGImage;

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

        string currentScene = SceneHandler.Instance.GetCurrentSceneName();
        Debug.Log("Current Scene: " + currentScene);
        
        // change game state
        SceneHandler.Instance.ChangeGameState(SceneHandler.GameState.Playing);
    }

    public void OpenLoadingScreen() => LoadingScreenCanvas.SetActive(true);
    public void CloseLoadingScreen() 
    {   
        Debug.Log("LOADING SCREEN STARTED...");
        LoadingScreenCanvas.SetActive(false);
    }

    public void StartLoadingBar(string sceneName, bool loadWithPhoton, Sprite background = null)
    {
        if (background != null)
        {   
            BGImage.sprite = background;
            BGImage.color = Color.white;
        }
        else
        {
            BGImage.color = Color.black;   
        }

        OpenLoadingScreen();
        loadingBar.fillAmount = 0f;
        if (loadWithPhoton)
            StartCoroutine(LoadWithPhoton(sceneName));
        else
            StartCoroutine(LoadAsynchronously(sceneName));
    }

    IEnumerator LoadWithPhoton(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime / 3;
            loadingBar.fillAmount = progress;
            yield return null;
        }
        
        CloseLoadingScreen();
        
        while (PhotonNetwork.LevelLoadingProgress < 1f)
        {
            if (PhotonNetwork.LevelLoadingProgress == 1f)
                CloseLoadingScreen();
                
            loadingBar.fillAmount = PhotonNetwork.LevelLoadingProgress;
            yield return null;
        }
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        Debug.Log("Starting scene load");
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log($"Loading progress: {progress}");
            loadingBar.fillAmount = progress;
            yield return null;
        }

        Debug.Log("Scene load complete");
        CloseLoadingScreen();
    }
}
