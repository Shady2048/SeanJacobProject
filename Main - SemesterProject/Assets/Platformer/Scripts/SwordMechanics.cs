using UnityEngine;

public class SwordMechanics : MonoBehaviour
{
    private int facing = -1;

    //entity stats
    private Entity stats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stats = GetComponentInParent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void flipPosition(float face){
        if((face < 0 && facing == 1) || (face > 0 && facing == -1)) {
            transform.localPosition = -transform.localPosition;
            facing = -facing;
        }
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        // Check if the colliding object is tagged as an Enemy
        if (trigger.gameObject.CompareTag("enemy"))
        {
            trigger.gameObject.GetComponent<Entity>().Damage(stats.Attack); //maybe update to be a seperate attack value for platformer later for balancing reasons
            Debug.Log(trigger.gameObject.GetComponent<Entity>().HP);
            if(trigger.gameObject.GetComponent<Entity>().HP <= 0){
                trigger.gameObject.GetComponent<enemyType1>().killEnemy(); //update later to be more of a general enemy script
            }
            
        }
    }
}
