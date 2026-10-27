using UnityEngine;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(int damage)
    {
        HP -= damage - Defense;
    }
}
