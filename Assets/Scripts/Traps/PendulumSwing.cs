using UnityEngine;

public class PendulumSwing : MonoBehaviour
{
    [SerializeField] private float _pushForce;
    [SerializeField] private GameObject _ball;
    private Rigidbody2D _ballRigidbody;
    private Transform _ballTransform;
    [SerializeField] private Transform _anchor;
    [SerializeField] private bool _activated;
    [SerializeField] private AudioSource _activationAudio;
    [SerializeField] private bool _audioPlayed;
    private float _previousOffsetX;
    private float _currentOffsetX;
    private bool _crossedCenter;
    private bool _hasPushedThisPass;
    private Vector2 _swingDir;

    void Start()
    {
        _ballRigidbody = _ball.GetComponent<Rigidbody2D>();
        _ballTransform = _ball.GetComponent<Transform>();
        _previousOffsetX = _ballTransform.position.x - _anchor.position.x;
    }

    void Update()
    {
        if(_activated) 
        {
            _ballRigidbody.bodyType = RigidbodyType2D.Dynamic;
            if(!_audioPlayed)
            {
                _activationAudio.Play();
                _audioPlayed = true;
            }
        }

        _currentOffsetX = _ballTransform.position.x - _anchor.position.x;
        _crossedCenter = Mathf.Sign(_currentOffsetX) != Mathf.Sign(_previousOffsetX);

        if (_crossedCenter && !_hasPushedThisPass)
        {
            Push();
            _hasPushedThisPass = true;
        }
        else if (!_crossedCenter)
        {
            _hasPushedThisPass = false;
        }

        _previousOffsetX = _currentOffsetX;
    }

    void Push()
    {
        _swingDir = new Vector2(Mathf.Sign(_ballRigidbody.linearVelocity.x), 0f);
        _ballRigidbody.AddForce(_swingDir * _pushForce, ForceMode2D.Impulse);
    }
}