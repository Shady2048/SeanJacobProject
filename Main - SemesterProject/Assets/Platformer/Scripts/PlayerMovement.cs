using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // Required for scene switching

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    public SwordMechanics swordActions;
    private Animator animator;

    //entity stats
    private Entity stats;

    public float moveSpeed = 10f;

    float horizontalMovement;

    public float jumpSpeed = 5f;

    public LayerMask groundlayer;

    //knockback vars
    public float knockbackDuration = 0.2f;
    private float stopKnockback = 0f;
    private Vector2 knockbackDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //grab other elements attached to this GO for use
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<Entity>();
        animator = GetComponent<Animator>();

        //enable player controls
        saveState.controls.Player.Enable();
        saveState.instance.allEnemiesDontExist();
        saveState.leaving = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= stopKnockback){rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);}
        swordActions.flipPosition(horizontalMovement);

        if(stats.data.HP <= 0){
            saveState.controls.Player.Disable();
            SceneManager.LoadScene("Retry");
        }


        //for easy input control
        if(saveState.controls.Player.enabled){
            if(saveState.controls.Player.Attack.IsPressed()){
                animator.SetBool("isAttacking", true);
            }else{
                animator.SetBool("isAttacking", false);
            }
        }
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
            enemyType1 check = collision.gameObject.GetComponent<enemyType1>();
            if(check != null)
            rb.AddForce(knockbackDirection * check.knockbackForceToPlayer, ForceMode2D.Impulse);

            //delay movement change
            stopKnockback = Time.time + knockbackDuration; //buffer to get off the wall
        }

        if (collision.gameObject.CompareTag("goal"))
        {
            saveState.controls.Player.Disable();
            SceneManager.LoadScene("Win");
        }
    }


    public void tryEnterBattle(){
        if(saveState.instance.getBattleEnemies().Count == 0) return; //if no enemies are in battle state, return

        //save all entity positions at this time
        Entity[] allEntities = FindObjectsByType<Entity>();

        foreach(Entity entity in allEntities){
            entity.savePos();
        }

        saveState.leaving = true;
        saveState.controls.Player.Disable();
        SceneManager.LoadScene("RPGTest");
    }

}
