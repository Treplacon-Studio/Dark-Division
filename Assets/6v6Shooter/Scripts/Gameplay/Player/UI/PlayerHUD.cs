using UnityEngine;
using TMPro;

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

    private void Update()
    {
        UpdateWeaponBoxes();
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
}
