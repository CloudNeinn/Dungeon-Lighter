using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance;

    [Header("Components")]
    [SerializeField] private Rigidbody2D playerRigidbody;
    [SerializeField] private CircleCollider2D playerCollider;

    [Header("Movement Parameters")]
    [SerializeField] private float topSpeed = 10f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float targetSpeed;
    [SerializeField] private float speedDif;
    [SerializeField] private float acceleration = 0.5f;

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

    [Header("Layer Masks")]
    public LayerMask groundLayer;

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
        GetInput();
        Movement();
        Jump();
        _isGrounded = isGrounded();
        
    }
    
    void Movement()
    {
        _moveInput = UserInput.Instance.MoveInput;

        targetSpeed = Mathf.Lerp(playerRigidbody.velocity.x, 
            Mathf.Clamp(Mathf.Abs(targetSpeed) + Time.fixedDeltaTime * acceleration, currentSpeed, topSpeed) * _moveInput.x, 1);
        
        speedDif = (targetSpeed - playerRigidbody.velocity.x) * 5;

        playerRigidbody.AddForce(speedDif * Vector2.right, ForceMode2D.Force);
    }

    void Jump()
    {
        float force = jumpForce;
        if(_jumpInput) Debug.Log("Jumping");

        if(isGrounded()) jumpCoyoteTimeCounter = jumpCoyoteTime;
        else jumpCoyoteTimeCounter -= Time.deltaTime;
    
        if(UserInput.Instance._jumpAction.WasPressedThisFrame() && !isGrounded()) 
            jumpBufferTimeCounter = jumpBufferTime;

        if(jumpBufferTimeCounter > 0 && isGrounded()) 
        {
            if (playerRigidbody.velocity.y > 0)
                force -= playerRigidbody.velocity.y;

            playerRigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }

        if(jumpBufferTimeCounter > 0) jumpBufferTimeCounter -= Time.deltaTime;

        if ((UserInput.Instance._jumpAction.WasPressedThisFrame() && (isGrounded() || jumpCoyoteTimeCounter > 0 || doubleJumpIndex > 0)))
        {
            if (playerRigidbody.velocity.y > 0)
                force -= playerRigidbody.velocity.y;

            playerRigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);

            if(!isGrounded() && jumpCoyoteTimeCounter < 0) 
                --doubleJumpIndex;
        }

        if(UserInput.Instance._jumpAction.WasPressedThisFrame()) jumpCoyoteTimeCounter = 0;


        if (UserInput.Instance._jumpAction.WasPressedThisFrame() && isGrounded())
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);

        if (UserInput.Instance._jumpAction.WasReleasedThisFrame() && playerRigidbody.velocity.y > 0 && doubleJumpIndex == totalDoubleJumpCount)
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.y / 4);
    }

    bool isGrounded()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + groundBoxOffset, groundBoxSize, 0, groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + groundBoxOffset, groundBoxSize);
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
