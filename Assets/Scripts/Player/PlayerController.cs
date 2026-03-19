using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Components")]
    [SerializeField] private Rigidbody2D _playerRigidbody;
    [SerializeField] private CapsuleCollider2D _playerCollider;
    private PlayerLight _playerLight;

    #region States
    [Header("States")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isSliding;
    [SerializeField] private bool _isJumping;
    [SerializeField] private bool _isAtApex;
    [SerializeField] private bool _isAlive;
    #endregion

    #region Movement
    [Header("Movement Parameters")]
    [SerializeField] private Vector2 _movementVector;
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

    #region Jump
    [Header("Jump Parameters")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpCoyoteTime;
    [SerializeField] private float _jumpCoyoteTimeCounter;
    [SerializeField] private float _jumpBufferTime;
    [SerializeField] private float _jumpBufferTimeCounter;
    [SerializeField] private int _doubleJumpIndex;
    [SerializeField] private int _totalDoubleJumpCount;
    [SerializeField] private int _doubleJumpCount;
    [SerializeField] private float _wallJumpStrengthX;
    [SerializeField] private float _wallJumpStrengthY;
    [SerializeField] private float _jumpFallForce;
    [SerializeField] private float _jumpApexThreshold;
    [SerializeField] private float _apexSpeedMultiplier;
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
    [SerializeField] private Vector2 _portalCheckBoxSize;
    [SerializeField] private Vector2 _portalCheckBoxOffset;
    #endregion

    #region Layer Masks
    [Header("Layer Masks")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public LayerMask portalLayer;
    #endregion

    #region User Inputs
    [field: Header ("Inputs")]    
    [field: SerializeField] public Vector2 moveInput {get; private set;}
    public bool jumpInput {get; private set;}
    public bool dashInput {get; private set;}
    public bool runInput {get; private set;}
    public bool use1Input {get; private set;}
    public bool use2Input {get; private set;}
    public bool menuToggleInput {get; private set;}
    public bool activeItem1Input {get; private set;}
    public bool mapInput {get; private set;}
    #endregion

    #region GET/SET
    
    public float RunSpeed
    {
        get => _runSpeed;
        set => _runSpeed = value;
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
        //_airSpeed = _runSpeed * 1.2f; 
    }


    void Update()
    {
        if(_isAlive) Rotate();

        GetInput();
        _isGrounded = IsGrounded();

        if (IsWalled() && Mathf.Abs(moveInput.x) > 0 && _canMove)
        {
            _isSliding = true;
            _playerRigidbody.linearVelocity = new Vector2(0, _playerRigidbody.linearVelocity.y);
        }
        else _isSliding = false;

        if (UserInput.Instance.jumpAction.WasPressedThisFrame())
            _jumpPressed = true;
        
        if (UserInput.Instance.jumpAction.WasReleasedThisFrame())
            _jumpReleased = true;
    }

    void FixedUpdate()
    {
        if(_moveCooldownTimeCounter > 0) _moveCooldownTimeCounter -= Time.fixedDeltaTime;
        else if(_isAlive) _canMove = true;
        
        if(_canMove)
        {
            Movement();
            Jump();
        }

        if(_isSliding) Slide();

        if(_canMove && !_isSliding) 
            _playerRigidbody.linearVelocity = new Vector2(moveInput.x * _currentSpeed/* + _playerRigidbody.linearVelocity.x/4*/, _playerRigidbody.linearVelocity.y);

        _jumpPressed = false;
        _jumpReleased = false;
    }

    void Movement()
    {
        if(IsGrounded())
        {
            if(UserInput.Instance.runAction.IsPressed()) _targetSpeed = _runSpeed;
            else if(!UserInput.Instance.runAction.IsPressed()) _targetSpeed = _walkSpeed;
        }
        else if(!IsGrounded() && !IsWalled())
        {
            if(_isAtApex) _targetSpeed = _airSpeed * _apexSpeedMultiplier; 
            else _targetSpeed = _airSpeed; 
        }
        else if(IsWalled())
        {
            _targetSpeed = 0;
        }

        if(_currentSpeed != _targetSpeed)
        {
            _speedDiff = _targetSpeed - _currentSpeed;
            _currentSpeed = Mathf.Clamp(_currentSpeed += _speedChangeRate * Time.fixedDeltaTime  * Mathf.Sign(_speedDiff), 0, _runSpeed);
            if(Mathf.Abs(_speedDiff) < 0.01f) _currentSpeed = _targetSpeed;
        }
    }

    void Jump()
    {
        float force = _jumpForce;

        if (IsGrounded() || _isSliding) 
        {
            _jumpCoyoteTimeCounter = _jumpCoyoteTime;
            _doubleJumpIndex = _totalDoubleJumpCount;
        }
        else _jumpCoyoteTimeCounter -= Time.fixedDeltaTime;

        if (_jumpPressed && !_isSliding) 
            _jumpBufferTimeCounter = _jumpBufferTime;

        if ((_jumpBufferTimeCounter > 0) && (IsGrounded() || _jumpCoyoteTimeCounter > 0 || _doubleJumpIndex > 0))
        {
            _jumpBufferTimeCounter = 0;

            if (_playerRigidbody.linearVelocity.y > 0)
                force -= _playerRigidbody.linearVelocity.y;

            if (!IsGrounded() && _jumpCoyoteTimeCounter <= 0) 
                --_doubleJumpIndex;

            _playerRigidbody.linearVelocity = new Vector2(_playerRigidbody.linearVelocity.x, force);
            PlayerAudio.Instance.PlayJumpSound();
        }
        else if(_jumpBufferTimeCounter > 0) _jumpBufferTimeCounter -= Time.fixedDeltaTime;

        if (_jumpPressed) 
            _jumpCoyoteTimeCounter = 0;

        if (_jumpReleased && _playerRigidbody.linearVelocity.y > 0 && _doubleJumpIndex == _totalDoubleJumpCount) 
        {
            _playerRigidbody.linearVelocity = new Vector2(_playerRigidbody.linearVelocity.x, _playerRigidbody.linearVelocity.y * 0.5f);
        }

        if(_isSliding && _jumpPressed)
        {
            _playerRigidbody.linearVelocity = new Vector2(Mathf.Sign(transform.localScale.x) * _wallJumpStrengthX, _wallJumpStrengthY);                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
            _canMove = false;
            _moveCooldownTimeCounter = _moveCooldownTime;
            _isSliding = false;
        }

        if(!_isJumping  && _playerRigidbody.linearVelocity.y > 0) _isJumping = true;
        if(_isJumping && _playerRigidbody.linearVelocity.y < 0 && !_isAtApex) _playerRigidbody.AddForce(Vector2.up * -_jumpFallForce);
        if(IsGrounded() && _isJumping) _isJumping = false;
        if(_isJumping && Mathf.Abs(_playerRigidbody.linearVelocity.y) <= _jumpApexThreshold) _isAtApex = true;
        else _isAtApex = false;
    }

    void Rotate()
    {
        _movementVector = new Vector2(_playerRigidbody.linearVelocity.x, _playerRigidbody.linearVelocity.y);
        if(!_canMove && Mathf.Abs(_playerRigidbody.linearVelocity.x) >= 0.1f) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(_playerRigidbody.linearVelocity.x), transform.localScale.y);
        else if(moveInput.x < 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
        else if(moveInput.x > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y);
    }

    public void Slide()
    {
        float speedDif = _slideSpeed - _playerRigidbody.linearVelocity.y;	
		float movement = speedDif * _slideAccel;
		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));
        //movement = Mathf.Min(movement, 0);
		if (_canMove) _playerRigidbody.AddForce(movement * Vector2.up);
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + _groundBoxOffset, _groundBoxSize, 0, groundLayer);
    }

    bool IsWalled()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + new Vector2(_wallBoxOffset.x * transform.localScale.x, _wallBoxOffset.y), _wallBoxSize, 0, wallLayer);
    }

    Collider2D GetPortal()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + _portalCheckBoxOffset, _portalCheckBoxSize, 0, groundLayer);
    }

    public bool IsMoving()
    {
        if(_playerRigidbody.linearVelocity.x != 0 && IsGrounded()) return true;
        else return false;
    }

    private bool MovingWithoutInput()
    {
        if(IsMoving() && Mathf.Abs(moveInput.x) == 0) return true;
        else return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + _groundBoxOffset, _groundBoxSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(_wallBoxOffset.x * transform.localScale.x, _wallBoxOffset.y), _wallBoxSize);
    }

    void GetInput()
    {
        moveInput = UserInput.Instance.moveInput;
        jumpInput = UserInput.Instance.jumpInput;
        dashInput = UserInput.Instance.dashInput;
        runInput = UserInput.Instance.runInput;
        use1Input = UserInput.Instance.use1Input;
        use2Input = UserInput.Instance.use2Input;
        menuToggleInput = UserInput.Instance.menuToggleInput; 
        activeItem1Input = UserInput.Instance.activeItem1Input;
        mapInput = UserInput.Instance.mapInput;
    }
    
    public bool GetIsAlive()
    {
        return _isAlive;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Trap")
        {
            Debug.Log("Player has collided with an enemy.");
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

    public float GetFlickerSpeedModifier()
    {
        return _currentSpeed / _runSpeed;
    }
}
