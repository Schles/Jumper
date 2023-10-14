using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IDataPersistance
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

    private HingeJoint2D hingeJoint2D;

    private Grappler grappler;
    public void Awake()
    {
        // assign a callback for the "jump" action.
        jumpAction.performed += ctx => { OnJump(ctx); };
        jumpAction.canceled += ctx => { OnJumpUp(ctx); };
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
        hingeJoint2D = GetComponent<HingeJoint2D>();
        grappler = GetComponent<Grappler>();

        sounds = GetComponents<AudioSource>();

        rb.gravityScale = Data.gravityScale;
        IsFacingRight = true;

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // if ( player.isDead )
        // {
        //     return;
        // }

        lastPressedJumpTime = Data.jumpInputBufferTime;
    }

    public void OnJumpUp(InputAction.CallbackContext context)
    {
        if (CanJumpCut() || CanWallJumpCut())
            IsJumpCut = true;
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

        if (moveInput.x > 0 && !IsFacingRight)
        {
            Flip();
        } else if (moveInput.x < 0 && IsFacingRight)
        {
            Flip();
        }

       

        // COllision Checks
        if(!IsJumping)
        {
           if(IsOnGround())
            {
                lastOnGroundTime = Data.coyoteTime;
            }

            //Right Wall Check
            if (((Physics2D.OverlapBox(frontWallCheckPoint.position, _wallCheckSize, 0, groundLayer) && IsFacingRight)
                    || (Physics2D.OverlapBox(backWallCheckPoint.position, _wallCheckSize, 0, groundLayer) && !IsFacingRight)) && !IsWallJumping)
                lastOnWallRightTime = Data.coyoteTime;

            //Left Wall Check
            if (((Physics2D.OverlapBox(frontWallCheckPoint.position, _wallCheckSize, 0, groundLayer) && !IsFacingRight)
                || (Physics2D.OverlapBox(backWallCheckPoint.position, _wallCheckSize, 0, groundLayer) && IsFacingRight)) && !IsWallJumping)
                lastOnWallLeftTime = Data.coyoteTime;

            //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
            lastOnWallTime = Mathf.Max(lastOnWallLeftTime, lastOnWallRightTime);
        }

        SetAnimatorParams();


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

 

        print("isHanging " + IsOnRope());
        // Jump
        if( (CanJump() || CanDoubleJump()) && !CanWallJump() && lastPressedJumpTime > 0)
        {

            if (!CanJump() && CanDoubleJump())
            {
                hasDoubleJumpedPressed = true;
            }
            
            IsJumping = true;
            IsWallJumping = false;
            IsJumpCut = false;
            isJumpFalling = false;
            Jump();
        }//WALL JUMP
        else if (CanWallJump() && lastPressedJumpTime > 0)
        {
            IsWallJumping = true;
            IsJumping = false;
            IsJumpCut = false;
            isJumpFalling = false;
            wallJumpStartTime = Time.time;
            lastWallJumpDir = (lastOnWallRightTime > 0) ? -1 : 1;

            WallJump(lastWallJumpDir);
        } 
        
        if (CanSlide() && ((lastOnWallLeftTime > 0 && moveInput.x < 0) || (lastOnWallRightTime > 0 && moveInput.x > 0)))
            IsSliding = true;
        else
            IsSliding = false;
        

        //Higher gravity if we've released the jump input or are falling
        if (IsSliding)
        {
            rb.gravityScale = 0;
        }
        else if (IsOnRope()) {
            rb.gravityScale = Data.gravityScale;
        }
        else if (rb.velocity.y < 0 && moveInput.y < 0 && !IsOnRope())
        {
            //Much higher gravity if holding down
            rb.gravityScale = Data.gravityScale * Data.fastFallGravityMult;
            //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFastFallSpeed));
        }
        else if (IsJumpCut && !IsOnRope())
        {
            //Higher gravity if jump button released
            rb.gravityScale = Data.gravityScale * Data.jumpCutGravityMult;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed));
        }
        else if ((IsJumping || IsWallJumping || isJumpFalling) && Mathf.Abs(rb.velocity.y) < Data.jumpHangTimeThreshold)
        {
            rb.gravityScale = Data.gravityScale * Data.jumpHangGravityMult;
        }
        else if (rb.velocity.y < 0 && !IsOnRope())
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


    public void FixedUpdate()
    {


        //Handle Run
        if (IsWallJumping)
            Run(Data.wallJumpRunLerp);
        else
            Run(1);

        //Handle Slide
        if (IsSliding)
            Slide();
        else if (IsOnRope() && moveInput.y != 0f) {
            grappler.Slide((int)moveInput.y);
        }        
    }

    private void Run(float lerpAmount)
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = moveInput.x * Data.runMaxSpeed;

        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount);


        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (lastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;


        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((IsJumping || IsWallJumping ||isJumpFalling) && Mathf.Abs(rb.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }

        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (Data.doConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && lastOnGroundTime < 0)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - rb.velocity.x;

        //Calculate force along x-axis to apply to thr player
        float movement = speedDif * accelRate;

        //Convert this to a vector and apply to rigidbody
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

        // friction;

        // check if we're grounded and that we are trying to stop (not pressing forwards or backwards)
        if (Data.doFriction && lastOnGroundTime > 0f && Mathf.Abs(moveInput.x) < 0.01f)
        {
            // then we use either the friction amount ( ~0.2) or our velocity
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(Data.frictionAmount));

            //sets to movement direction
            amount *= Mathf.Sign(rb.velocity.x);

            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }

    private bool IsOnGround()
    {
        if (IsJumping)
        {
            return false;
        }
        
        for (int i = 0; i < groundCheckPoint.Length; i++)
        {
            if (Physics2D.OverlapBox(groundCheckPoint[i].position, new Vector2(0.2f, 0.2f), 0, groundLayer))
            {
                return true;
            }            
        }

        return false;
    }


    private void SetAnimatorParams()
    {
        animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
        animator.SetBool("IsJumping", IsJumping || IsWallJumping);
        animator.SetBool("IsFalling", isJumpFalling);
        animator.SetBool("IsSliding", IsSliding);



        animator.SetBool("IsSitting", !(IsJumping || IsWallJumping || IsSliding) && moveInput.y < 0f);
    }
    private void Jump()
    {
        //Ensures we can't call Jump multiple times from one press
        lastPressedJumpTime = 0;
        lastOnGroundTime = 0;



        //We increase the force applied if we are falling
        //This means we'll always feel like we jump the same amount 
        //(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
        float force = Data.jumpForce;
        if (rb.velocity.y < 0)
            force -= rb.velocity.y;

        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        
        sounds[0].Play();
    }



    private void WallJump(int dir)
    {
        print("Wall Jumo");
        //Ensures we can't call Wall Jump multiple times from one press
        lastPressedJumpTime = 0;
        lastOnGroundTime = 0;
        lastOnWallRightTime = 0;
        lastOnWallLeftTime = 0;

        sounds[0].Play();

        Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force.x *= dir; //apply force in opposite direction of wall

        if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
            force.x -= rb.velocity.x;

        if (rb.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
            force.y -= rb.velocity.y;

        //Unlike in the run we want to use the Impulse mode.
        //The default mode will apply are force instantly ignoring masss
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void Slide()
    {
        //Works the same as the Run but only in the y-axis
        //THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
        float speedDif = Data.slideSpeed - rb.velocity.y;
        float movement = speedDif * Data.slideAccel;
        //So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
        //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        rb.AddForce(movement * Vector2.up);
    }

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

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        IsFacingRight = !IsFacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }


    private bool IsOnRope() {
        return hingeJoint2D.enabled;
    }

    private bool CanJump()
    {
        return lastOnGroundTime > 0 && !IsJumping;
    }

    private bool CanDoubleJump()
    {
        return (IsJumping || isJumpFalling) && !hasDoubleJumpedPressed && canDoubleJump;
    }
    
    private bool CanWallJump()
    {
        return lastPressedJumpTime > 0 && lastOnWallTime > 0 && lastOnGroundTime <= 0 && (!IsWallJumping ||
             (lastOnWallRightTime > 0 && lastWallJumpDir == 1) || (lastOnWallLeftTime > 0 && lastWallJumpDir == -1));   
    }

    private bool CanJumpCut()
    {
        return IsJumping && rb.velocity.y > 0;
    }

    private bool CanWallJumpCut()
    {
        return IsWallJumping && rb.velocity.y > 0;
    }

    public bool CanSlide()
    {
        
        if (lastOnWallTime > 0 && !IsJumping && !IsWallJumping && lastOnGroundTime <= 0)
            return true;
        else
            return false;
                
    }

    public void LoadGame(GameData data)
    {
        canDoubleJump = data.canDoubleJump;
    }

    public void SaveGame(ref GameData data)
    {
        data.canDoubleJump = canDoubleJump;
    }

}
