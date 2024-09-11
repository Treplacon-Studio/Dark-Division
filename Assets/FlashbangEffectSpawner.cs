using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlashbangEffectSpawner : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] float delayTime;
    
    void OnEnable()
    {
        Invoke(nameof(SpawnEffect), delayTime);
    }

    void SpawnEffect(){
    var go = Instantiate(prefab);
        var bangSettings = go.GetComponent<FlashbangEffectHandler>();
        Destroy(go, bangSettings.Lifetime);
    
    }
   
}
