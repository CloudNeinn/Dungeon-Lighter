using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLight : LightFlickering
{
    [SerializeField] private bool _burn;
    [SerializeField] private float _burningTime;
    [SerializeField] private float _burningTimeCounter;

    void Update()
    {
        Flicker();
        if(_burn) Burn();
    }

    void Burn()
    {
        if(_burningTimeCounter > 0 && SceneLoading.Instance.currentSceneType == SceneLoading.SceneType.Level) _burningTimeCounter -= Time.deltaTime;
        else if(SceneLoading.Instance.currentSceneType == SceneLoading.SceneType.Level && PlayerController.Instance.GetIsAlive()) 
            PlayerController.Instance.Death();
    }

    protected override void Flicker()
    {
        _flicker = Mathf.Sin((Time.time + _timeOffset) * _flickerSpeed) + PlayerController.Instance.GetFlickerSpeedModifier();
        _light2D.intensity = Mathf.Lerp(_minIntensity, _maxIntensity, (_flicker + 1f) / 2f) * _burningTimeCounter / _burningTime;
    }    
}
