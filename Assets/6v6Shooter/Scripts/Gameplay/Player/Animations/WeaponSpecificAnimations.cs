using UnityEngine;


public class WeaponSpecificAnimations : MonoBehaviour
{
    [SerializeField] private Animator anim;

    [SerializeField] private RuntimeAnimatorController scarController;

    [SerializeField] private RuntimeAnimatorController m4a1Controller;

    [SerializeField] private RuntimeAnimatorController dsr50Controller;
    
    [SerializeField] private RuntimeAnimatorController tac45Controller;
    
    [SerializeField] private RuntimeAnimatorController fnFiveController;
    
    [SerializeField] private RuntimeAnimatorController vector500Controller;
    
    [SerializeField] private RuntimeAnimatorController vel308Controller;
    
    [SerializeField] private RuntimeAnimatorController balistaVortexController;

    public void ChangeAnimations(WeaponInfo.WeaponName n)
    {
        switch (n)
        {
            case WeaponInfo.WeaponName.ScarEnforcer557:
                ChangeWeaponAnimations(scarController);
                break;
            case WeaponInfo.WeaponName.M4A1Sentinel254:
                ChangeWeaponAnimations(m4a1Controller);
                break;
            case WeaponInfo.WeaponName.Dsr50:
                ChangeWeaponAnimations(dsr50Controller);
                break;
            case WeaponInfo.WeaponName.Tac45:
                ChangeWeaponAnimations(tac45Controller);
                break;
            case WeaponInfo.WeaponName.FnFive8:
                ChangeWeaponAnimations(fnFiveController);
                break;
            case WeaponInfo.WeaponName.VectorGhost500:
                ChangeWeaponAnimations(vector500Controller);
                break;
            case WeaponInfo.WeaponName.VelIronclad308:
                ChangeWeaponAnimations(vel308Controller);
                break;
            case WeaponInfo.WeaponName.BalistaVortex:
                ChangeWeaponAnimations(balistaVortexController);
                break;
        }
    }

    private void ChangeWeaponAnimations(RuntimeAnimatorController controller)
    {
        anim.runtimeAnimatorController = controller;
    }
}