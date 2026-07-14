using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Slope : MonoBehaviour
{
    [SerializeField] private float _slideForceMultiplier = 1f;
    [SerializeField] private float _maxSlideSpeed;
    [SerializeField] private float _currentSlideSpeed;
    [SerializeField] private float _angle;
    [SerializeField] private float _slideForce;
    [SerializeField] private float _maxSlideForce;
    [SerializeField] private Vector2 _tangent;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _slopeClimbTime;
    [SerializeField] private float _slopeClimbTimeCounter;

    void Start()
    {
        _slopeClimbTimeCounter = _slopeClimbTime;
    }
    void FixedUpdate()
    {
        //if(PlayerController.Instance.IsTouchingSlope() && _slopeClimbTimeCounter < 0 && !PlayerController.Instance.isSliding) GoUpSlope();
        //else if(PlayerController.Instance.IsTouchingSlope()) _slopeClimbTimeCounter -= Time.fixedDeltaTime;
        if(PlayerController.Instance.isSliding && _slideForce < _maxSlideForce) _slideForce += _slideForceMultiplier * Time.fixedDeltaTime;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        //PlayerController.Instance.isSliding = false;
        _rigidbody = collision.rigidbody;
        if (_rigidbody == null) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            _currentSlideSpeed = PlayerController.Instance.PlayerRigidbody.linearVelocity.magnitude;
            _angle = Vector2.Angle(contact.normal, Vector2.up);
            if (_angle > 120f && _angle < 175f)
            {
                _tangent = new Vector2(contact.normal.y, -contact.normal.x);
                if (_tangent.y > 0) _tangent = -_tangent;
                //_tangent = Vector2.down;
                //_slideForce = Mathf.Sin(_angle * Mathf.Deg2Rad) * Physics2D.gravity.magnitude * _rigidbody.mass * _slideForceMultiplier;
                //_slideForce += _slideForceMultiplier * Time.fixedDeltaTime;
                //if(_currentSlideSpeed < _maxSlideSpeed) _rigidbody.AddForce(_tangent * _slideForce * (_currentSlideSpeed/_maxSlideSpeed));
                //if(_currentSlideSpeed < _maxSlideSpeed) _rigidbody.AddForce(Vector2.down * _slideForce, ForceMode2D.Impulse);
                if (_rigidbody != null && _rigidbody.linearVelocity.magnitude > _maxSlideSpeed)
                    _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * _maxSlideSpeed;
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController.Instance.isSliding = true;
        PlayerController.Instance.CanMove = false;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        _slideForce = 0;
        StartCoroutine(LeaveSlope());
    }

    void GoUpSlope()
    {
        PlayerController.Instance.isSliding = true;
        PlayerController.Instance.CanMove = false;
    }

    private IEnumerator LeaveSlope()
    {
        yield return new WaitForSeconds(_slopeClimbTime/4);
        PlayerController.Instance.isSliding = false;
        PlayerController.Instance.CanMove = true;
        _slopeClimbTimeCounter = _slopeClimbTime;
        
    }
}
