using UnityEngine;

[CreateAssetMenu(fileName = "EntityDataSO", menuName = "Scriptable Objects/EntityDataSO")]
public class EntityDataSO : ScriptableObject
{
    [Header("Entity Data")]
    public string Name;
    public int MaxHP = 10;
    public int HP = 10;
    public int MaxSP = 10;
    public int SP = 10;
    public int Attack = 1;
    public int Defense = 0;
    public Vector3 lastPos;
}
