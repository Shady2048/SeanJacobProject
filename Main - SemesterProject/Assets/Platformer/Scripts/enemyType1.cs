using UnityEngine;

public class enemyType1 : MonoBehaviour
{
    public Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    public float moveSpeed = 5f;
    float horizontalMovement = 0; //positive - right, negative - left, 0 - not moving
    public bool moveLeft = false;

    //to check if needs to turn around
    public LayerMask wallLayer;

    //to provide delay between change in enemy movemnt
    public float changeDelay = 2f; //time in seconds
    float nextChange = 0f;  // 0 is default, should not register it

    //knockback vars
    public float knockbackForce = 10f;
    public float hitknockbackForce = 20f;
    public float knockbackDuration = 0.2f;
    private float stopKnockback = 0f;
    private Vector2 knockbackDirection;

    //entity stats
    private Entity stats;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //grab other elements attached to this GO for use
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<Entity>();

        //set layer masks
        wallLayer = LayerMask.GetMask("walls");
        
        //set enemy direction 
        horizontalMovement = moveLeft ? -1 : 1;
    }

    // Update is called once per frame
    void Update()
    {
        //set movemnt
        if(Time.time >= stopKnockback){rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);}

        //detect wall collision
        if(boxCollider.IsTouchingLayers(wallLayer) && horizontalMovement != 0 && Time.time >= nextChange) {
            horizontalMovement = 0;
            nextChange = Time.time + changeDelay;
        }
        
        //change direction
        if(Time.time >= nextChange && !moveLeft && horizontalMovement == 0 && nextChange != 0){
            horizontalMovement = -1;
            moveLeft = true;
            nextChange = Time.time + 0.05f; //buffer to get off the wall
            
        }else if(Time.time >= nextChange && moveLeft && horizontalMovement == 0 && nextChange != 0){
            horizontalMovement = 1;
            moveLeft = false;
            nextChange = Time.time + 0.05f; //buffer to get off the wall
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the colliding object is tagged as an Enemy
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("enemy"))
        {
            // Calculate direction away from the enemy
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            // Apply instantaneous force
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

            //delay movement change
            stopKnockback = Time.time + knockbackDuration;

            if(collision.gameObject.CompareTag("Player")){
                collision.gameObject.GetComponent<Entity>().Damage(stats.Attack); //maybe update to be a seperate attack value for platformer later for balancing reasons
                Debug.Log(collision.gameObject.GetComponent<Entity>().HP);
            }else{
                horizontalMovement = -horizontalMovement;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        // Check if the colliding object is tagged as an Enemy
        if (trigger.gameObject.CompareTag("sword"))
        {
            // Calculate direction away from the enemy
            Vector2 knockbackDirection = (transform.position - trigger.transform.position).normalized;
            // Apply instantaneous force
            rb.AddForce(knockbackDirection * hitknockbackForce, ForceMode2D.Impulse);

            //delay movement change
            stopKnockback = Time.time + knockbackDuration;
            stats.Damage(trigger.gameObject.GetComponentInParent<Entity>().Attack);
            Debug.Log(stats.HP);
        }
    }
}
