using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance;

    [Header("Components")]
    [SerializeField] private Rigidbody2D playerRigidbody;
    [SerializeField] private CapsuleCollider2D playerCollider;

    [Header("Movement Parameters")]
    [SerializeField] private float topSpeed = 10f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float targetSpeed;
    [SerializeField] private float speedDif;
    [SerializeField] private float acceleration = 0.5f;
    [SerializeField] private float decceleration = 0.5f;
    [SerializeField] private float speedChangeRate = 0.5f;
    [SerializeField] private float velocityPower;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCoyoteTime;
    [SerializeField] private float jumpCoyoteTimeCounter;
    [SerializeField] private float jumpBufferTime;
    [SerializeField] private float jumpBufferTimeCounter;

    [SerializeField] private int doubleJumpIndex;
    [SerializeField] private int totalDoubleJumpCount;
    [SerializeField] private int doubleJumpCount;

    [Header("Check Parameters")]
    [SerializeField] private Vector2 groundBoxSize;
    [SerializeField] private Vector2 groundBoxOffset;
    [SerializeField] private Vector2 wallBoxSize;
    [SerializeField] private Vector2 wallBoxOffset;

    [Header("Layer Masks")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    #region UserInput

    [field: Header ("Inputs")]    
    [field: SerializeField] public Vector2 _moveInput {get; private set;}
    public bool _jumpInput {get; private set;}
    public bool _dashInput {get; private set;}
    public bool _crouchInput {get; private set;}
    public bool _use1Input {get; private set;}
    public bool _use2Input {get; private set;}
    public bool _menuToggleInput {get; private set;}
    public bool _activeItem1Input {get; private set;}
    public bool _mapInput {get; private set;}
    #endregion

    [field: SerializeField] public bool _isGrounded {get; private set;}

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if(_moveInput.x > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        else if(_moveInput.x < 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
        acceleration = Mathf.Clamp(acceleration, 0, 1);
        decceleration = Mathf.Clamp(decceleration, 0, 1);
        GetInput();
        Movement();
        Jump();
        _isGrounded = isGrounded();
        currentSpeed = playerRigidbody.velocity.x;
        
    }
    
    void Movement()
    {
        float force;
        _moveInput = UserInput.Instance.MoveInput;

        targetSpeed = topSpeed * _moveInput.x;

        if(!isWalled()) speedDif = targetSpeed - currentSpeed;
        else speedDif = 0;

        speedChangeRate = (Mathf.Abs(_moveInput.x) > 0.01f) ? acceleration : decceleration;

        force = Mathf.Pow(Mathf.Abs(speedDif) * speedChangeRate, velocityPower) * Mathf.Sign(speedDif);

        playerRigidbody.AddForce(force * Vector2.right, ForceMode2D.Force);
    }

    void Jump()
    {
        float force = jumpForce;

        if (isGrounded() || isWalled()) 
        {
            jumpCoyoteTimeCounter = jumpCoyoteTime;
            doubleJumpIndex = totalDoubleJumpCount;
        }
        else jumpCoyoteTimeCounter -= Time.deltaTime;

        if (UserInput.Instance._jumpAction.WasPressedThisFrame()) 
            jumpBufferTimeCounter = jumpBufferTime;

        if ((jumpBufferTimeCounter > 0) && (isGrounded() || jumpCoyoteTimeCounter > 0 || doubleJumpIndex > 0))
        {
            jumpBufferTimeCounter = 0;

            if (playerRigidbody.velocity.y > 0)
                force -= playerRigidbody.velocity.y;

            if (!isGrounded() && jumpCoyoteTimeCounter <= 0) 
                --doubleJumpIndex;

            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, force);
        }

        if (UserInput.Instance._jumpAction.WasPressedThisFrame()) 
            jumpCoyoteTimeCounter = 0;

        if (UserInput.Instance._jumpAction.WasReleasedThisFrame() && playerRigidbody.velocity.y > 0 && doubleJumpIndex == totalDoubleJumpCount) 
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.y * 0.5f);
        }
    }


    bool isGrounded()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + groundBoxOffset, groundBoxSize, 0, groundLayer);
    }

    bool isWalled()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + new Vector2(wallBoxOffset.x * transform.localScale.x, wallBoxOffset.y), wallBoxSize, 0, groundLayer);
    }

    public bool isMoving()
    {
        if(playerRigidbody.velocity.x != 0 && isGrounded()) return true;
        else return false;
    }

    private bool movingWithoutInput()
    {
        if(isMoving() && Mathf.Abs(_moveInput.x) == 0) return true;
        else return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + groundBoxOffset, groundBoxSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(wallBoxOffset.x * transform.localScale.x, wallBoxOffset.y), wallBoxSize);
    }

    void GetInput()
    {
        _moveInput = UserInput.Instance.MoveInput;
        _jumpInput = UserInput.Instance.JumpInput;
        _dashInput = UserInput.Instance.DashInput;
        _crouchInput = UserInput.Instance.CrouchInput;
        _use1Input = UserInput.Instance.Use1Input;
        _use2Input = UserInput.Instance.Use2Input;
        _menuToggleInput = UserInput.Instance.MenuToggleInput; 
        _activeItem1Input = UserInput.Instance.ActiveItem1Input;
        _mapInput = UserInput.Instance.MapInput;
    }
}
