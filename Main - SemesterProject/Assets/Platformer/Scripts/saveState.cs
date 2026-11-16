using UnityEngine;

public class saveState : MonoBehaviour
{

    // Static reference so any script can access this instance easily
    public static saveState instance { get; private set; }



    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
