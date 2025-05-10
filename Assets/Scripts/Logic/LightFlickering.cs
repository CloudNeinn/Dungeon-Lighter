using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightFlickering : MonoBehaviour
{
    protected UnityEngine.Rendering.Universal.Light2D _light2D;
    [SerializeField] protected float _flickerSpeed = 0.1f; // Speed of flickering
    [SerializeField] protected float _minIntensity = 0.5f; // Minimum intensity of the light
    [SerializeField] protected float _maxIntensity = 2f; // Maximum intensity of the light
    protected float _timeOffset;
    protected float _flicker;

    void Start()
    {
        _light2D = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        _timeOffset = Random.Range(0f, 100f); // Give each light a random starting point
    }

    void Update()
    {
        Flicker();
    }

    protected virtual void Flicker()
    {
        _flicker = Mathf.Sin((Time.time + _timeOffset) * _flickerSpeed);
        _light2D.intensity = Mathf.Lerp(_minIntensity, _maxIntensity, (_flicker + 1f) / 2f); // Normalize to 0-1
    }     
}
