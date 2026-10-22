using Unity.VisualScripting;
using UnityEngine;

enum BattleState { Start, PlayerTurn, EnemyTurn, End };
public class BattleController : MonoBehaviour
{

    BattleState state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = BattleState.Start;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
