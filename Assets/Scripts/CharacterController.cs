using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D levelRb;

    [SerializeField] private SpeedUI speedUI;

    [SerializeField] private GameObject playerDisplay;
    private SpriteRenderer playerSprite;
    private Animator anim;

    [Header("Movement")]
    BoxCollider2D collider;
    Rigidbody2D rb;
    [SerializeField] float jumpHeight = 15f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float speedCap = 30f;
    [SerializeField] float teleportDistance = 5f;

    [Header("Ability Costs")]
    [SerializeField] float teleportSpeedCost;
    [SerializeField] float teleportScaleFactor;

    [SerializeField] float bulletTimeSpeedCost;
    [SerializeField] float bulletTimeScaleFactor;

    [SerializeField] float scaleDecreaseRate;

    private float costScalingFactor;



    // Experimental movement
    bool inControl = true;
    bool canWallJump = false;
    bool facingLeft = true;


    //Time Stuff

    [SerializeField] TimeManager gameManager;

    private bool inBulletTime;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        playerSprite = playerDisplay.GetComponent<SpriteRenderer>();
        anim = playerDisplay.GetComponent<Animator>();
        costScalingFactor = 1;
        speedUI.setScaleFactor(costScalingFactor);
        speedUI.setSpeed(0);
        inBulletTime = false;
    }

    // Update is called once per frame
    void Update()
    {
        FancyMove();

        CheckTime();
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
                moveSpeed += 0.05f;
        }
        if (Input.GetKeyDown(KeyCode.Space))  // Jump 
        {
            if (IsGrounded())
            {
                rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
            } 
            else if (canWallJump)
            {
                float horiVel = facingLeft ? jumpHeight / 2 : -jumpHeight / 2;
                levelRb.AddForce(new Vector2(horiVel, 0), ForceMode2D.Impulse); // Wall jump
                rb.AddForce(new Vector2(0, jumpHeight / 2), ForceMode2D.Impulse); // Wall jump
                facingLeft = !facingLeft;
                canWallJump = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))  // Dash-jump
        {
            rb.velocity = Vector2.zero;
            levelRb.AddForce(new Vector2(directionx, 0) * 10f, ForceMode2D.Impulse);
            rb.AddForce(new Vector2(0, directiony) * 10f, ForceMode2D.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.RightShift))  // Teleport
        {
            Teleport();
        }
        float dir = facingLeft ? -1f : 1f;
        levelRb.velocity = new Vector2(dir * moveSpeed, 0);  // Modifies x velocity directly.
        rb.velocity = new Vector2(0, rb.velocity.y);  // Modifies x velocity directly.

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
            gameManager.ToggleSlowMotion();
            inBulletTime = false;
        }
    }

    void Teleport()
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
        RaycastHit2D target = Physics2D.Raycast(gameObject.transform.position, Vector2.down, 1.2f, LayerMask.GetMask("Terrain"));
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
            gameManager.ToggleSlowMotion();
            inBulletTime = !inBulletTime;
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


}
