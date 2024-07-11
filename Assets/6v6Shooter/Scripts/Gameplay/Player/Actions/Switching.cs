using Photon.Pun;
using UnityEngine;


public class Switching : MonoBehaviour
{
    [SerializeField] private PlayerNetworkController pnc;
    
    [SerializeField] [Tooltip("Component holder to access components.")]
    private ComponentHolder componentHolder;
    
    [SerializeField] [Tooltip("Socket where the gun is kept.")]
    private GameObject gunSocket;
    
    private GameObject[] equippedGuns;
    private GameObject _weapon;
    private Transform _bulletStartPoint;
    private float _nextFireTime;
    private int _gunInHandsIndex;
    private bool _weaponInitialized;

    private void Awake()
    {
        ActionsManager.GetInstance(pnc.GetInstanceID()).Switching = this;
    }

    public void SetNewEquipment(string[] weapons, int[,] attachments)
    {
        _gunInHandsIndex = 0;
        
        if(equippedGuns is not null)
        { 
            foreach (var gun in equippedGuns)
            {
                gun.SetActive(false);
                gun.transform.SetParent(null);
                DestroyImmediate(gun, true);
            }
        }
        
        equippedGuns = new GameObject[weapons.Length];
        componentHolder.bulletPoolingManager.ClearPools();
        
        for (var index = 0; index < equippedGuns.Length; index++)
        {
            equippedGuns[index] = PhotonNetwork.Instantiate(weapons[index],
                gunSocket.transform.position,
                gunSocket.transform.rotation * Quaternion.Euler(90, 0, 0));
            equippedGuns[index].transform.parent = gunSocket.transform;
            
            equippedGuns[index].GetComponent<Weapon>().ApplyAttachmentsAssaultRifle(attachments, index);
            if (equippedGuns[index] != null)
                equippedGuns[index].SetActive(false);
            else
                Debug.LogError("Equipped gun is null!");
            equippedGuns[index].transform.SetParent(gunSocket.transform);
            equippedGuns[index].transform.localPosition = Vector3.zero;
            equippedGuns[index].transform.localScale = Vector3.one * 0.01f;
            var mg = equippedGuns[index].GetComponent<Weapon>().GetMag().GetComponent<Mag>();
            componentHolder.bulletPoolingManager.AddPool(new BulletPoolingManager.Pool(index, mg.ammoType, mg.size));
        }

        componentHolder.bulletPoolingManager.ApplyPools();
        SwitchWeapon(_gunInHandsIndex);
        _weaponInitialized = true;
    }

    private void SwitchWeapon(int wn)
    {
        _gunInHandsIndex = wn;
        foreach (var w in equippedGuns)
            w.SetActive(false);
        componentHolder.weaponSpecificAnimations.ChangeAnimations(equippedGuns[wn].GetComponent<Weapon>().Info().Name());
        _weapon = equippedGuns[wn];
        _weapon.SetActive(true);
    }

    public void Run()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");

        if (componentHolder.playerAnimationController.reloadingLock)
            return;

        if (scroll > 0f) //Scrolled up
        {
            if (_gunInHandsIndex == 0)
                SwitchWeapon(equippedGuns.Length - 1);
            else
                SwitchWeapon(_gunInHandsIndex - 1);
        }
        else if (scroll < 0f) //Scrolled down
        {
            SwitchWeapon((_gunInHandsIndex + 1) % equippedGuns.Length);
        }
    }

    public Weapon WeaponComponent()
    {
        if (equippedGuns == null)
            return null;
        
        if (_weapon == null)
            _weapon = equippedGuns[_gunInHandsIndex];

        if (_weapon == null)
            return null;

        return _weapon.GetComponent<Weapon>();
    }

    public int GetCurrentWeaponID()
    {
        return _gunInHandsIndex;
    }

    public string[] GetWeaponsNames()
    {
        if (WeaponComponent() is null)
            return new[] { "", "" };

        var names = new string[2];
        names[0] = WeaponComponent().Info().Name().ToString();

        var firstWp = equippedGuns[0].GetComponent<Weapon>().Info().Name().ToString();
        var secondWp = equippedGuns[1].GetComponent<Weapon>().Info().Name().ToString();
        if (firstWp == names[0])
            names[1] = secondWp;
        else
            names[1] = firstWp;
        return names;
    }
}