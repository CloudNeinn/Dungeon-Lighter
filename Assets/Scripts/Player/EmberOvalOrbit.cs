using UnityEngine;

public class EmberOvalOrbit : MonoBehaviour
{
    public static EmberOvalOrbit Instance;

    [Header("References")]
    [SerializeField] private Transform _target;

    [Header("Orbit")]
    [SerializeField] private Vector3 _centerOffset = new Vector3(0f, 1.2f, 0f);
    [SerializeField] private float _horizontalRadius = 0.6f;
    [SerializeField] private float _verticalRadius = 0.25f;
    [SerializeField] private float _emberSpeed = 2f;
    [SerializeField] private Vector3 _orbitRotationEuler = new Vector3(0f, 0f, 45f);
    [SerializeField] private float _orbitRotationSpeed;

    [Header("Options")]
    [SerializeField] private bool _faceMovement = false;
    [SerializeField] private float _emberGap = 0.4f;

    [Header("Gizmos")]
    [SerializeField] private bool _drawGizmos = true;
    [SerializeField] private int _gizmoSegments = 32;
    [SerializeField] private Color _gizmoColor = new Color(1f, 0.5f, 0.1f, 0.9f);

    private float _timePeriod;
    private Vector3 _orbitCenter;
    private Quaternion _orbitRotation;
    private Vector3[] _lastPositions;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (EmberController.Instance.embers != null)
            _lastPositions = new Vector3[EmberController.Instance.embers.Length];
    }

    void Update()
    {
        if (_target == null || EmberController.Instance.embers == null || EmberController.Instance.embers.Length == 0)
            return;

        _timePeriod += Time.deltaTime * _emberSpeed;
        _orbitRotationEuler.z += Time.deltaTime * _orbitRotationSpeed;

        _orbitCenter = _target.position + _centerOffset;
        _orbitRotation = Quaternion.Euler(_orbitRotationEuler);

        if (_lastPositions == null || _lastPositions.Length != EmberController.Instance.embers.Length)
            _lastPositions = new Vector3[EmberController.Instance.embers.Length];

        for (int i = 0; i < EmberController.Instance.embers.Length; i++)
        {
            if (EmberController.Instance.embers[i] == null)
                continue;

            float emberTime = _timePeriod + (i * _emberGap);

            Vector3 localOffset = new Vector3(
                Mathf.Cos(emberTime) * _horizontalRadius,
                Mathf.Sin(emberTime) * _verticalRadius,
                0f
            );

            Vector3 worldPosition = _orbitCenter + _orbitRotation * localOffset;

            Transform emberTransform = EmberController.Instance.embers[i].transform;
            Vector3 delta = worldPosition - emberTransform.position;

            emberTransform.position = worldPosition;

            if (_faceMovement && delta.sqrMagnitude > 0.0001f)
                emberTransform.right = delta.normalized;

            _lastPositions[i] = emberTransform.position;
        }
    }

    void OnDrawGizmos()
    {
        if (!_drawGizmos || _target == null || _gizmoSegments < 3)
            return;

        Gizmos.color = _gizmoColor;

        Vector3 orbitCenter = _target.position + _centerOffset;
        Quaternion orbitRotation = Quaternion.Euler(_orbitRotationEuler);

        Vector3 prevPoint = orbitCenter + orbitRotation * new Vector3(_horizontalRadius, 0f, 0f);

        for (int i = 1; i <= _gizmoSegments; i++)
        {
            float angle = (i / (float)_gizmoSegments) * Mathf.PI * 2f;

            Vector3 localPoint = new Vector3(
                Mathf.Cos(angle) * _horizontalRadius,
                Mathf.Sin(angle) * _verticalRadius,
                0f
            );

            Vector3 worldPoint = orbitCenter + orbitRotation * localPoint;
            Gizmos.DrawLine(prevPoint, worldPoint);
            prevPoint = worldPoint;
        }

        Gizmos.DrawSphere(orbitCenter, 0.03f);

        if (EmberController.Instance != null && EmberController.Instance.embers != null)
        {
            for (int i = 0; i < EmberController.Instance.embers.Length; i++)
            {
                float emberTime = _timePeriod - (i * _emberGap);

                Vector3 localOffset = new Vector3(
                    Mathf.Cos(emberTime) * _horizontalRadius,
                    Mathf.Sin(emberTime) * _verticalRadius,
                    0f
                );

                Vector3 worldPoint = orbitCenter + orbitRotation * localOffset;
                Gizmos.DrawSphere(worldPoint, 0.05f);
            }
        }
    }
}