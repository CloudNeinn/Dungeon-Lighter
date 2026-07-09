using UnityEngine;

public class SpikePlatformTrap : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _rotationAngle;
    [SerializeField] private float _rotationCooldown;

    private float _rotationCooldownTimer;
    private float _rotationProgress;
    private bool _isRotating;

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        if (_isRotating)
        {
            Rotate();
        }
        else
        {
            _rotationCooldownTimer -= Time.deltaTime;
            if (_rotationCooldownTimer <= 0)
            {
                _isRotating = true;
                _rotationProgress = 0f;
            }
        }
    }

    void Rotate()
    {
        float step = _rotationSpeed * Time.deltaTime;
        
        if (_rotationProgress + step >= _rotationAngle)
        {
            step = _rotationAngle - _rotationProgress;
            _isRotating = false;
            ResetTimer();
        }

        transform.Rotate(new Vector3(0, 0, step));
        _rotationProgress += step;
    }

    void ResetTimer()
    {
        _rotationCooldownTimer = _rotationCooldown;
    }
}