using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    public float moveSpeed = 10f;

    float horizontalMovement;

    public float jumpSpeed = 5f;

    public int playerHealth = 3;

    public LayerMask groundlayer;

    //knockback vars
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.2f;
    private float stopKnockback = 0f;
    private Vector2 knockbackDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //grab other elements attached to this GO for use
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= stopKnockback){rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);}
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void jump(InputAction.CallbackContext context)
    {
        if(context.started && boxCollider.IsTouchingLayers(groundlayer))
        {
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the colliding object is tagged as an Enemy
        if (collision.gameObject.CompareTag("enemy"))
        {
            // Calculate direction away from the enemy
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            // Apply instantaneous force
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

            //delay movement change
            stopKnockback = Time.time + knockbackDuration; //buffer to get off the wall
        }
    }

}
