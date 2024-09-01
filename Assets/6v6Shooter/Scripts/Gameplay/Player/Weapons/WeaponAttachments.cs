using UnityEngine;

[CreateAssetMenu(fileName = "WeaponAttachments", menuName = "ScriptableObjects/WeaponAttachments")]
public class WeaponAttachments : ScriptableObject
{
    public WeaponInfo.WeaponName weaponName;

    [Tooltip("Weapon mags attachments.")]
    public GameObject[] mags;

    [Tooltip("Weapon barrels attachments.")]
    public GameObject[] barrels;

    [Tooltip("Weapon under barrels attachments.")]
    public GameObject[] underBarrels;

    [Tooltip("Weapon sights attachments.")]
    public GameObject[] sights;

    [Tooltip("Weapon stocks attachments.")]
    public GameObject[] stocks;

    [Tooltip("Weapon stands attachments (snipers only).")]
    public GameObject[] stands;
}
