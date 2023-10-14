using UnityEngine;
using UnityEngine.InputSystem;

public class RopeMovement : MonoBehaviour
{


    public PlayerMovementData Data;

    AudioSource[] sounds;

    [Header("Checks")]
    public Transform[] groundCheckPoint;
    
    public LayerMask groundLayer;
    public Transform frontWallCheckPoint;
    public Transform backWallCheckPoint;
    
    
    private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
    private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [Header("Input")]
    public InputAction moveAction;
    public InputAction jumpAction;
    Rigidbody2D rb;

    private Animator animator;
    private Player player;

    public bool isJumpFalling = false;

    public bool IsWallJumping = false;

    private bool IsFacingRight = false;

    private Vector2 moveInput;
    public bool IsJumping = false;
    public bool IsSliding = false;
    public bool IsJumpCut = false;
    public float lastOnGroundTime = 0f;
    public float lastOnWallTime = 0f;

    private float lastOnWallRightTime = 0f;
    private float lastOnWallLeftTime = 0f;

    private int lastWallJumpDir = 0;

    public float wallJumpStartTime = 0f;

    public float lastPressedJumpTime = 0f;

    public bool hasDoubleJumpedPressed = false;

    public bool canDoubleJump = false;

    private HingeJoint2D grapper;

    public void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
        grapper = GetComponent<HingeJoint2D>();

        sounds = GetComponents<AudioSource>();

        rb.gravityScale = Data.gravityScale;
        IsFacingRight = true;

    }

   


    // Update is called once per frame
    void Update()
    {

        // Timers
        lastOnGroundTime -= Time.deltaTime;
        lastOnWallTime -= Time.deltaTime;
        lastOnWallRightTime -= Time.deltaTime;
        lastOnWallLeftTime -= Time.deltaTime;


        lastPressedJumpTime -= Time.deltaTime;

        Vector2 moveAmount = new Vector2(0f, 0f);
        if (!player.isDead)
        {
            moveAmount = moveAction.ReadValue<Vector2>();
        }
        else
        {
        }

        moveInput = moveAmount;


       

       


        if (IsWallJumping && (Time.time - wallJumpStartTime) > Data.wallJumpTime)
        {
            IsWallJumping = false;
        }

        // Jump check
        if (IsJumping && rb.velocity.y < 0)
        {
            IsJumping = false;

            if (!IsWallJumping)
                isJumpFalling = true;

        }

        if (rb.velocity.y < 0 && !IsJumping && !IsWallJumping)
        {
            isJumpFalling = true;
        }

        

        if (lastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
            IsJumpCut = false;

            if (!IsJumping)
            {
                isJumpFalling = false;
                hasDoubleJumpedPressed = false;
            }
        }

 
        // Jump
        if (CanSlide() && ((lastOnWallLeftTime > 0 && moveInput.x < 0) || (lastOnWallRightTime > 0 && moveInput.x > 0)))
            IsSliding = true;
        else
            IsSliding = false;
        

        //Higher gravity if we've released the jump input or are falling
        if (IsSliding)
        {
            rb.gravityScale = 0;
        }
        else if (grapper.enabled) {
            rb.gravityScale = Data.gravityScale;
        }
        else if (rb.velocity.y < 0 && moveInput.y < 0)
        {
            //Much higher gravity if holding down
            rb.gravityScale = Data.gravityScale * Data.fastFallGravityMult;
            //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFastFallSpeed));
        }
        else if (IsJumpCut)
        {
            //Higher gravity if jump button released
            rb.gravityScale = Data.gravityScale * Data.jumpCutGravityMult;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed));
        }
        else if ((IsJumping || IsWallJumping || isJumpFalling) && Mathf.Abs(rb.velocity.y) < Data.jumpHangTimeThreshold)
        {
            rb.gravityScale = Data.gravityScale * Data.jumpHangGravityMult;
        }
        else if (rb.velocity.y < 0)
        {
            //Higher gravity if falling
            rb.gravityScale = Data.gravityScale * Data.fallGravityMult;
            //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed));
        }
        else
        {
            //Default gravity if standing on a platform or moving upwards
            rb.gravityScale = Data.gravityScale;
        }

    }


    // public void FixedUpdate()
    // {


    //     //Handle Run
    //     if (IsWallJumping)
    //         Run(Data.wallJumpRunLerp);
    //     else
    //         Run(1);

    //     //Handle Slide
    //     if (IsSliding)
    //         Slide();
    // }

   
  





  



    public void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    public void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

  

    public bool CanSlide()
    {
        
        if (lastOnWallTime > 0 && !IsJumping && !IsWallJumping && lastOnGroundTime <= 0)
            return true;
        else
            return false;
                
    }

   
}
