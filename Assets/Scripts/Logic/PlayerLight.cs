using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLight : LightFlickering
{
    [SerializeField] protected float burningTime;
    [SerializeField] protected float burningTimeCounter;
    private bool isDead;

    void Update()
    {
        Flicker();
        if(burningTimeCounter > 0 && SceneLoading.Instance.currentSceneType == SceneLoading.SceneType.Level) burningTimeCounter -= Time.deltaTime;
        else if(SceneLoading.Instance.currentSceneType == SceneLoading.SceneType.Level && !isDead)
        {
            isDead = true;
            PlayerControl.Instance.Death();
        }
    }

    protected override void Flicker()
    {
        _flicker = Mathf.Sin((Time.time + _timeOffset) * _flickerSpeed) + PlayerControl.Instance.getFlickerSpeedModifier();
        _light2D.intensity = Mathf.Lerp(_minIntensity, _maxIntensity, (_flicker + 1f) / 2f) * burningTimeCounter / burningTime;
    }    
}
