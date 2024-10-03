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

        // Change game state
        SceneHandler.Instance.ChangeGameState(SceneHandler.GameState.Playing);
    }

    public void OpenLoadingScreen() => LoadingScreenCanvas.SetActive(true);
    
    public void CloseLoadingScreen() 
    {   
        Debug.Log("Loading screen closed.");
        LoadingScreenCanvas.SetActive(false);
    }

    public void StartLoadingBar(string sceneName, bool loadWithPhoton, Sprite background = null)
    {
        // Display the loading screen and optionally set the background
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

        // Load scene with or without Photon
        if (loadWithPhoton)
            StartCoroutine(LoadWithPhoton(sceneName));
        else
            StartCoroutine(LoadAsynchronously(sceneName));
    }

    private IEnumerator LoadWithPhoton(string sceneName)
    {
        // Start loading the scene with Photon
        PhotonNetwork.LoadLevel(sceneName);
        
        // Wait until the level is fully loaded
        while (PhotonNetwork.LevelLoadingProgress < 1f)
        {
            // Update the loading bar with Photon loading progress
            loadingBar.fillAmount = PhotonNetwork.LevelLoadingProgress;
            yield return null;
        }

        // Ensure the loading screen is closed only after loading is complete
        CloseLoadingScreen();
        Debug.Log("Scene load complete with Photon.");
    }

    private IEnumerator LoadAsynchronously(string sceneName)
    {
        Debug.Log("Starting scene load asynchronously.");
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.fillAmount = progress;
            yield return null;
        }

        Debug.Log("Scene load complete asynchronously.");
        CloseLoadingScreen();
    }
}
