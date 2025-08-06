using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FireTrap : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private float _fireTimer;
    [SerializeField] private float _fireTimerCooldown;
    [SerializeField] private bool _startTimer;
    [SerializeField] private bool _activateLight;
    [SerializeField] private Light2D _light;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _light = GetComponentInChildren<Light2D>();
        if (_fireTimerCooldown == 0) ResetTimer();
    }

    void Update()
    {
        if (_startTimer)
        {
            _fireTimerCooldown -= Time.deltaTime;
        }

        if (_fireTimerCooldown <= 0)
        {
            _startTimer = false;
            _animator.SetTrigger("Fire");
            ResetTimer();
        }

        if (_activateLight) _light.enabled = true;
        else _light.enabled = false;
    }

    void ResetTimer()
    {
        _fireTimerCooldown = _fireTimer;
    }
    
    
}
