using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerHUD : MonoBehaviour
{
    [Header("Weapon Boxes")]
    
    [SerializeField] [Tooltip("Text on current weapon box.")]
    private TextMeshProUGUI currentWeaponName;
    
    [SerializeField] [Tooltip("Text on hidden weapon box.")]
    private TextMeshProUGUI hiddenWeaponName;

    [SerializeField] [Tooltip("Ammo in current mag left.")]
    private TextMeshProUGUI ammoLeftInMagCurrentText;
    
    [SerializeField] [Tooltip("Ammo in current mag left.")]
    private TextMeshProUGUI ammoLeftInMagHiddenText;

    [SerializeField] [Tooltip("Whole ammo that left.")]
    private TextMeshProUGUI ammoLeftCurrentText;
    
    [SerializeField] [Tooltip("Whole ammo that left.")]
    private TextMeshProUGUI ammoLeftHiddenText;

    [Header("TIME")]
    public TMP_Text TimeRemainingText;

    [Header("SCORE")]
    public TMP_Text TeamScoreText;
    public TMP_Text EnemyScoreText;

    void Start()
    {
        UpdateTeamScores();
    }

    private void Update()
    {
        if (TeamDeathmatchManager.instance.CheckIfGameShouldEnd())
        {
            return;
        }

        UpdateWeaponBoxes();
        UpdateTeamScores();
        UpdateTimerDisplay();
    }

    private void UpdateWeaponBoxes()
    {
        if (ActionsManager.Instance?.Switching.WeaponComponent() is null)
            return;

        var weaponsNames = ActionsManager.Instance.Switching.GetWeaponsNames();
        currentWeaponName.text = weaponsNames[0];
        hiddenWeaponName.text = weaponsNames[1];
        
        ammoLeftInMagCurrentText.text = ActionsManager.Instance.ComponentHolder.bulletPoolingManager.GetAmmoPrimary().ToString();
        ammoLeftInMagHiddenText.text = ActionsManager.Instance.ComponentHolder.bulletPoolingManager.GetAmmoSecondary().ToString();
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(TeamDeathmatchManager.instance.TimeRemaining / 60);
        int seconds = Mathf.FloorToInt(TeamDeathmatchManager.instance.TimeRemaining % 60);

        TimeRemainingText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdateTeamScores()
    {
        Team? team = TeamManager.GetTeam(PhotonNetwork.LocalPlayer);
        if (team == Team.Blue)
        {
            TeamScoreText.text = $"{TeamDeathmatchManager.instance.TeamBlueScore}";
            EnemyScoreText.text = $"{TeamDeathmatchManager.instance.TeamRedScore}";
        }
        else if (team == Team.Red)
        {
            TeamScoreText.text = $"{TeamDeathmatchManager.instance.TeamRedScore}";
            EnemyScoreText.text = $"{TeamDeathmatchManager.instance.TeamBlueScore}";
        }
        else
            Debug.Log("Error validating team for this player.");        
    }
}
