using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D levelRb;

    [SerializeField] private SpeedUI speedUI;

    [SerializeField] PostProcess globalPostProcess;

    [SerializeField] private GameObject playerDisplay;

    private SpriteRenderer playerSprite;
    private Animator anim;
    private AudioSource jumpSound;

    [Header("Movement")]
    BoxCollider2D boxCollider;
    Rigidbody2D rb;
    [SerializeField] float jumpHeight = 15f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float speedCap = 30f;
    [SerializeField] float teleportDistance = 5f;
    [SerializeField] float passiveAcceleration = 5f;

    [Header("Ability Costs")]
    [SerializeField] float teleportSpeedCost;
    [SerializeField] float teleportScaleFactor;

    [SerializeField] float bulletTimeSpeedCost;
    [SerializeField] float bulletTimeScaleFactor;

    [SerializeField] float scaleDecreaseRate;



    private float costScalingFactor;

    [Header("Death Timer")]
    
    [SerializeField] float deathTimerStart;

    [SerializeField] float timerSpeedThreshold;
    private float deathTimer;



    // Experimental movement
    bool inControl = true;
    bool canWallJump = false;
    bool facingLeft = true;
    // Phasing
    bool moveFreeze = false;
    bool phasing = false;
    float gravscale = 0;
    float xvel = 0;


    //Time Stuff

    [SerializeField] TimeManager gameManager;

    private bool inBulletTime;


    //phasing timer
    [Header("Phase stuff")]
    [SerializeField] float phaseCooldown;
    [SerializeField] float phaseMaxDuration;
    bool canPhase;

    // Animator
    public bool animInProgress = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerSprite = playerDisplay.GetComponent<SpriteRenderer>();
        anim = playerDisplay.GetComponent<Animator>();
        jumpSound = GameObject.Find("JumpSound").GetComponent<AudioSource>();
        costScalingFactor = 1;
        speedUI.setScaleFactor(costScalingFactor);
        speedUI.setSpeed(0);
        inBulletTime = false;
        canPhase = true;
        deathTimer = deathTimerStart;
    }

    // Update is called once per frame
    void Update()
    {
        FancyMove();

        if (!phasing) { CheckTime(); }

        doDeathTimer();
    }

    // Force-based movement, for dashes and whatnot
    void FancyMove()
    {
        float directionx = Input.GetAxis("Horizontal");
        float directiony = Input.GetAxis("Vertical");
        // Define player's ability to change velocity
        inControl = IsGrounded();  // TODO: Make this assign the DI Threshold (deprecated)

        if (inControl)
        {
            if (directionx > 0)  // The design of this game will require this to be removed in the future
            {
                facingLeft = true;
            }
            else if (directionx < 0)
            {
                facingLeft = false;
            }
            if (moveSpeed < speedCap && !inBulletTime)
                moveSpeed += passiveAcceleration / 1000;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !phasing)  // Jump 
        { 
            if (canWallJump)
            {
                float horiVel = facingLeft ? jumpHeight / 2 : -jumpHeight / 2;
                // levelRb.velocity = Vector2.zero;
                if (rb.velocity.y < 0)
                    rb.velocity = Vector2.zero;
                levelRb.AddForce(new Vector2(horiVel, 0), ForceMode2D.Impulse); // Wall jump
                rb.AddForce(new Vector2(0, jumpHeight / 2), ForceMode2D.Impulse); // Wall jump
                moveSpeed += 0.5f;
                facingLeft = !facingLeft;
                canWallJump = false;
                jumpSound.Play();
            } else if (IsGrounded())
            {
                rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
                jumpSound.Play();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !phasing)  // Teleport
        {
            anim.Play("Teleport");
            animInProgress = true;
        }

        // Phasing

        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            if (!phasing && canPhase && moveSpeed >= speedCap)
            {
                anim.Play("RiftEnter");
            } else if (phasing)
            {
                anim.Play("RiftExit");
            }
        }

        float dir = facingLeft ? -1f : 1f;
        if (!moveFreeze)
        {
            levelRb.velocity = new Vector2(dir * moveSpeed, 0);  // Modifies x velocity directly.
            rb.velocity = new Vector2(0, rb.velocity.y);  // Modifies x velocity directly.
        }
        

        if (facingLeft) 
        {
            playerSprite.flipX = false;
        } else
        {
            playerSprite.flipX = true;
        }

        speedUI.setSpeed(moveSpeed);
        
        if (costScalingFactor > 1)
        {
            costScalingFactor -= scaleDecreaseRate * Time.deltaTime;
            speedUI.setScaleFactor(costScalingFactor);
        }
        
        //protects the player from dying to their own slow motion
        if(inBulletTime && (moveSpeed - bulletTimeSpeedCost * Time.deltaTime * costScalingFactor) <= 0)
        {
            gameManager.StopSlowMotion();
            inBulletTime = false;
        }

        // Looping Animations
        if (!animInProgress)
        {
            if (phasing)
            {
                anim.Play("RiftRun");
            }
            else if (canWallJump)
            {
                anim.Play("WallHang");
            }
            else if (!inControl)
            {
                anim.Play("Airborne");
            }
            else
            {
                anim.Play("Run");
            }
        }
    }

    // Turn off bullet time, gravity, save velocity and stop the character before resuming movement after phasing starts
    public void PreStartPhase()
    {
        // To stop slow motion / bullet time
        gameManager.StopSlowMotion();
        inBulletTime = false;
        phasing = true;
        gravscale = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        xvel = levelRb.velocity.x;
        levelRb.velocity = Vector2.zero;
        moveFreeze = true;
    }

    public void StartPhase()
    {
        levelRb.velocity = new Vector2(xvel, 0);
        moveFreeze = false;
        boxCollider.enabled = false;
        globalPostProcess.GrayShift();
        StartCoroutine(PhaseEndTimer());
        canPhase = false;
    }

    public void EndPhase()
    {
        phasing = false;
        rb.gravityScale = gravscale;
        boxCollider.enabled = true;
        globalPostProcess.WhiteShift();
        StartCoroutine(PhaseCooldownTimer());
    }

    public void Teleport()
    {
        if (moveSpeed < teleportSpeedCost * costScalingFactor)
            return;
        float tpDist = teleportDistance;  
        Transform levelTr = levelRb.gameObject.transform;
        if (facingLeft)  // Teleport left 
        {
            levelTr.position += Vector3.left * tpDist;
        }
        else  // Teleport right 
        {
            levelTr.position += Vector3.right * tpDist;
        }
        moveSpeed -= teleportSpeedCost * costScalingFactor;
        costScalingFactor += teleportScaleFactor;
        speedUI.setScaleFactor(costScalingFactor);
    }

    bool IsGrounded()
    {
        RaycastHit2D target = Physics2D.Raycast(gameObject.transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Terrain"));
        return (target.collider != null);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Jumpable"))
        {
            Debug.Log("Can wallhop!");
            canWallJump = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        canWallJump = false;
    }

    void CheckTime()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            gameManager.StartSlowMotion();
            inBulletTime = true;
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            gameManager.StopSlowMotion();
            inBulletTime = false;
        }
    }


    public void BurnTime()
    {
        moveSpeed -= bulletTimeSpeedCost * Time.deltaTime * costScalingFactor; //this might need to be unchanged time, idk
        costScalingFactor += bulletTimeScaleFactor;
    }

    public void ReduceSpeed(float toSubtract)
    {
        moveSpeed -= toSubtract;
    }


    private void doDeathTimer()
    {
        if (moveSpeed < timerSpeedThreshold)
        {
            deathTimer -= Time.deltaTime;
            speedUI.setDeathTimer(deathTimer, false);
        }
        else
        {
            deathTimer += Time.deltaTime;
            speedUI.setDeathTimer(deathTimer, true);
        }

        
        if (deathTimer <= 0)
        {
            Debug.Log("I have died :(");
            //call gamemanger death thing here
        }
    }

    IEnumerator PhaseCooldownTimer()
    {
        yield return new WaitForSeconds(phaseCooldown);
        canPhase = true;
    }


    IEnumerator PhaseEndTimer()
    {
        yield return new WaitForSeconds(phaseMaxDuration);
        anim.Play("RiftExit");
    }


}
