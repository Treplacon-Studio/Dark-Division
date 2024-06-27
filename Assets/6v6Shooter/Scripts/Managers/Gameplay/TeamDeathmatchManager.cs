using UnityEngine;
using TMPro; 

public class TeamDeathmatchManager : MonoBehaviour
{
    public static TeamDeathmatchManager instance;

    [Header("TIME")]
    public float TimeRemaining = 600.0f;
    public TMP_Text TimeRemainingText;

    [Header("SCORE")]
    public int TeamAScore = 0;
    public int TeamBScore = 0;
    public TMP_Text TeamAAScoreText;
    public TMP_Text TeamBScoreText;

    void Start()
    {
        TeamAAScoreText.text = $"{TeamAScore}";
        TeamBScoreText.text = $"{TeamBScore}";
    }

	void OnEnable() {
        if(instance == null) 
            instance = this;
    }

    void Update()
    {
        if (TimeRemaining > 0)
        {
            TimeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(TimeRemaining / 60);
        int seconds = Mathf.FloorToInt(TimeRemaining % 60);

        TimeRemainingText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
