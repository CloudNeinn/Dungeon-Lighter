using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightFlickering : MonoBehaviour
{
    private UnityEngine.Rendering.Universal.Light2D light2D;
    [SerializeField] private float flickerSpeed = 0.1f; // Speed of flickering
    [SerializeField] private float minIntensity = 0.5f; // Minimum intensity of the light
    [SerializeField] private float maxIntensity = 2f; // Maximum intensity of the light
    private float timeOffset;

    void Start()
    {
        light2D = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        timeOffset = Random.Range(0f, 100f); // Give each light a random starting point
    }

    void Update()
    {
        float flicker = Mathf.Sin((Time.time + timeOffset) * flickerSpeed);
        light2D.intensity = Mathf.Lerp(minIntensity, maxIntensity, (flicker + 1f) / 2f); // Normalize to 0-1
    }
}
