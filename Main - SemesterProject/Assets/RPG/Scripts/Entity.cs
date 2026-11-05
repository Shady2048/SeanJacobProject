using UnityEngine;
using System.Collections.Generic;

// This is a list of stats used by both the players and enemies in the RPG sections
public class Entity : MonoBehaviour
{
    [Header("Entity Data")]
    public string Name;
    public int MaxHP = 10;
    public int HP = 10;
    public int MaxSP = 10;
    public int SP = 10;
    public int Attack = 1;
    public int Defense = 0;

    // returns the damage dealt to the entity and whether or not the entity has died from the damage.
    public (int dmgDealt, bool hasDied) Damage(int damage)
    {
        int currentHP = HP;
        HP -= damage - Defense;
        return (currentHP - HP, HP <= 0);
    }

    public void Heal(int recover)
    {
        HP += recover;
        if(HP > MaxHP)
        {
            HP = MaxHP;
        }
    }

    // Returns true if the entity has enough SP to spend, false if it's unable to.
    public bool SpendSP(int cost)
    {
        if (SP >= cost)
        {
            SP -= cost;
            return true;
        }
        return false;
    }

}
