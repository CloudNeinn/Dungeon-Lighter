using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Frog : MonoBehaviour
{
    [SerializeField] private float _currentSightAngle;
    [SerializeField] private float _targetSightAngle;
    [SerializeField] private int _rotationSpeed;
    [SerializeField] private GameObject[] _lookAtObjects;
    [SerializeField] private bool _cycle;          // false = loop (default), true = ping-pong cycle
    [SerializeField] private Light2D[] _lights;
    [SerializeField] private GameObject[] _lightObjects;
    [SerializeField] private float _lookTime;
    [SerializeField] private float _lookTimeCounter;
    [SerializeField] private float _visionTime;
    [SerializeField] private float _visionTimeCounter;
    [SerializeField] private float _visionTimeAdvancementSpeed;
    [SerializeField] private float _visionTimeRetractionSpeed;
    [SerializeField] private int _sceneIndex;
    [SerializeField] private float _maxLightIntensity;
    private float _initialLightIntensity;

    [Header("Sight")]
    [SerializeField] private float _sightDistance = 10f;
    [SerializeField] private int _rayCount = 9;
    [SerializeField] private float _fovAngle = 45f;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private LayerMask _playerMask;
    private float _originalSpread;
    private bool _effectApplied;
    private int _currentTargetIndex = 0;
    private int _cycleDirection = 1;
    private GameObject _target;
    private int _next;

    void Start()
    {
        _initialLightIntensity = _lights[0].intensity;
        _originalSpread = _lights[0].pointLightInnerAngle;
        _effectApplied = false;
        if (_lightObjects != null)
            _currentSightAngle = _lightObjects[0].transform.eulerAngles.z;

        UpdateTargetAngle();

        _visionTimeCounter = _visionTime;
    }

    void Update()
    {
        ChangeVisionSpread();
        if (_lookAtObjects != null && _lookAtObjects.Length > 0)
        {
            UpdateTargetAngle();
            _currentSightAngle = Mathf.MoveTowardsAngle(
                _currentSightAngle,
                _targetSightAngle,
                _rotationSpeed * Time.deltaTime);

            if (Mathf.Abs(Mathf.DeltaAngle(_currentSightAngle, _targetSightAngle)) < 0.05f)
            {
                _lookTimeCounter += Time.deltaTime;
                if (_lookTimeCounter >= _lookTime)
                {
                    _lookTimeCounter = 0f;
                    AdvanceTarget();
                }
            }
            else
            {
                _lookTimeCounter = 0f;
            }
        }

        if (_lightObjects != null)
        {
            foreach (var lightObject in _lightObjects)
            {
                lightObject.transform.rotation = Quaternion.Euler(0, 0, _currentSightAngle);
            }
        }

        if (PlayerInSight())
        {
            UpdateTargetAngle();
            if(_visionTimeCounter > 0) _visionTimeCounter -= Time.deltaTime * _visionTimeAdvancementSpeed;
        }
        else if(_visionTimeCounter < _visionTime)
        {
            _visionTimeCounter += Time.deltaTime * _visionTimeRetractionSpeed;
        }

        if(_visionTimeCounter <= 0 && !_effectApplied)
        {
            _effectApplied = true;
            ApplyEffect();
        }

        float newIntensity = Mathf.Clamp(_initialLightIntensity * (_visionTime / (_visionTimeCounter <= 0 ? 0.01f : _visionTimeCounter)), _initialLightIntensity, _maxLightIntensity);
        foreach (var light in _lights)
        {
            light.intensity = newIntensity;
        }
    }

    void ChangeVisionSpread()
    {
        float spread = Mathf.Clamp(_originalSpread * (_visionTimeCounter/_visionTime), 1f, _originalSpread);
        foreach (var light in _lights)
        {
            light.pointLightInnerAngle = spread;
            light.pointLightOuterAngle = spread;
        }
    }

    void ApplyEffect()
    {
        SceneLoading.Instance.LoadScene(_sceneIndex, false, false);
    }

    void UpdateTargetAngle()
    {
        if (_lookAtObjects == null || _lookAtObjects.Length == 0) return;
        if(PlayerInSight()) _target = PlayerController.Instance.gameObject;
        else _target = _lookAtObjects[_currentTargetIndex];
        if (_target == null) return;

        _targetSightAngle = VectorToRotation((_target.transform.position - transform.position).normalized);
    }

    void AdvanceTarget()
    {
        if (_lookAtObjects.Length <= 1) return;

        if (_cycle)
        {
            _next = _currentTargetIndex + _cycleDirection;
            if (_next >= _lookAtObjects.Length || _next < 0)
            {
                _cycleDirection = -_cycleDirection;
                _next = _currentTargetIndex + _cycleDirection;
            }
            _currentTargetIndex = _next;
        }
        else
        {
            _currentTargetIndex = (_currentTargetIndex + 1) % _lookAtObjects.Length;
        }
    }

    void ChangeSightDirection()
    {

    }

    bool PlayerInSight()
    {
        _fovAngle = _lights[0].pointLightInnerAngle;
        float half = _fovAngle * 0.5f;
        LayerMask combined = _obstacleMask | _playerMask;

        for (int i = 0; i < _rayCount; i++)
        {
            float t = _rayCount == 1 ? 0.5f : (float)i / (_rayCount - 1);
            float angle = _currentSightAngle - half + t * _fovAngle;
            Vector2 dir = RotationToVector(angle);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, _sightDistance, combined);
            if (hit.collider != null && ((1 << hit.collider.gameObject.layer) & _playerMask) != 0)
                return true;
        }
        return false;
    }

    Vector2 RotationToVector(float degrees)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, degrees);
        Vector2 v = rotation * Vector3.up;
        return v;
    }

    float VectorToRotation(Vector2 dir)
    {
        float deg = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
        return Mathf.Repeat(deg, 360f);
    }

    void OnDrawGizmos()
    {
        LayerMask combined = _obstacleMask | _playerMask;
        float half = _fovAngle * 0.5f;

        for (int i = 0; i < _rayCount; i++)
        {
            float t = _rayCount == 1 ? 0.5f : (float)i / (_rayCount - 1);
            float angle = _currentSightAngle - half + t * _fovAngle;
            Vector2 dir = RotationToVector(angle);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, _sightDistance, combined);
            float dist = hit.collider != null ? hit.distance : _sightDistance;

            bool sawPlayer = hit.collider != null
                && ((1 << hit.collider.gameObject.layer) & _playerMask) != 0;

            Gizmos.color = sawPlayer ? Color.green : Color.yellow;
            Gizmos.DrawRay(transform.position, dir * dist);
        }

        // Center sight ray, clipped to obstacles only
        Vector2 centerDir = RotationToVector(_currentSightAngle);
        RaycastHit2D centerHit = Physics2D.Raycast(transform.position, centerDir, _sightDistance, _obstacleMask);
        float centerDist = centerHit.collider != null ? centerHit.distance : _sightDistance;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, centerDir * centerDist);

        // Target indicator
        Vector2 targetDir = RotationToVector(_targetSightAngle);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, targetDir * _sightDistance);

        if (_lookAtObjects != null)
        {
            foreach (var obj in _lookAtObjects)
            {
                if (obj == null) continue;
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(obj.transform.position, 2f);
            }
        }
    }
}