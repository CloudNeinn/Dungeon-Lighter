using UnityEngine;

public class DartTrap : MonoBehaviour
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private GameObject _trigger;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _shootDelayTimer;
    private float _shootDelayCooldown;
    [SerializeField] private bool _Vertical;
    [SerializeField] private Vector2 _shootDirection;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private Vector2 _triggerBoxSize;
    [SerializeField] private Vector2 _triggerBoxOffset;
    private bool _wasTriggered;
    private bool _isShootPending;
    
    void Start()
    {
        if(_Vertical) 
        {
            _shootDirection = Vector2.up * 0.9f;
            if(Mathf.Sign(transform.localScale.y) == 1) transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, -90);
            else transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 90);
        }
        else _shootDirection = Vector2.right * 0.9f;
    }

    void Update()
    {
        if(Triggered() && !_wasTriggered && !_isShootPending) 
        {
            _isShootPending = true;
            _wasTriggered = true;
            _shootDelayCooldown = _shootDelayTimer;
        }
        else if(!Triggered() && _wasTriggered) _wasTriggered = false;

        if(_isShootPending)
        {
            _shootDelayCooldown -= Time.deltaTime;
            if(_shootDelayCooldown <= 0f)
            {
                Shoot();
                _isShootPending = false;
            }
        }
    }

    bool Triggered()
    {
        return Physics2D.OverlapBox((Vector2)_trigger.transform.position + new Vector2(_triggerBoxOffset.x * _trigger.transform.localScale.x, _triggerBoxOffset.y), _triggerBoxSize, 0, _playerLayer);
    }

    void Shoot()
    {
        GameObject spawnedObject = ObjectPoolManager.SpawnObject(_projectile, new Vector3(transform.position.x + _shootDirection.x * Mathf.Sign(transform.localScale.x),
        transform.position.y + _shootDirection.y * Mathf.Sign(transform.localScale.y), transform.position.z), Quaternion.identity, ObjectPoolManager.PoolType.TrapProjectileObjects);
        if(_Vertical) spawnedObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * Mathf.Sign(transform.localScale.y) * _projectileSpeed;
        else spawnedObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.right * Mathf.Sign(transform.localScale.x) * _projectileSpeed;
    } 

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)_trigger.transform.position + new Vector2(_triggerBoxOffset.x * _trigger.transform.localScale.x, _triggerBoxOffset.y), _triggerBoxSize);
    
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_trigger.transform.position, transform.position);
    }
}
