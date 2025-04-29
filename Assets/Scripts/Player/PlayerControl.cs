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

    public enum FrameRate
    {
        FPS_15 = 15,
        FPS_30 = 30,
        FPS_60 = 60,
        FPS_120 = 120,
        FPS_144 = 144,
        FPS_240 = 240
    }

    [SerializeField] public FrameRate frameRate;

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
    }


    void Update()
    {
        Application.targetFrameRate = (int)frameRate;
        Rotate();
        GetInput();
        Movement();
        
        if(moveCooldownTimeCounter > 0) moveCooldownTimeCounter -= Time.deltaTime;
        else canMove = true;

        Jump();
        _isGrounded = isGrounded();

        if(isWalled() && Mathf.Abs(_moveInput.x) > 0 && canMove) isSliding = true;
        else isSliding = false;


        if(_use1Input)
        {
            torch = getTorch()?.gameObject.transform.GetChild(0).gameObject;
            if(torch != null) torch.SetActive(true);
        }
           
    }

    void FixedUpdate()
    {
        if(isSliding) Slide();
        if(canMove && !isSliding) playerRigidbody.velocity = new Vector2(_moveInput.x * currentSpeed + playerRigidbody.velocity.x/4, playerRigidbody.velocity.y);

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
            targetSpeed = airSpeed; 
        }
        else if(isWalled())
        {
            targetSpeed = 0;
        }

        if(currentSpeed != targetSpeed)
        {
            speedDiff = targetSpeed - currentSpeed;
            currentSpeed = Mathf.Clamp(currentSpeed += speedChangeRate * Time.deltaTime * Mathf.Sign(speedDiff), 0, runSpeed);
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
            PlayerAudio.Instance.PlayJumpSound();
        }
        else if(jumpBufferTimeCounter > 0) jumpBufferTimeCounter -= Time.deltaTime;

        if (UserInput.Instance._jumpAction.WasPressedThisFrame()) 
            jumpCoyoteTimeCounter = 0;

        if (UserInput.Instance._jumpAction.WasReleasedThisFrame() && playerRigidbody.velocity.y > 0 && doubleJumpIndex == totalDoubleJumpCount) 
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.y * 0.5f);
        }

        if(isSliding && UserInput.Instance._jumpAction.WasPressedThisFrame())
        {
            playerRigidbody.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * wallJumpStrengthX, wallJumpStrengthY);                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
            canMove = false;
            moveCooldownTimeCounter = moveCooldownTime;
            isSliding = false;
        }
    }

    void Rotate()
    {
        movementVector = new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.y);
        if(!canMove && Mathf.Abs(playerRigidbody.velocity.x) >= 0.1f) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(playerRigidbody.velocity.x), transform.localScale.y);
        else if(_moveInput.x < 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
        else if(_moveInput.x > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y);
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

    void Death()
    {
        // Implement death logic here
        Debug.Log("Player has died.");
        SceneLoading.Instance.ReloadScene();
    }
}
