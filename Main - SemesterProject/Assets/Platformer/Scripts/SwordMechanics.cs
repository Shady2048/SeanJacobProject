using UnityEngine;

public class SwordMechanics : MonoBehaviour
{
    private int facing = -1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
}
