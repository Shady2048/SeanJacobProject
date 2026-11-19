using UnityEngine;

public class enemyBasics : MonoBehaviour
{
    public Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private SpriteRenderer sr;



    //entity stats
    private Entity stats;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //grab other elements attached to this GO for use
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<Entity>();
        sr = GetComponent<SpriteRenderer>();

    

        //if enemy dead upon return, kill it
        if(stats.data.HP <= 0) killEnemy();
    }

    // Update is called once per frame
    void Update()
    {

    }



    //to control interactions with enemy (attacks)

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player")){
            collision.gameObject.GetComponent<Entity>().Damage(stats.data.Attack); //maybe update to be a seperate attack value for platformer later for balancing reasons
            Debug.Log(collision.gameObject.GetComponent<Entity>().data.HP);
        }
    }


    private void OnTriggerEnter2D(Collider2D trigger)
    {
        // Check if the colliding object is tagged as battleZone
        if (trigger.gameObject.CompareTag("battleZone") && stats.data.canBattle)
        {
            stats.data.isInBattle = true;
        }
    }

    



    //kill enemy
    public void killEnemy(){
        sr.enabled = false;
        if(rb != null) rb.simulated = false;
        boxCollider.enabled = false;
    }

}
