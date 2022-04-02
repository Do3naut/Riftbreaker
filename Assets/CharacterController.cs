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
        FancyMove();
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
                facingLeft = false;
            }
            else if (directionx < 0)
            {
                facingLeft = true;
            }
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
                DIThreshold = Mathf.Abs(rb.velocity.x) + (directionalInfluence * 5f);
            } 
            else if (canWallJump)
            {
                float horiVel = facingLeft ? jumpHeight / 2 : -jumpHeight / 2;
                rb.AddForce(new Vector2(horiVel, jumpHeight / 2), ForceMode2D.Impulse); // Wall jump
                DIThreshold = Mathf.Abs(rb.velocity.x) + (directionalInfluence * 5f);  // tiny DI
                facingLeft = !facingLeft;
                canWallJump = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))  // Dash
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(directionx, directiony) * 10f, ForceMode2D.Impulse);
        }
        float dir = facingLeft ? -1f : 1f;
        rb.velocity = new Vector2(dir * moveSpeed, rb.velocity.y);  // Modifies x velocity directly.
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
