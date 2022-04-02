using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float jumpHeight = 15f;
    [SerializeField] float moveSpeed = 5f;

    // Experimental movement
    bool inControl = true;
    [SerializeField] const float directionalInfluence = 0.05f;
    [SerializeField] float DIThreshold = 0;
    bool canWallJump = false;
    bool facingLeft = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // BasicMove();
        FancyMove();
    }

    // Handles the elementary logic for moving and jumping
    void BasicMove()
    {
        float direction = Input.GetAxisRaw("Horizontal");
        if (direction > 0)
        {
            facingLeft = false;
        } 
        else if (direction < 0)
        {
            facingLeft = true;
        }
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);  // Modifies x velocity directly.
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())  // Jump 
        {
            rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        }
    }

    // EXPERIMENTAL: Force-based movement, for dashes and whatnot
    void FancyMove()
    {
        float directionx = Input.GetAxis("Horizontal");
        float directiony = Input.GetAxis("Vertical");
        // Define player's ability to change velocity
        inControl = IsGrounded();  // TODO: Make this assign the DI Threshold
        // TODO: Use deceleration instead of instant velocity reset when grounded

        if (inControl)
        {
            rb.velocity = new Vector2(directionx * moveSpeed, rb.velocity.y);  // Modifies x velocity directly.
            
        } else
        {
            if (directionx * rb.velocity.x < DIThreshold)  // Setting a DI threshold
            {
                rb.AddForce(new Vector2(directionx * directionalInfluence, 0), ForceMode2D.Impulse);  // DI
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))  // Jump 
        {
            if (IsGrounded())
            {
                rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
                DIThreshold = Mathf.Abs(rb.velocity.x) + (directionalInfluence * 75f);
            } 
            else if (canWallJump)
            {
                float horiVel = facingLeft ? -jumpHeight / 2 : jumpHeight / 2;
                rb.AddForce(new Vector2(horiVel, jumpHeight / 2), ForceMode2D.Impulse); // Wall jump
                DIThreshold = Mathf.Abs(rb.velocity.x) + (directionalInfluence * 75f);
                canWallJump = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))  // Dash
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(directionx, directiony) * 10f, ForceMode2D.Impulse);
        }

    }

    bool IsGrounded()
    {
        RaycastHit2D target = Physics2D.Raycast(gameObject.transform.position, Vector2.down, 1.2f, LayerMask.GetMask("Terrain"));
        return (target.collider != null);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Jumpable")
        {
            Debug.Log("Can wallhop!");
            canWallJump = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        canWallJump = false;
    }
}
