using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MM_CinemaCC : MonoBehaviour
{
    private Animator animator;

    [Header("SCENE OBJECT CAMERA POSITIONS")]
    public CinemachineVirtualCamera mainVC;
    public CinemachineVirtualCamera modeVC;
    public CinemachineVirtualCamera workbenchVC;
    public CinemachineVirtualCamera weaponDisplayVC;
    public CinemachineVirtualCamera settingsVC;
    public CinemachineVirtualCamera shopVC;
    public CinemachineVirtualCamera exitVC;

    [Header("WEAPON CONTAINER CAMERA POSITIONS")]
    public CinemachineVirtualCamera pistolVC;
    public CinemachineVirtualCamera submachineVC;
    public CinemachineVirtualCamera assaultVC;
    public CinemachineVirtualCamera sniperVC;
    public CinemachineVirtualCamera shotgunVC;

    public void SetCameraPriority(string cameraName)
    {
        mainVC.Priority = 0;
        modeVC.Priority = 0;
        workbenchVC.Priority = 0;
        settingsVC.Priority = 0;
        shopVC.Priority = 0;
        exitVC.Priority = 0;
        weaponDisplayVC.Priority = 0;

        pistolVC.Priority = 0;
        submachineVC.Priority = 0;
        assaultVC.Priority = 0;
        sniperVC.Priority = 0;
        shotgunVC.Priority = 0;

        switch (cameraName.ToLower())
        {
            case "main":
                mainVC.Priority = 1;
                break;
            case "mode":
                modeVC.Priority = 1;
                break;
            case "workbench":
                workbenchVC.Priority = 1;
                break;
            case "weapondisplay":
                weaponDisplayVC.Priority = 1;
                break;
            case "settings":
                settingsVC.Priority = 1;
                break;
            case "shop":
                shopVC.Priority = 1;
                break;
            case "exit":
                exitVC.Priority = 1;
                break;
            case "pistolcontainer":
                pistolVC.Priority = 1;
                break;
            case "submachinecontainer":
                submachineVC.Priority = 1;
                break;
            case "assaultcontainer":
                assaultVC.Priority = 1;
                break;
            case "snipercontainer":
                sniperVC.Priority = 1;
                break;
            case "shotguncontainer":
                shotgunVC.Priority = 1;
                break;
            default:
                Debug.LogError("Invalid camera name: " + cameraName);
                break;
        }
    }
}
