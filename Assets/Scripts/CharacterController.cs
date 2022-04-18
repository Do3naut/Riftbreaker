using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    /*
     The player, loving compiled in game jam format, a.k.a. a mess. This class does most of the method calls to
     the other classes that handle their respective behaviours - kinda like SuperPeachSisters from CS32! Sorry I'll stop.
     (also yes I recognize CharacterController is a built-in that I overrode fight me) 
     [Don't be like me I'm a bad example; ^that was a genuine mistake]
     */

    // References to other GameObjects. SerializeField is an easy way to reference stuff without much code.

    [SerializeField] private Rigidbody2D levelRb;  // The RigidBody for the level. The player does not move horizontally.

    [SerializeField] private SpeedUI speedUI;  // The UI display for the player's speed

    [SerializeField] PostProcess globalPostProcess;  // Post processing

    [SerializeField] private GameObject playerDisplay;  // The GameObject that actually displays the player sprite

    // References to components of the current GameObject
    private SpriteRenderer playerSprite;
    private Animator anim;

    // The only non-self object referenced by code (due to game jam disorganization)
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



    private float costScalingFactor;  // Abilities cost speed; spamming them will cost exponentially more.

    [Header("Death Timer")]
    
    [SerializeField] float deathTimerStart;  // Starting timer value

    [SerializeField] float timerSpeedThreshold;  // The speed threshold where the timer starts ticking down
    private float deathTimer;



    // Experimental movement
    bool inControl = true;
    bool canWallJump = false;
    bool facingLeft = true;
    float collisionPenalty = 0f;
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
        inControl = IsGrounded(); 

        if (inControl)
        {
            //if (directionx > 0)  // The design of this game will require this to be removed in the future (dev feature)
            //{
            //    facingLeft = true;
            //}
            //else if (directionx < 0)
            //{
            //    facingLeft = false;
            //}
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
                levelRb.AddForce(new Vector2(horiVel, 0), ForceMode2D.Impulse); // Wall jump - x direction
                rb.AddForce(new Vector2(0, jumpHeight / 2), ForceMode2D.Impulse); // Wall jump - y direction
                moveSpeed += 0.5f;  // Little speed boost as a bonus
                facingLeft = !facingLeft;  // Reverse direction
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
        if (!animInProgress)  // This variable controls if the looping animations can play (to prevent nonlooping ones from being interrupted)
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

    // Start phasing implementation (ah, the feature that took so much effort yet is so hard to access in game) [I cri everytiem]

    // The following 3 functions are called by the Animator (RiftEnter & RiftExit)

    // Turn off bullet time, gravity, save velocity and stop the character before resuming movement after phasing starts
    public void PreStartPhase()
    {
        // To stop slow motion / bullet time
        gameManager.StopSlowMotion();
        inBulletTime = false;
        // Inform the other functions that we are phasing
        phasing = true;
        // Save current gravity scale & set it to 0
        gravscale = rb.gravityScale;
        rb.gravityScale = 0;
        // Set player velocity to 0 (so we don't glide vertically)
        rb.velocity = Vector2.zero;
        // Save horizontal velocity and set it to 0
        xvel = levelRb.velocity.x;
        levelRb.velocity = Vector2.zero;
        moveFreeze = true;
    }

    public void StartPhase()
    {
        // Resume horizontal motion 
        levelRb.velocity = new Vector2(xvel, 0);
        moveFreeze = false;
        // Disable collider (so we phase thru walls)
        boxCollider.enabled = false;
        // Post Processing for the pizzazz
        globalPostProcess.GrayShift();
        // Start timer to countdown the time we remain in phase
        StartCoroutine(PhaseEndTimer());
        // Disable button for phasing
        canPhase = false;
    }

    public void EndPhase()
    {
        // Inform the class that we're out of phasing
        phasing = false;
        // Restore gravity
        rb.gravityScale = gravscale;
        // Reenable collider
        boxCollider.enabled = true;
        // Deduct cost for phasing
        moveSpeed -= 20;
        // Reset post processing
        globalPostProcess.WhiteShift();
        // Start cooldown timer to be able to phase again
        StartCoroutine(PhaseCooldownTimer());
    }

    // Coroutines for phase cooldown and phase duration timers. A little overkill...maybe you can change it? [this works pretty well as-is, not actually hinting at changing this]

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

    // End of phasing implementation

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

    // Called once, when 2 colliders make contact
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Jumpable"))
        {
            Debug.Log("Can wallhop!");
            canWallJump = true;
        }
        collisionPenalty = 0f;
    }

    // Called repeatedly while colliders are touching
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Jumpable")) return;
        if (moveSpeed > 5.5f) moveSpeed -= collisionPenalty * Time.deltaTime;  // Test 
        else moveSpeed = 5.5f;
        canWallJump = true;
        collisionPenalty += 0.002f * Time.deltaTime;
    }

    // Called once when the 2 colliders part
    private void OnCollisionExit2D(Collision2D collision)
    {
        canWallJump = false;
        collisionPenalty = 0f;
    }

    // Handles bullet time
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
        else if (deathTimer < 60)
        {
            deathTimer += Time.deltaTime;
            speedUI.setDeathTimer(deathTimer, true);
        }

        
        if (deathTimer <= 0)
        {
            Debug.Log("I have died :(");
            GameObject.FindObjectOfType<GameManager>().killPlayer();  // Finds the GameManager to end the game
        }
    }


}
