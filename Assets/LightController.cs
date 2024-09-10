using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{

    public const float MAX_LUMINENCE = 100.0f;
    public new Light light;
    public float fadeSpeed = 5.0f;
    float multiplier ;
    public float duration = 10.0f;
    float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        multiplier = MAX_LUMINENCE;
    }

    // Update is called once per frame
    void Update()
    {   
        if(time > duration)
        {
            light.intensity = 0;
            return;
        }

        float intensity = Random.Range(1, 5) * multiplier;
        float t = time / duration;
        
        light.intensity = Mathf.Lerp(light.intensity, intensity, 10.0f * Time.deltaTime);
        multiplier = Mathf.Lerp(0, MAX_LUMINENCE, t);
        
        time += Time.deltaTime;
    }
}
