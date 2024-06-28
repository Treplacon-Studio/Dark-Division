using UnityEngine;
using TMPro; 
using Photon.Pun;

public class TeamDeathmatchManager : MonoBehaviourPunCallbacks
{
    public static TeamDeathmatchManager instance;

    [Header("TIME")]
    public float TimeRemaining = 600.0f;

    [Header("SCORE")]
    public int TeamBlueScore = 0;
    public int TeamRedScore = 0;

	void Awake() {
        if(instance == null) 
            instance = this;
    }

    void Update()
    {
        if (CheckIfGameShouldEnd())
        {
            TimeRemaining = 0.0f;
            return;
        }
        
        if (TimeRemaining > 0)
            TimeRemaining -= Time.deltaTime;
    }

    public bool CheckIfGameShouldEnd()
    {
        if (TimeRemaining < 1 || TeamBlueScore == 75 || TeamRedScore == 75)
            return true;

        return false;
    }

    public void AddPointForTeam()
    {
        Debug.Log(PhotonNetwork.LocalPlayer);

        Team? team = TeamManager.GetTeam(PhotonNetwork.LocalPlayer);
        if (team == Team.Blue)
            TeamBlueScore++;
        else if (team == Team.Red)
            TeamRedScore++;
        else
            Debug.Log("Error validating team for this player so point will not count.");
    }
}
