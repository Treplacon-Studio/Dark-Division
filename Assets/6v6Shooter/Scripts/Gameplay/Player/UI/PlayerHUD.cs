using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerHUD : MonoBehaviourPun
{
    [SerializeField] private PlayerNetworkController pnc;
    
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

    public GameObject GameScorePanel;    

    private bool _hudDisplayed = true;

    private void Awake()
    {
        string currentScene = SceneHandler.Instance.GetCurrentSceneName();
        if (currentScene == "S05_PracticeRange")
            _hudDisplayed = false;
    }

    void Start()
    {
        if (_hudDisplayed is true)
            GameScorePanel.SetActive(true);
        else 
            GameScorePanel.SetActive(false);
    }

    private void Update()
    {
        UpdateWeaponBoxes();

        if (_hudDisplayed)
        {
            UpdateTeamScores();
            UpdateTimerDisplay();
        }
    }

    private void UpdateWeaponBoxes()
    {
        var instance = ActionsManager.GetInstance(pnc.GetInstanceID());
        var switching = instance?.Switching;
        var weaponComponent = switching?.WeaponComponent();
        
        if (instance is null || switching is null || weaponComponent is null)
            return;

        var weaponsNames = switching.GetWeaponsNames();
        currentWeaponName.text = weaponsNames[0];
        hiddenWeaponName.text = weaponsNames[1];
        
        ammoLeftInMagCurrentText.text = ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder.bulletPoolingManager.GetAmmoPrimary().ToString();
        ammoLeftInMagHiddenText.text = ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder.bulletPoolingManager.GetAmmoSecondary().ToString();
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
