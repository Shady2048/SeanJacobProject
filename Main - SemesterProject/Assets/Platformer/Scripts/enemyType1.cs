using UnityEngine;

public class enemyType1 : MonoBehaviour
{
    public Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    public float moveSpeed = 5f;
    float horizontalMovement = 1; //positive - right, negative - left, 0 - not moving
    bool moveLeft = false;

    //to check if needs to turn around
    public LayerMask wallLayer;

    //to provide delay between change in enemy movemnt
    public float changeDelay = 2f; //time in seconds
    float nextChange = 0f;  // 0 is default, should not register it



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //grab other elements attached to this GO for use
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        //set layer masks
        wallLayer = LayerMask.GetMask("walls");
    }

    // Update is called once per frame
    void Update()
    {
        //set movemnt
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);

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
}
