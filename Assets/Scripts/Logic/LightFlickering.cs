using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightFlickering : MonoBehaviour
{
    private UnityEngine.Rendering.Universal.Light2D _light2D;
    [SerializeField] private float _flickerSpeed = 0.1f; // Speed of flickering
    [SerializeField] private float _minIntensity = 0.5f; // Minimum intensity of the light
    [SerializeField] private float _maxIntensity = 2f; // Maximum intensity of the light
    private float _timeOffset;
    private float _flicker;

    void Start()
    {
        _light2D = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        _timeOffset = Random.Range(0f, 100f); // Give each light a random starting point
    }

    void Update()
    {
        _flicker = Mathf.Sin((Time.time + _timeOffset) * _flickerSpeed);
        _light2D.intensity = Mathf.Lerp(_minIntensity, _maxIntensity, (_flicker + 1f) / 2f); // Normalize to 0-1
    }
}
