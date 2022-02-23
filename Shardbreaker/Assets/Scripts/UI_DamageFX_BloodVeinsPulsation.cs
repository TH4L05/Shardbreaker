using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class UI_DamageFX_BloodVeinsPulsation : MonoBehaviour
{
    Material material;
    RawImage image;
    [Range(0.1f,5f)]public float pulseDuration = 1f;
    [Range(0.1f, 5f)] public float pulseDelay = 1f;

    float time = 0;

    void Awake()
    {
        image = GetComponent<RawImage>();
        material = image.material;

        if(material == null)
        {
            Debug.LogWarning("Material missing on " + gameObject.name);
            return;
        }

        material.SetFloat("_Pulsation_Scale", 0);
        time = 0;
    }

    void Update()
    {
        //Update pulse
        if(time <= pulseDuration)
        {
            float pulseProgress = Mathf.Clamp(time / pulseDuration, 0, 1);
            material.SetFloat("_Pulsation_Scale", pulseProgress);
        }

        time += Time.deltaTime;

        //Reset Pulse
        if(time >= pulseDuration + pulseDelay)
        {
            time = 0;
        }
    }
}
