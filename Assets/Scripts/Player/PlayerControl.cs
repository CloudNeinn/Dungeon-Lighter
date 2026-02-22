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
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float airSpeed;
    [SerializeField] private bool canMove;
    [SerializeField] private float moveCooldownTime;
    [SerializeField] private float moveCooldownTimeCounter;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float speedChangeRate;
    private float targetSpeed;
    [SerializeField] private float speedDiff;

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
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isAtApex;
    [SerializeField] private float jumpFallForce;
    [SerializeField] private float jumpApexThreshold;
    [SerializeField] private float apexSpeedMultiplier;
    [field: SerializeField] public float slideSpeed;
    [field: SerializeField] public float slideAccel;

    [Header("Check Parameters")]
    [SerializeField] private Vector2 groundBoxSize;
    [SerializeField] private Vector2 groundBoxOffset;
    [SerializeField] private Vector2 wallBoxSize;
    [SerializeField] private Vector2 wallBoxOffset;
    [SerializeField] private Vector2 portalCheckBoxSize;
    [SerializeField] private Vector2 portalCheckBoxOffset;

    [Header("Layer Masks")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public LayerMask portalLayer;

    #region UserInput

    [field: Header ("Inputs")]    
    [field: SerializeField] public Vector2 _moveInput {get; private set;}
    public bool _jumpInput {get; private set;}
    public bool _dashInput {get; private set;}
    public bool _runInput {get; private set;}
    public bool _use1Input {get; private set;}
    public bool _use2Input {get; private set;}
    public bool _menuToggleInput {get; private set;}
    public bool _activeItem1Input {get; private set;}
    public bool _mapInput {get; private set;}
    #endregion

    [field: SerializeField] public bool _isGrounded {get; private set;}
    [field: SerializeField] public Vector2 movementVector {get; private set;}
    private GameObject torch;
    private PlayerLight playerLight;
    private bool jumpPressed;
    private bool jumpReleased;
    bool isAlive;

    #region GET/SET
    
    public float RunSpeed
    {
        get => runSpeed;
        set => runSpeed = value;
    }

    public int TotalDoubleJumpCount
    {
        get => totalDoubleJumpCount;
        set => totalDoubleJumpCount = value;
    }

    public float GravityScale
    {
        get => playerRigidbody.gravityScale;
        set => playerRigidbody.gravityScale = value;
    }

    public Rigidbody2D PlayerRigidbody => playerRigidbody;
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
        currentSpeed = walkSpeed;
        isAlive = true;
    }


    void Update()
    {
        if(isAlive) Rotate();
        GetInput();
        _isGrounded = isGrounded();

        if (isWalled() && Mathf.Abs(_moveInput.x) > 0 && canMove)
        {
            isSliding = true;
            playerRigidbody.linearVelocity = new Vector2(0, playerRigidbody.linearVelocity.y);
        }
        else isSliding = false;

        if (UserInput.Instance._jumpAction.WasPressedThisFrame())
            jumpPressed = true;
        
        if (UserInput.Instance._jumpAction.WasReleasedThisFrame())
            jumpReleased = true;
    }

    void FixedUpdate()
    {
        if(moveCooldownTimeCounter > 0) moveCooldownTimeCounter -= Time.fixedDeltaTime;
        else if(isAlive) canMove = true;
        
        if(canMove)
        {
            Movement();
            Jump();
        }

        if(isSliding) Slide();
        if(canMove && !isSliding) 
            playerRigidbody.linearVelocity = new Vector2(_moveInput.x * currentSpeed/* + playerRigidbody.linearVelocity.x/4*/, playerRigidbody.linearVelocity.y);

        jumpPressed = false;
        jumpReleased = false;
    }

    void Movement()
    {
        if(isGrounded())
        {
            if(UserInput.Instance._runAction.IsPressed()) targetSpeed = runSpeed;
            else if(!UserInput.Instance._runAction.IsPressed()) targetSpeed = walkSpeed;
        }
        else if(!isGrounded() && !isWalled())
        {
            if(isAtApex) targetSpeed = airSpeed * apexSpeedMultiplier; 
            else targetSpeed = airSpeed; 
        }
        else if(isWalled())
        {
            targetSpeed = 0;
        }

        if(currentSpeed != targetSpeed)
        {
            speedDiff = targetSpeed - currentSpeed;
            currentSpeed = Mathf.Clamp(currentSpeed += speedChangeRate * Time.fixedDeltaTime  * Mathf.Sign(speedDiff), 0, runSpeed);
            if(Mathf.Abs(speedDiff) < 0.01f) currentSpeed = targetSpeed;
        }
    }

    void Jump()
    {
        float force = jumpForce;

        if (isGrounded() || isSliding) 
        {
            jumpCoyoteTimeCounter = jumpCoyoteTime;
            doubleJumpIndex = totalDoubleJumpCount;
        }
        else jumpCoyoteTimeCounter -= Time.fixedDeltaTime;

        if (jumpPressed && !isSliding) 
            jumpBufferTimeCounter = jumpBufferTime;

        if ((jumpBufferTimeCounter > 0) && (isGrounded() || jumpCoyoteTimeCounter > 0 || doubleJumpIndex > 0))
        {
            jumpBufferTimeCounter = 0;

            if (playerRigidbody.linearVelocity.y > 0)
                force -= playerRigidbody.linearVelocity.y;

            if (!isGrounded() && jumpCoyoteTimeCounter <= 0) 
                --doubleJumpIndex;

            playerRigidbody.linearVelocity = new Vector2(playerRigidbody.linearVelocity.x, force);
            PlayerAudio.Instance.PlayJumpSound();
        }
        else if(jumpBufferTimeCounter > 0) jumpBufferTimeCounter -= Time.fixedDeltaTime;

        if (jumpPressed) 
            jumpCoyoteTimeCounter = 0;

        if (jumpReleased && playerRigidbody.linearVelocity.y > 0 && doubleJumpIndex == totalDoubleJumpCount) 
        {
            playerRigidbody.linearVelocity = new Vector2(playerRigidbody.linearVelocity.x, playerRigidbody.linearVelocity.y * 0.5f);
        }

        if(isSliding && jumpPressed)
        {
            playerRigidbody.linearVelocity = new Vector2(Mathf.Sign(transform.localScale.x) * wallJumpStrengthX, wallJumpStrengthY);                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
            canMove = false;
            moveCooldownTimeCounter = moveCooldownTime;
            isSliding = false;
        }

        if(!isJumping  && playerRigidbody.linearVelocity.y > 0) isJumping = true;
        if(isJumping && playerRigidbody.linearVelocity.y < 0 && !isAtApex) playerRigidbody.AddForce(Vector2.up * -jumpFallForce);
        if(isGrounded() && isJumping) isJumping = false;
        if(isJumping && Mathf.Abs(playerRigidbody.linearVelocity.y) <= jumpApexThreshold) isAtApex = true;
        else isAtApex = false;
    }

    void Rotate()
    {
        movementVector = new Vector2(playerRigidbody.linearVelocity.x, playerRigidbody.linearVelocity.y);
        if(!canMove && Mathf.Abs(playerRigidbody.linearVelocity.x) >= 0.1f) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(playerRigidbody.linearVelocity.x), transform.localScale.y);
        else if(_moveInput.x < 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
        else if(_moveInput.x > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y);
    }

    public void Slide()
    {
        float speedDif = slideSpeed - playerRigidbody.linearVelocity.y;	
		float movement = speedDif * slideAccel;
		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));
		if (canMove) playerRigidbody.AddForce(movement * Vector2.up);
    }

    bool isGrounded()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + groundBoxOffset, groundBoxSize, 0, groundLayer);
    }

    bool isWalled()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + new Vector2(wallBoxOffset.x * transform.localScale.x, wallBoxOffset.y), wallBoxSize, 0, wallLayer);
    }

    Collider2D getPortal()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + portalCheckBoxOffset, portalCheckBoxSize, 0, groundLayer);
    }

    public bool isMoving()
    {
        if(playerRigidbody.linearVelocity.x != 0 && isGrounded()) return true;
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
        _runInput = UserInput.Instance.RunInput;
        _use1Input = UserInput.Instance.Use1Input;
        _use2Input = UserInput.Instance.Use2Input;
        _menuToggleInput = UserInput.Instance.MenuToggleInput; 
        _activeItem1Input = UserInput.Instance.ActiveItem1Input;
        _mapInput = UserInput.Instance.MapInput;
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
        isAlive = false;
        canMove = false;
        playerRigidbody.bodyType = RigidbodyType2D.Static;
        SceneLoading.Instance.ReloadScene();
    }

    public float getFlickerSpeedModifier()
    {
        return currentSpeed / runSpeed;
    }
}
