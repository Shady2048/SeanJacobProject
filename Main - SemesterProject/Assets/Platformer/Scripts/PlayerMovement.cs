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
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
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
}
