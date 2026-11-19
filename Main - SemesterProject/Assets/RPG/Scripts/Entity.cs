using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Required for scene switching


// This is a list of stats used by both the players and enemies in the RPG sections
// Runs before scripts at Default Time (0)
[DefaultExecutionOrder(-50)]
public class Entity : MonoBehaviour
{
    [Header("Entity Data")]
    public EntityDataSO data = null;

    public EntityDataSO template;// Start is called once before the first execution of Update after the MonoBehaviour is created
    
    
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Platformer"){
            if(data == null){
                data = saveState.instance.getEnemy(template.IDName, template);
            }

            if(saveState.returning){
                transform.position = data.lastPos;
                Debug.Log("Updating position of " + gameObject.name);
            }
        }

        if (SceneManager.GetActiveScene().name == "RPGTest")
        {
            if(data == null)
            {
                data = FindObjectsByType<BattleController>()[0].getEnemyData();
            }
        }
    }  

    // returns the damage dealt to the entity and whether or not the entity has died from the damage.
    public (int dmgDealt, bool hasDied) Damage(int damage)
    {
        int currentHP = data.HP;
        data.HP -= damage - data.Defense;
        return (currentHP - data.HP, data.HP <= 0);
    }

    public void Heal(int recover)
    {
        data.HP += recover;
        if(data.HP > data.MaxHP)
        {
            data.HP = data.MaxHP;
        }
    }

    // Returns true if the entity has enough SP to spend, false if it's unable to.
    public bool SpendSP(int cost)
    {
        if (data.SP >= cost)
        {
            data.SP -= cost;
            return true;
        }
        return false;
    }

    public void savePos(){
        data.lastPos = transform.position;
    }
}
