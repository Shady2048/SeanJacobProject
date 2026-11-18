using UnityEngine;
using System.Collections.Generic;

public class saveState : MonoBehaviour
{

    // Static reference so any script can access this instance easily
    public static saveState instance { get; private set; }

    [System.Serializable]
    public class myPair
    {
        public EntityDataSO stats { get; set; }
        public bool exists { get; set; }
        public myPair(EntityDataSO statsNew, bool existsNew) {
            stats = statsNew;
            exists = existsNew;
        }
    }

    public List<myPair> enemies;

    public static bool returning = false;


    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        returning = false;
    }

    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public EntityDataSO getEnemy(string enemyType, EntityDataSO template){
        for(int i = 0; i < enemies.Count; i++){
            if(enemies[i].stats.IDName == enemyType && !enemies[i].exists){
                enemies[i].exists = true;
                return enemies[i].stats;
            }
        }

        if(returning) Debug.Log("Enemy Data not found");

        //if makes it here, it does not exist yet, needs to be created
        
        enemies.Add(new myPair(Instantiate(template), false));

        for(int i = 0; i < enemies.Count; i++){
            if(enemies[i].stats.IDName == enemyType && !enemies[i].exists){
                Debug.Log("Returning created data");
                enemies[i].exists = true;
                return enemies[i].stats;
            }
        }

        return null;
    }


    public List<EntityDataSO> getBattleEnemies(){
        List<EntityDataSO> endList;
        endList = new List<EntityDataSO>{};

        for(int i = 0; i < enemies.Count; i++){
            if(enemies[i].stats.isInBattle){
                endList.Add(enemies[i].stats);
            }
        }

        return endList;
    }
}
