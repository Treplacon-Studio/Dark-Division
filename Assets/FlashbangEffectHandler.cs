using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FlashbangEffectHandler : MonoBehaviour
{
    Image image;
    Image cutout;

    [field: SerializeField] public float Lifetime {get; private set;}= 2.0f;
    [SerializeField] AnimationCurve curve2;

    [SerializeField] float speed = 4.0f;
    Vector2 targetSize = new Vector2(1920, 1080);
    
    void Start()
    { 
        image = transform.GetChild(0).GetComponent<Image>();
        cutout = transform.GetChild(1).GetComponent<Image>();

        StartCoroutine(StartFadeout(Lifetime));
        StartCoroutine(StartVisionTransition(Lifetime, 1.0f));
    }

    IEnumerator StartFadeout(float duration) {
       
        float time = 0;
        float t = 0;
        Color color = image.color; 

        while(t <= 1){
            
            time += Time.deltaTime;
            t = time / duration;
            
            float curve = 1 - Mathf.Pow(t, 5);  
            
            color = image.color;
            color.a = Mathf.Lerp(color.a, curve, speed * Time.deltaTime);
            
            image.color = color; 

            yield return null;
        }
        
        color.a = 0;
        image.color = color; 

    }

    IEnumerator StartVisionTransition(float duration, float delay){
        float time = 0;
        float t = 0;
        
        yield return new WaitForSeconds(delay);
        
        while(t <= 1)
        {
            time += Time.deltaTime;
            t = time / duration;

            cutout.rectTransform.sizeDelta = Vector2.Lerp(Vector2.zero, targetSize, curve2.Evaluate(t));
            
            yield return null;
        }
    }


}
