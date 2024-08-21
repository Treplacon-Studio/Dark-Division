using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MM_CinemaCC : MonoBehaviour
{

    private Animator animator;
    public CinemachineVirtualCamera mainVC;
    public CinemachineVirtualCamera modeVC;
    public CinemachineVirtualCamera workbenchVC;
    public CinemachineVirtualCamera settingsVC;
    public CinemachineVirtualCamera shopVC;
    public CinemachineVirtualCamera exitVC;

    public void SetCameraPriority(string cameraName)
    {
        mainVC.Priority = 0;
        modeVC.Priority = 0;
        workbenchVC.Priority = 0;
        settingsVC.Priority = 0;
        shopVC.Priority = 0;
        exitVC.Priority = 0;

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
            case "settings":
                settingsVC.Priority = 1;
                break;
            case "shop":
                shopVC.Priority = 1;
                break;
            case "exit":
                exitVC.Priority = 1;
                break;
            default:
                Debug.LogError("Invalid camera name: " + cameraName);
                break;
        }
    }
}
