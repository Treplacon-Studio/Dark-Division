using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
     //enum game states
    public enum GameState
    {
        MainMenu,
        Playing,
        GameOver
    }

    //instance for the singleton
    private static SceneHandler _instance;

    public static SceneHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SceneHandler>();
                
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(SceneHandler).ToString());
                    _instance = singletonObject.AddComponent<SceneHandler>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void ChangeGameState(GameState newState)
    {
        // TODO:implement logic to handle game state changes
        Debug.Log("Changing game state to: " + newState);
    }
}
