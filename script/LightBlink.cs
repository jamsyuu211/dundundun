using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightBlink : MonoBehaviour
{
    Light2D lightScript;
    bool isMax = false;
    // Start is called before the first frame update
    void Start()
    {
        lightScript = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(lightScript.falloffIntensity <= 0.6f)
        {
            if (isMax)
            {
                isMax = false;
            }
        }
        else if(lightScript.falloffIntensity >= 0.9f)
        {
            if (!isMax)
            {
                isMax = true;
            }
        }

        if (!isMax)
        {
            lightScript.falloffIntensity += Time.deltaTime * 0.2f;
        }
        else
        {
            lightScript.falloffIntensity -= Time.deltaTime * 0.2f;
        }
    }
}
