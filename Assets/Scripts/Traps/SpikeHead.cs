using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeHead : MonoBehaviour
{
    enum SpikeHeadDirection
    {
        Up,
        Down,
        Left,
        Right

    }
    static readonly Vector3[] vectorDir = new Vector3[] {
        Vector3.up,
        Vector3.down,
        Vector3.left,
        Vector3.right
    };

    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _raycastLayer;
    [SerializeField] private SpikeHeadDirection _movementDirection; 
    [SerializeField] private float _attackSpeed = 5f;
    [SerializeField] private float _returnSpeed = 1f;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private BoxCollider2D _collider;
    private bool _ready = true;
    private Vector3 _initialPosition;
    [SerializeField] private float _cooldownTime = 2f;
    private float _cooldownTimeCounter = 0f;

    void Start()
    {  
       _initialPosition = transform.position; 
       _cooldownTimeCounter = _cooldownTime;
    }

    void Update()
    {
        if(playerInSight() && _ready) Attack();
        if(checkIfCollidingWithWall()) _ready = false;

        if(!_ready && _cooldownTimeCounter > 0f)
        {
            _cooldownTimeCounter -= Time.deltaTime;
        }
        else if(!_ready && _cooldownTimeCounter <= 0f)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            if (Vector3.Distance(transform.position, _initialPosition) > .1f)
            {
                _rigidbody.linearVelocity = -vectorDir[(int)_movementDirection] * _returnSpeed;
            }
            else
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _ready = true;
                _cooldownTimeCounter = _cooldownTime;
            }
        }
    }

    void Attack()
    {
        _rigidbody.linearVelocity = vectorDir[(int)_movementDirection] * _attackSpeed;
    }

    bool playerInSight()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, vectorDir[(int)_movementDirection], 20f, _raycastLayer);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }
        else return false;
    }

    bool checkIfCollidingWithWall()
    {
        return Physics2D.OverlapBox(transform.position + vectorDir[(int)_movementDirection] * .02f, _collider.bounds.size, 0f, _raycastLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, vectorDir[(int)_movementDirection] * 20f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + vectorDir[(int)_movementDirection] * .02f, _collider.bounds.size);
    }
}
