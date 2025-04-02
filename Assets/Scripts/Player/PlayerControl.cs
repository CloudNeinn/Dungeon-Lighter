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
    [SerializeField] private bool canMove;
    [SerializeField] private float moveCooldownTime;
    [SerializeField] private float moveCooldownTimeCounter;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCoyoteTime;
    [SerializeField] private float jumpCoyoteTimeCounter;
    [SerializeField] private float jumpBufferTime;
    [SerializeField] private float jumpBufferTimeCounter;

    [SerializeField] private int doubleJumpIndex;
    [SerializeField] private int totalDoubleJumpCount;
    [SerializeField] private int doubleJumpCount;

    [SerializeField] private float wallJumpStrengthX;
    [SerializeField] private float wallJumpStrengthY;

    [SerializeField] private bool isSliding;
    [field: SerializeField] public float slideSpeed;
    [field: SerializeField] public float slideAccel;

    [Header("Check Parameters")]
    [SerializeField] private Vector2 groundBoxSize;
    [SerializeField] private Vector2 groundBoxOffset;
    [SerializeField] private Vector2 wallBoxSize;
    [SerializeField] private Vector2 wallBoxOffset;
    [SerializeField] private float torchLightRadius;
    [SerializeField] private Vector2 portalCheckBoxSize;
    [SerializeField] private Vector2 portalCheckBoxOffset;

    [Header("Layer Masks")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public LayerMask torchLayer;
    public LayerMask portalLayer;

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
    [field: SerializeField] public Vector2 movementVector {get; private set;}
    private GameObject torch;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        //DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {

    }

    void Update()
    {
        movementVector = new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.y);
        if(_moveInput.x > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        else if(_moveInput.x < 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
        else if(Mathf.Abs(playerRigidbody.velocity.x) >= 0.1f) transform.localScale = new Vector3(Mathf.Sign(playerRigidbody.velocity.x), transform.localScale.y);
        acceleration = Mathf.Clamp(acceleration, 0, 1);
        decceleration = Mathf.Clamp(decceleration, 0, 1);
        GetInput();

        if(canMove) Movement();
        else if(moveCooldownTimeCounter > 0) moveCooldownTimeCounter -= Time.deltaTime;
        else canMove = true;

        Jump();
        _isGrounded = isGrounded();
        currentSpeed = playerRigidbody.velocity.x;

        if(isWalled() && Mathf.Abs(_moveInput.x) > 0 && canMove) isSliding = true;
        else isSliding = false;

        if(isSliding && UserInput.Instance._jumpAction.WasPressedThisFrame())
        {
            playerRigidbody.AddForce(new Vector2(Mathf.Sign(transform.localScale.x) * wallJumpStrengthX, wallJumpStrengthY), ForceMode2D.Impulse);
            canMove = false;
            moveCooldownTimeCounter = moveCooldownTime;
            isSliding = false;
        }

        if(_use1Input)
        {
            torch = getTorch()?.gameObject.transform.GetChild(0).gameObject;
            if(torch != null) torch.SetActive(true);
        }
           
    }

    void FixedUpdate()
    {
        if(isSliding) Slide();
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

        if (UserInput.Instance._jumpAction.WasPressedThisFrame() && !isSliding) 
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
        else if(jumpBufferTimeCounter > 0) jumpBufferTimeCounter -= Time.deltaTime;

        if (UserInput.Instance._jumpAction.WasPressedThisFrame()) 
            jumpCoyoteTimeCounter = 0;

        if (UserInput.Instance._jumpAction.WasReleasedThisFrame() && playerRigidbody.velocity.y > 0 && doubleJumpIndex == totalDoubleJumpCount) 
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.y * 0.5f);
        }
    }

    public void Slide()
    {
        float speedDif = slideSpeed - playerRigidbody.velocity.y;	
		float movement = speedDif * slideAccel;
		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));
		if(canMove) playerRigidbody.AddForce(movement * Vector2.up);
    }

    bool isGrounded()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + groundBoxOffset, groundBoxSize, 0, groundLayer);
    }

    bool isWalled()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + new Vector2(wallBoxOffset.x * transform.localScale.x, wallBoxOffset.y), wallBoxSize, 0, groundLayer);
    }

    Collider2D getPortal()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + portalCheckBoxOffset, portalCheckBoxSize, 0, groundLayer);
    }

    Collider2D getTorch()
    {
        return Physics2D.OverlapCircle((Vector2)transform.position, torchLightRadius, torchLayer);
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

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position, torchLightRadius);
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

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Portal")
        {
            Debug.Log("Teleporting");
            collision.gameObject.GetComponent<Portal>().TeleportPlayer();
        }
    }
}
