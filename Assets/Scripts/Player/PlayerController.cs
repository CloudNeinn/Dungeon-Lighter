using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    #region Variables
    
    #region Components
    [Header("Components")]
    [SerializeField] private Rigidbody2D _playerRigidbody;
    [SerializeField] private CapsuleCollider2D _playerCollider;
    #endregion

    #region States
    [Header("States")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isMoving;
    [SerializeField] private bool _isWalled;
    [SerializeField] private bool _isSliding;
    [SerializeField] private bool _isWallSliding;
    [SerializeField] private bool _isJumping;
    [SerializeField] private bool _isFalling;
    [SerializeField] private bool _isAtApex;
    [SerializeField] private bool _isAlive;
    [SerializeField] public bool isFlare;
    #endregion

    #region Movement
    [Header("Movement Parameters")]
    [SerializeField] private Vector2 _movementVector;
    [SerializeField] private float _horizontalAcceleration;
    [SerializeField] private float _horizontalDeceleration;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _airSpeed;
    [SerializeField] private bool _canMove;
    [SerializeField] private float _moveCooldownTime;
    [SerializeField] private float _moveCooldownTimeCounter;
    [SerializeField] private float _currentSpeed;
    [SerializeField] private float _speedChangeRate;
    [SerializeField] private float _targetSpeed;
    [SerializeField] private float _speedDiff;
    #endregion

    #region Movement Supports
    [Header("Movement Supports")]
    [SerializeField] private float _edgeMoveAmount;
    [SerializeField] private float _ledgeMoveAmount;
    #endregion

    #region Jump
    [Header("Jump Parameters")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpCoyoteTime;
    [SerializeField] private float _jumpCoyoteTimeCounter;
    [SerializeField] private float _jumpBufferTime;
    [SerializeField] private float _jumpBufferTimeCounter;
    [SerializeField] private int _doubleJumpCount;
    [SerializeField] private int _totalDoubleJumpCount;
    [SerializeField] private float _wallJumpStrengthX;
    [SerializeField] private float _wallJumpStrengthY;
    [SerializeField] private float _jumpFallForce;
    [SerializeField] private float _jumpApexThreshold;
    [SerializeField] private float _apexSpeedMultiplier;
    [SerializeField] private float _apexGravityModifier;
    [SerializeField] private float _fallingGravityModifier;
    [SerializeField] private float _originalGravity;
    [SerializeField] private float _maxFallSpeed;
    [SerializeField] private float _maxWallSlideSpeed;
    private bool _jumpPressed;
    private bool _jumpReleased;
    #endregion

    #region Slide
    [Header("Slide Parameters")]
    [SerializeField] private float _slideSpeed;
    [SerializeField] private float _slideAccel;
    #endregion

    #region Hitboxes
    [Header("Hitboxes Parameters")]
    [SerializeField] private Vector2 _groundBoxSize;
    [SerializeField] private Vector2 _groundBoxOffset;
    [SerializeField] private Vector2 _wallBoxSize;
    [SerializeField] private Vector2 _wallBoxOffset;
    [SerializeField] private Vector2 _edgeDetectionBoxSize;
    [SerializeField] private Vector2 _leftEdgeDetectionBoxOffset;
    [SerializeField] private Vector2 _rightEdgeDetectionBoxOffset;
    [SerializeField] private Vector2 _ledgeDetectionBoxSize;
    [SerializeField] private Vector2 _ledgeDetectionBoxOffset;
    #endregion

    #region Layer Masks
    [Header("Layer Masks")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    #endregion

    #region GET/SET
    
    public float RunSpeed
    {
        get => _runSpeed;
        set => _runSpeed = value;
    }
    public Vector2 Velocity
    {
        get => _playerRigidbody.linearVelocity;
        set => _playerRigidbody.linearVelocity = value;
    }

    public int DoubleJumpCount
    {
        get => _doubleJumpCount;
        set => _doubleJumpCount = value;
    }

    public int TotalDoubleJumpCount
    {
        get => _totalDoubleJumpCount;
        set => _totalDoubleJumpCount = value;
    }

    public float GravityScale
    {
        get => _playerRigidbody.gravityScale;
        set => _playerRigidbody.gravityScale = value;
    }

    public Rigidbody2D PlayerRigidbody => _playerRigidbody;
    #endregion
    #endregion
    
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
        _currentSpeed = _walkSpeed;
        _isAlive = true;
        _originalGravity = _playerRigidbody.gravityScale;
        //_airSpeed = _runSpeed * 1.2f; 
    }


    void Update()
    {
        if(_isAlive) Rotate();

        _isGrounded = IsGrounded();
        _isWalled = IsWalled();
        _isMoving = IsMoving();

        if (_isWalled && !_isGrounded && Mathf.Abs(UserInput.Instance.moveInput.x) > 0 && _canMove)
        {
            _isWallSliding = true;
            //_playerRigidbody.linearVelocity = new Vector2(0, _playerRigidbody.linearVelocity.y);
        }
        else _isWallSliding = false;

        if (UserInput.Instance.jumpInput)
            _jumpPressed = true;
        
        if (UserInput.Instance.jumpReleased)
            _jumpReleased = true;
            
        if (Mathf.Abs(_playerRigidbody.linearVelocity.x) < 0.08f && UserInput.Instance.moveInput.x == 0)
            _playerRigidbody.linearVelocity = new Vector2(0, _playerRigidbody.linearVelocity.y);
    }

    void FixedUpdate()
    {
        if(_isJumping) EdgeSupport();

        if(_isJumping && _isMoving && !_isWalled && IsTouchingLedge()) PushUpLedge();
        
        if(_moveCooldownTimeCounter > 0) _moveCooldownTimeCounter -= Time.fixedDeltaTime;
        else if(_isAlive) _canMove = true;
        
        if(_canMove)
        {
            Movement();
            if(!isFlare) Jump();
        }

        if(!_isWallSliding) ClampFallSpeed();
        else ClampWallSlideSpeed();

        if(_isWallSliding) WallSlide();

        if (_canMove && !_isWallSliding)
        {
            if (Mathf.Abs(_playerRigidbody.linearVelocity.x) > _targetSpeed && _isGrounded)
            {
                _playerRigidbody.AddForce(Vector2.right * 
                    -(_playerRigidbody.linearVelocity.x - Mathf.Sign(_playerRigidbody.linearVelocity.x) 
                    * _targetSpeed) * _horizontalDeceleration, ForceMode2D.Force);
            }
            else if (Mathf.Abs(_playerRigidbody.linearVelocity.x) < _targetSpeed)
            {
                _playerRigidbody.AddForce(Vector2.right * UserInput.Instance.moveInput.x * _horizontalAcceleration, ForceMode2D.Force);
            }
        }

        if (Mathf.Sign(UserInput.Instance.moveInput.x) != Mathf.Sign(_playerRigidbody.linearVelocity.x) 
            && _targetSpeed != 0 && !_isWalled && _canMove && !isFlare)
        {
            _playerRigidbody.linearVelocity = new Vector2(0, _playerRigidbody.linearVelocity.y);
        }
            

        _jumpPressed = false;
        _jumpReleased = false;
    }

    void Movement()
    {
        if(_isGrounded)
        {
            if(UserInput.Instance.moveInput.x == 0) _targetSpeed = 0;
            else if(UserInput.Instance.runAction.IsPressed()) _targetSpeed = _runSpeed;
            else _targetSpeed = _walkSpeed;
        }
        else if(!_isGrounded && !_isWalled)
        {
            if(_isAtApex) _targetSpeed = _airSpeed * _apexSpeedMultiplier; 
            else _targetSpeed = _airSpeed; 
        }
        else if(_isWalled)
        {
            _targetSpeed = _walkSpeed;
        }

        if(_currentSpeed != _targetSpeed && !_isAtApex)
        {
            _speedDiff = _targetSpeed - _currentSpeed;
            _currentSpeed = Mathf.Clamp(_currentSpeed += _speedChangeRate * Time.fixedDeltaTime  * Mathf.Sign(_speedDiff), 0, _runSpeed);
            if(Mathf.Abs(_speedDiff) < 0.01f) _currentSpeed = _targetSpeed;
        }
    }

    void Jump()
    {
        float force = _jumpForce;

        if (_isGrounded || _isWallSliding) 
        {
            _jumpCoyoteTimeCounter = _jumpCoyoteTime;
            _doubleJumpCount = _totalDoubleJumpCount;
        }
        else _jumpCoyoteTimeCounter -= Time.fixedDeltaTime;

        if (_jumpPressed && !_isWallSliding) 
            _jumpBufferTimeCounter = _jumpBufferTime;

        if ((_jumpBufferTimeCounter > 0) && (_isGrounded || _jumpCoyoteTimeCounter > 0 || _doubleJumpCount > 0))
        {
            _jumpBufferTimeCounter = 0;

            if (!_isGrounded && _jumpCoyoteTimeCounter <= 0)
            {
                --_doubleJumpCount;
                // if decide to not stack speed
                //_playerRigidbody.linearVelocity = new Vector2(_playerRigidbody.linearVelocity.x, 0); 

                // magic number which is players mass
                force = _jumpForce - _playerRigidbody.linearVelocity.y * 5;
                EmberOvalOrbit.Instance.ConsumeEmber();
            }

            //_playerRigidbody.linearVelocity = new Vector2(_playerRigidbody.linearVelocity.x, force); jump force = 19 
            _playerRigidbody.AddForce(force * Vector2.up, ForceMode2D.Impulse);
            PlayerAudio.Instance.PlayJumpSound();
        }
        else if(_jumpBufferTimeCounter > 0) _jumpBufferTimeCounter -= Time.fixedDeltaTime;

        if (_jumpPressed) 
            _jumpCoyoteTimeCounter = 0;

        if (_jumpReleased && _playerRigidbody.linearVelocity.y > 0 && _doubleJumpCount == _totalDoubleJumpCount) 
        {
            _playerRigidbody.AddForce(0.5f * force * -Vector2.up, ForceMode2D.Impulse);
            //_playerRigidbody.linearVelocity = new Vector2(_playerRigidbody.linearVelocity.x, _playerRigidbody.linearVelocity.y * 0.5f);
        }

        if(_isWallSliding && _jumpPressed)// && !_wallJumped)
        {
            //_playerRigidbody.linearVelocity = new Vector2(Mathf.Sign(transform.localScale.x) * _wallJumpStrengthX, _wallJumpStrengthY);                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
            //===
            // questionable idk if i should leave it like this
            _playerRigidbody.linearVelocity = new Vector2(_playerRigidbody.linearVelocity.x, 0);
            //===
            _playerRigidbody.AddForce(
                new Vector2(Mathf.Sign(transform.localScale.x) * -_wallJumpStrengthX, _wallJumpStrengthY), 
                ForceMode2D.Impulse);                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
            _canMove = false;
            _moveCooldownTimeCounter = _moveCooldownTime;
            _isWallSliding = false;
        }

        GetAerialState();

        if(_isAtApex && !_isFalling)
        {
            _playerRigidbody.gravityScale = _apexGravityModifier * _originalGravity;
        }
        else if(_isGrounded || _isWalled || _isJumping) 
        {
            _playerRigidbody.gravityScale = _originalGravity;
        }
        else if(_isFalling) _playerRigidbody.gravityScale = _fallingGravityModifier * _originalGravity;
        
        if(_isJumping && Mathf.Abs(_playerRigidbody.linearVelocity.y) <= _jumpApexThreshold) _isAtApex = true;
        else _isAtApex = false;
    }

    void GetAerialState()
    {
        if(!_isJumping  && _playerRigidbody.linearVelocity.y > 0) _isJumping = true;
        else if(_playerRigidbody.linearVelocity.y <= 0) _isJumping = false;
        
        if (_playerRigidbody.linearVelocity.y < 0) _isFalling = true;
        else _isFalling = false;
    }

    void ClampFallSpeed()
    {
        if (_playerRigidbody.linearVelocity.y <= _maxFallSpeed) _playerRigidbody.linearVelocity = new Vector2(_playerRigidbody.linearVelocity.x, _maxFallSpeed);
    }
    void ClampWallSlideSpeed()
    {
        if (_playerRigidbody.linearVelocity.y <= _maxWallSlideSpeed) _playerRigidbody.linearVelocity = new Vector2(_playerRigidbody.linearVelocity.x, _maxWallSlideSpeed);
    }

    void EdgeSupport()
    {
        if(RightEdgeDetection() && !LeftEdgeDetection()) transform.Translate(Vector2.right * -1 /* Mathf.Sign(transform.localScale.x) */ * _edgeMoveAmount * Time.fixedDeltaTime);
        else if(LeftEdgeDetection() && !RightEdgeDetection()) transform.Translate(Vector2.right /* Mathf.Sign(transform.localScale.x) */ * _edgeMoveAmount * Time.fixedDeltaTime);
    }

    bool RightEdgeDetection()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + _rightEdgeDetectionBoxOffset, _edgeDetectionBoxSize, 0, groundLayer);
    }

    bool LeftEdgeDetection()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + _leftEdgeDetectionBoxOffset, _edgeDetectionBoxSize, 0, groundLayer);
    }

    void PushUpLedge()
    {
        transform.Translate(Vector2.up * _ledgeMoveAmount * Time.fixedDeltaTime);
    }

    bool IsTouchingLedge()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + new Vector2(_ledgeDetectionBoxOffset.x * transform.localScale.x, _ledgeDetectionBoxOffset.y), _ledgeDetectionBoxSize, 0, groundLayer);
    }


    void Rotate()
    {
        _movementVector = new Vector2(_playerRigidbody.linearVelocity.x, _playerRigidbody.linearVelocity.y);
        if(!_canMove && Mathf.Abs(_playerRigidbody.linearVelocity.x) >= 0.1f) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(_playerRigidbody.linearVelocity.x), transform.localScale.y);
        else if(UserInput.Instance.moveInput.x < 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
        else if(UserInput.Instance.moveInput.x > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y);
    }

    public void WallSlide()
    {
        float speedDif = _slideSpeed - _playerRigidbody.linearVelocity.y;	
		float movement = speedDif * _slideAccel;
		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));
        //movement = Mathf.Min(movement, 0);
		if (_canMove) _playerRigidbody.AddForce(movement * Vector2.up);
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + new Vector2(_groundBoxOffset.x * transform.localScale.x, _groundBoxOffset.y), _groundBoxSize, 0, groundLayer);
    }

    bool IsWalled()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + new Vector2(_wallBoxOffset.x * transform.localScale.x, _wallBoxOffset.y), _wallBoxSize, 0, wallLayer);
    }

    public bool IsMoving()
    {
        if(_playerRigidbody.linearVelocity.x != 0) return true;
        else return false;
    }

    private bool MovingWithoutInput()
    {
        if(IsMoving() && IsGrounded() && Mathf.Abs(UserInput.Instance.moveInput.x) == 0) return true;
        else return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(_groundBoxOffset.x * transform.localScale.x, _groundBoxOffset.y), _groundBoxSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(_wallBoxOffset.x * transform.localScale.x, _wallBoxOffset.y), _wallBoxSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + _rightEdgeDetectionBoxOffset, _edgeDetectionBoxSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + _leftEdgeDetectionBoxOffset, _edgeDetectionBoxSize);

        Gizmos.color = Color.purple;
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(_ledgeDetectionBoxOffset.x * transform.localScale.x, _ledgeDetectionBoxOffset.y), _ledgeDetectionBoxSize);
    }
    
    public bool GetIsAlive()
    {
        return _isAlive;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("LethalTrap"))
        {
            Debug.Log("Player has collided with a lethal trap.");
            Death();
        }
    }
    
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("LethalTrap"))
        {
            Debug.Log("Player has triggered with a lethal trap.");
            Death();
        }
    }

    public void Death()
    {
        // Implement death logic here
        //Time.timeScale = 0;
        Debug.Log("Player has died.");
        _isAlive = false;
        _canMove = false;
        _playerRigidbody.bodyType = RigidbodyType2D.Static;
        SceneLoading.Instance.ReloadScene();
    }

    public void Respawn()
    {
        _isAlive = true;
        _canMove = true;
        _playerRigidbody.bodyType = RigidbodyType2D.Dynamic;
    }

    public float GetFlickerSpeedModifier()
    {
        return _currentSpeed / _runSpeed;
    }
    
    public void SetMoveCooldown(float time)
    {
        _canMove = false;
        _moveCooldownTimeCounter = time;
    }
}
