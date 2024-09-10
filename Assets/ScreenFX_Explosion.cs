using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenFX_Explosion : MonoBehaviour
{
    CinemachineImpulseSource impulseSrc;

    void Awake(){
        impulseSrc = GetComponent<CinemachineImpulseSource>();
    }
    void OnEnable(){
        impulseSrc.GenerateImpulse();
    }
}
