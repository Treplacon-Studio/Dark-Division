using Photon.Pun;
using UnityEngine;

/// <summary>
/// Class handles weapon switching feature.
/// </summary>
public class Switching : MonoBehaviourPunCallbacks
{
    #region Base Parameters
    
    [Header("Basic action setup.")]
    
    [SerializeField] [Tooltip("Player network controller component.")]
    private PlayerNetworkController pnc;
    
    #endregion Base Parameters
    
    #region Specific Parameters
    
    [SerializeField] [Tooltip("Socket where the gun is kept.")]
    private GameObject gunSocket;
    
    private GameObject[] _equippedGuns;
    private GameObject _weapon;
    private Transform _tBulletStartPoint;
    private float _fNextFireTime;
    private int _iGunInHandsIndex;
    private bool _bWeaponInitialized;
    
    #endregion Specific Parameters

    #region Base Methods
    
    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Switching = this;
    }
    
    /// <summary>
    /// Called every frame method for action handle.
    /// </summary>
    public void Run()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        
        var componentHolder = ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder;
        if (componentHolder.playerAnimationController.reloadingLock)
            return;

        if (scroll > 0f) //Scrolled up
        {
            if (_iGunInHandsIndex == 0)
                SwitchWeapon(_equippedGuns.Length - 1);
            else
                SwitchWeapon(_iGunInHandsIndex - 1);
        }
        else if (scroll < 0f) //Scrolled down
        {
            SwitchWeapon((_iGunInHandsIndex + 1) % _equippedGuns.Length);
        }
    }
    
    #endregion Base Methods
    
    #region Specific Methods
    
    /// <summary>
    /// Sets new equipment for a player.
    /// </summary>
    /// <param name="weapons">Weapons that should be set as equipment.</param>
    /// <param name="attachments">Attachments that these weapons will have.</param>
    public void SetNewEquipment(string[] weapons, int[,] attachments)
    {
        _iGunInHandsIndex = 0;

        if (_equippedGuns is not null)
        {
            foreach (var gun in _equippedGuns)
            {
                gun.SetActive(false);
                gun.transform.SetParent(null);
                DestroyImmediate(gun, true);
            }
        }

        _equippedGuns = new GameObject[weapons.Length];
        
        var componentHolder = ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder;
        componentHolder.bulletPoolingManager.ClearPools();

        for (var i = 0; i < _equippedGuns.Length; i++)
        {
            _equippedGuns[i] = PhotonNetwork.Instantiate(weapons[i],
                gunSocket.transform.position,
                gunSocket.transform.rotation * Quaternion.Euler(90, 0, 0));
            
            _equippedGuns[i].transform.SetParent(gunSocket.transform);

            _equippedGuns[i].GetComponent<Weapon>().ApplyAttachmentsAssaultRifle(attachments, i);
            if (_equippedGuns[i] != null)
                _equippedGuns[i].SetActive(false);
            else
                Debug.LogError("Equipped gun is null!");
            _equippedGuns[i].transform.SetParent(gunSocket.transform);
            _equippedGuns[i].transform.localPosition = Vector3.zero;
            _equippedGuns[i].transform.localScale = Vector3.one * 0.01f;
            var mg = _equippedGuns[i].GetComponent<Weapon>().GetMag().GetComponent<Mag>();
            componentHolder.bulletPoolingManager.AddPool(new BulletPoolingManager.Pool(i, mg.ammoType, mg.size));
        }

        componentHolder.bulletPoolingManager.ApplyPools();
        SwitchWeapon(_iGunInHandsIndex);
        _bWeaponInitialized = true;
    }

    /// <summary>
    /// Switches weapon to given.
    /// </summary>
    /// <param name="wn">Number (index) of the weapon in equipment.</param>
    private void SwitchWeapon(int wn)
    {
        _iGunInHandsIndex = wn;
        foreach (var w in _equippedGuns)
            w.SetActive(false);
        
        var componentHolder = ActionsManager.GetInstance(pnc.GetInstanceID()).ComponentHolder;
        componentHolder.weaponSpecificAnimations.ChangeAnimations(_equippedGuns[wn].GetComponent<Weapon>().Info().Name());
        
        _weapon = _equippedGuns[wn];
        _weapon.SetActive(true);
    }
    
    #endregion Specific Methods

    #region Accessors
    
    /// <summary>
    /// Getter method.
    /// </summary>
    /// <returns>Current weapon.</returns>
    public Weapon WeaponComponent()
    {
        if (_equippedGuns == null)
            return null;
        
        if (_weapon == null)
            _weapon = _equippedGuns[_iGunInHandsIndex];

        if (_weapon == null)
            return null;

        return _weapon.GetComponent<Weapon>();
    }
    
    /// <summary>
    /// Getter method.
    /// </summary>
    /// <returns>Index of current weapon.</returns>
    public int GetCurrentWeaponID()
    {
        return _iGunInHandsIndex;
    }

    /// <summary>
    /// Getter method.
    /// </summary>
    /// <returns>Name of the weapons in equipment.</returns>
    public string[] GetWeaponsNames()
    {
        if (WeaponComponent() is null)
            return new[] { "", "" };

        var sNames = new string[2];
        sNames[0] = WeaponComponent().Info().Name().ToString();

        var sFirstWp = _equippedGuns[0].GetComponent<Weapon>().Info().Name().ToString();
        var sSecondWp = _equippedGuns[1].GetComponent<Weapon>().Info().Name().ToString();
        if (sFirstWp == sNames[0])
            sNames[1] = sSecondWp;
        else
            sNames[1] = sFirstWp;
        return sNames;
    }
    
    #endregion Accessors
}