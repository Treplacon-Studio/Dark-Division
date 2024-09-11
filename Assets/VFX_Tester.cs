using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class VFX_Tester : MonoBehaviour
{
    enum GrenadeType {
        Frag,               // has a 4 second fuse. then goes KABOOOM!
        Smoke,              
        Incidiary,
        Sticky, 
        Thermite,
        __COUNT__
    }


    [SerializeField] GameObject editorCamera;

    public List<GameObject> prefabs = new List<GameObject>();

    void OnValidate(){
        if(!prefabs.Any()){
            prefabs = new List<GameObject>((int)GrenadeType.__COUNT__);
        }
    }

    bool isActive = true;
    float timeout;
    
    void Update(){
    
        if(Input.GetKeyDown(KeyCode.P) && timeout <= 0){
            timeout = 0.75f;
            isActive = !isActive;
            editorCamera.SetActive(isActive);
        }
        
        if( timeout > 0){
            timeout -= Time.deltaTime;
        }
        if(timeout <0 ){
            timeout = 0;
        }

        List<KeyCode> acceptable_inputs = new List<KeyCode>(){ KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4,KeyCode.Alpha5 } ;

        for(int i = 0; i < acceptable_inputs.Count; i++){
            var inp = acceptable_inputs[i];
            
            if(Input.GetKeyDown(inp)){
                var type = (GrenadeType)i;
                
                if(type < GrenadeType.__COUNT__){
                    SpawnGrenade(type);
                }    
            }
        }
    }


    void SpawnGrenade(GrenadeType t){

        var prefab = prefabs[(int)t];
        if(prefab == null)
            return; // probably still working on an effect of this type.. 
        
        var go = Instantiate(prefab);
        
        go.transform.position = Vector3.zero;
        Destroy(go, 10);
    }

}
